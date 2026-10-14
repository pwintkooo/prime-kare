import os

from dotenv import load_dotenv
from fastapi import FastAPI, HTTPException, Header, Depends
from openai import OpenAI, OpenAIError
from pydantic import BaseModel, Field
from typing import Literal
import json
import httpx
from fastapi.security import HTTPBearer

# Load settings from .env
load_dotenv()

# Read configuration
api_key = os.getenv("OPENAI_API_KEY")
model = os.getenv("OPENAI_MODEL", "gpt-5.4-mini")
services_url = os.getenv("PRIMEKARE_SERVICES_URL")
api_base_url = os.getenv("PRIMEKARE_API_URL")

# Create the FastAPI application
app = FastAPI(
    title="PrimeKare AI Agent",
    description="AI assistant for PrimeKare",
    version="0.1.0",
)

bearer_scheme = HTTPBearer()

# Create the OpenAI client
client = OpenAI(api_key=api_key) if api_key else None


# Define the expected request body
class ChatMessage(BaseModel):
    role: Literal["user", "assistant"]
    content: str = Field(min_length=1, max_length=2000)


class ChatRequest(BaseModel):
    messages: list[ChatMessage] = Field(min_length=1, max_length=20)


# Define the response format
class ChatResponse(BaseModel):
    reply: str


def call_protected_api(
    path: str,
    authorization: str | None,
):
    if not authorization or not authorization.lower().startswith("bearer "):
        raise HTTPException(
            status_code=401,
            detail="Authentication is required.",
        )

    if not api_base_url:
        raise HTTPException(
            status_code=500,
            detail="PrimeKare API URL is not configured.",
        )

    try:
        response = httpx.get(
            f"{api_base_url.rstrip('/')}/{path.lstrip('/')}",
            headers={
                "Authorization": authorization,
            },
            timeout=10.0,
        )

        if response.status_code in (401, 403):
            raise HTTPException(
                status_code=response.status_code,
                detail="The backend rejected the authentication request.",
            )

        response.raise_for_status()
        return response.json()

    except httpx.HTTPStatusError as e:
        print(f"PrimeKare API returned HTTP {e.response.status_code}")

        raise HTTPException(
            status_code=502,
            detail="The PrimeKare backend request failed.",
        )

    except httpx.RequestError as e:
        print(f"PrimeKare API connection error: {type(e).__name__}")

        raise HTTPException(
            status_code=502,
            detail="Unable to connect to the PrimeKare backend.",
        )


def get_available_services():
    """Retrieve active services from the PrimeKare backend."""

    if not services_url:
        return {"error": "PrimeKare services API URL is not configured."}

    try:
        response = httpx.get(services_url, timeout=10.0)
        response.raise_for_status()

        services = response.json()

        if not isinstance(services, list):
            return {"error": "Unexpected response from the services API."}

        return [
            {
                "id": service["id"],
                "name": service["name"],
                "description": service.get("description"),
                "price": service.get("price"),
                "estimatedMinutes": service.get("estimatedMinutes"),
            }
            for service in services
            if service.get("isActive") is True
        ]

    except (httpx.HTTPError, ValueError, KeyError) as e:
        print(f"PrimeKare API error: {type(e).__name__}: {e}")
        return {"error": "Unable to retrieve services from PrimeKare."}


def get_my_vehicles(authorization: str):
    vehicles = call_protected_api(
        path="api/Vehicles",
        authorization=authorization,
    )

    # Return only fields the AI needs to answer the user.
    return [
        {
            "id": vehicle["id"],
            "plateNumber": vehicle["plateNumber"],
            "make": vehicle["make"],
            "model": vehicle["model"],
            "year": vehicle["year"],
            "status": vehicle["status"],
        }
        for vehicle in vehicles
        if not vehicle.get("isDeleted", False)
    ]


tools = [
    {
        "type": "function",
        "name": "get_available_services",
        "description": (
            "Get the services offered by PrimeKare. "
            "Use this when a customer asks about available services."
        ),
        "parameters": {
            "type": "object",
            "properties": {},
            "required": [],
            "additionalProperties": False,
        },
        "strict": True,
    },
    {
        "type": "function",
        "name": "get_my_vehicles",
        "description": (
            "Retrieve vehicles accessible to the currently authenticated user. "
            "Use this when the user asks about their registered vehicles. "
            "Do not ask the user for a customer ID."
        ),
        "parameters": {
            "type": "object",
            "properties": {},
            "required": [],
            "additionalProperties": False,
        },
        "strict": True,
    },
]

# Only functions registered here can be executed by the AI agent.
tool_handlers = {
    "get_available_services": lambda arguments, authorization: (
        get_available_services()
    ),
    "get_my_vehicles": lambda arguments, authorization: (
        get_my_vehicles(authorization)
    ),
}


@app.get("/")
def home():
    return {"message": "PrimeKare AI Agent is running!"}


@app.post("/chat", response_model=ChatResponse)
def chat(
    request: ChatRequest,
    credentials = Depends(bearer_scheme),
):
    authorization = f"Bearer {credentials.credentials}"
    
    if client is None:
        raise HTTPException(
            status_code=500,
            detail="OpenAI API key is not configured.",
        )

    try:
        conversation = [
            {
                "role": message.role,
                "content": message.content,
            }
            for message in request.messages
        ]

        instructions = (
            "You are PrimeKare's helpful car workshop assistant. "
            "Use the available tools to retrieve real service information. "
            "Only report information present in the tool results. "
            "Display prices with two decimal places, such as 89.90. "
            "Do not assume a currency unless the backend specifies it. "
            "Display service durations in minutes. "
            "Use a clear, readable list or table when presenting multiple services. "
            "If a tool returns an error, explain that the information is "
            "temporarily unavailable. Never invent prices or claim to have "
            "created or changed a booking."
        )

        response = client.responses.create(
            model=model,
            instructions=instructions,
            input=conversation,
            tools=tools,
        )

        # Allow up to 5 rounds of tool calls.
        for _ in range(5):
            function_calls = [
                item for item in response.output if item.type == "function_call"
            ]

            # No more tools requested: we have the final answer.
            if not function_calls:
                break

            # Preserve the model's complete response for the next call.
            conversation.extend(response.output)

            for call in function_calls:
                handler = tool_handlers.get(call.name)

                if handler is None:
                    result = {"error": "This tool is not available."}
                else:
                    try:
                        arguments = json.loads(call.arguments)

                        if not isinstance(arguments, dict):
                            raise ValueError("Tool arguments must be an object.")

                        result = handler(arguments, authorization)

                    except (ValueError, TypeError):
                        result = {"error": "Invalid tool arguments."}

                conversation.append(
                    {
                        "type": "function_call_output",
                        "call_id": call.call_id,
                        "output": json.dumps(result),
                    }
                )

            # Send tool results back to the model.
            response = client.responses.create(
                model=model,
                instructions=instructions,
                input=conversation,
                tools=tools,
            )

        else:
            # The loop used all 5 iterations.
            # Check whether the latest response still requests a tool.
            function_calls = [
                item for item in response.output if item.type == "function_call"
            ]

            if function_calls:
                raise HTTPException(
                    status_code=502,
                    detail="The AI exceeded the tool-call limit.",
                )

        return ChatResponse(reply=response.output_text)

    except OpenAIError as e:
        print(f"OpenAI error: {type(e).__name__}: {e}")

        raise HTTPException(
            status_code=502,
            detail="The AI service request failed. Check the server logs.",
        )
