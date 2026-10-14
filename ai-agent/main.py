import os

from dotenv import load_dotenv
from fastapi import FastAPI, HTTPException
from openai import OpenAI, OpenAIError
from pydantic import BaseModel, Field
from typing import Literal
import json

# Load settings from .env
load_dotenv()

# Read configuration
api_key = os.getenv("OPENAI_API_KEY")
model = os.getenv("OPENAI_MODEL", "gpt-5.4-mini")

# Create the FastAPI application
app = FastAPI(
    title="PrimeKare AI Agent",
    description="AI assistant for PrimeKare",
    version="0.1.0",
)

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


def get_available_services():
    """Return sample services offered by PrimeKare."""

    return [
        {
            "name": "Oil Change",
            "description": "Engine oil and oil filter replacement.",
        },
        {
            "name": "Brake Inspection",
            "description": "Inspection of brake pads and braking components.",
        },
        {
            "name": "General Vehicle Inspection",
            "description": "A general check of the vehicle's condition.",
        },
    ]


@app.get("/")
def home():
    return {"message": "PrimeKare AI Agent is running!"}


@app.post("/chat", response_model=ChatResponse)
def chat(request: ChatRequest):
    if client is None:
        raise HTTPException(
            status_code=500,
            detail="OpenAI API key is not configured.",
        )

    try:
        tools = [
            {
                "type": "function",
                "name": "get_available_services",
                "description": (
                    "Get the list of services currently offered by PrimeKare. "
                    "Use this when a customer asks what services are available."
                ),
                "parameters": {
                    "type": "object",
                    "properties": {},
                    "required": [],
                    "additionalProperties": False,
                },
                "strict": True,
            }
        ]
        
        conversation = [
            {
                "role": message.role,
                "content": message.content,
            }
            for message in request.messages
        ]

        response = client.responses.create(
            model=model,
            instructions=(
                "You are PrimeKare's AI assistant for a car workshop management system. "
                "Your responsibilities are to help customers understand vehicle "
                "servicing, workshop services, vehicle maintenance, and bookings. "
                "Answer in a friendly, professional, and easy-to-understand way. "
                "Keep answers concise unless the customer asks for more detail. "
                "You do not currently have access to PrimeKare's database, customer "
                "records, vehicles, service prices, or appointment availability. "
                "Never invent service prices, customer information, booking details, "
                "or appointment availability. If you do not know something, say so. "
                "Never claim that you have created, updated, or cancelled a booking. "
                "Explain that you cannot perform these actions yet. "
                "Only answer within your capabilities. If a request requires access "
                "to real PrimeKare data or an action you cannot perform, explain "
                "the limitation clearly."
            ),
            input=conversation,
            tools=tools,
        )
        
        # Check whether the AI requested a tool.
        for item in response.output:
            if item.type == "function_call":
                if item.name == "get_available_services":
                    # Execute our Python function.
                    result = get_available_services()

                    # Add the AI's tool request and the function result
                    # to the conversation.
                    conversation.append(item.model_dump(exclude_none=True))

                    conversation.append(
                        {
                            "type": "function_call_output",
                            "call_id": item.call_id,
                            "output": json.dumps(result),
                        }
                    )

                    # Let the AI turn the result into a natural answer.
                    response = client.responses.create(
                        model=model,
                        instructions=(
                            "You are PrimeKare's helpful car workshop assistant. "
                            "Answer clearly and politely. "
                            "Use the supplied tool results to answer accurately. "
                            "Do not invent prices or claim to have made bookings."
                        ),
                        input=conversation,
                    )

                    break


        return ChatResponse(reply=response.output_text)

    # except OpenAIError:
    #     raise HTTPException(
    #         status_code=502,
    #         detail=(
    #             "The AI service request failed. "
    #             "Check your API key, model access, and billing."
    #         ),
    #     )

    except OpenAIError as e:
        print(f"OpenAI error: {type(e).__name__}: {e}")

    raise HTTPException(
        status_code=502,
        detail=f"OpenAI error: {type(e).__name__}: {e}",
    )
