import os

from dotenv import load_dotenv
from fastapi import FastAPI, HTTPException
from openai import OpenAI, OpenAIError
from pydantic import BaseModel, Field

# Load settings from .env
load_dotenv()

# Read configuration
api_key = os.getenv("OPENAI_API_KEY")
model = os.getenv("OPENAI_MODEL", "gpt-4.1-mini")

# Create the FastAPI application
app = FastAPI(
    title="PrimeKare AI Agent",
    description="AI assistant for PrimeKare",
    version="0.1.0",
)

# Create the OpenAI client
client = OpenAI(api_key=api_key) if api_key else None


# Define the expected request body
class ChatRequest(BaseModel):
    message: str = Field(min_length=1, max_length=2000)


# Define the response format
class ChatResponse(BaseModel):
    reply: str


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
        response = client.responses.create(
            model=model,
            instructions=(
                "You are PrimeKare's helpful car workshop assistant. "
                "Answer questions clearly and politely. "
                "You cannot access real booking or customer data yet. "
                "Never claim to have created or changed a booking."
            ),
            input=request.message,
        )

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