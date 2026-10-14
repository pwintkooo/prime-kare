import json

from fastapi import HTTPException
from openai import OpenAIError

from core.config import OPENAI_MODEL
from core.openai_client import client
from agent.handlers import tool_handlers
from agent.tools import tools
from schemas.chat import ChatMessage

MAX_TOOL_ROUNDS = 5
MAX_OUTPUT_TOKENS = 1000


INSTRUCTIONS = (
    "You are PrimeKare's helpful car workshop assistant. "
    "Answer clearly and politely. "
    "When listing available services, use a Markdown table "
    "with Service, Description, Price, and Duration columns. "
    "Display prices in Singapore dollars (S$) with two "
    "decimal places, such as S$89.90, only when the backend "
    "specifies that currency. Display durations in minutes. "
    "When listing registered vehicles, use a Markdown table "
    "with Plate Number, Make, Model, Year, and Status columns. "
    "Never display internal database IDs or customer IDs. "
    "If there are no registered vehicles, say so clearly. "
    "Use readable Markdown formatting. Only report information "
    "present in the relevant tool results. Never invent service "
    "details, vehicle details, prices, or durations. "
    "If a tool returns an error, explain that the information "
    "is temporarily unavailable. Never claim to have created "
    "or changed a booking unless the backend confirms it."
)


def generate_reply(
    messages: list[ChatMessage],
    authorization: str,
) -> str:
    if client is None:
        raise HTTPException(
            status_code=500,
            detail="OpenAI API key is not configured.",
        )

    conversation = [message.model_dump() for message in messages]

    try:
        response = client.responses.create(
            model=OPENAI_MODEL,
            instructions=INSTRUCTIONS,
            input=conversation,
            tools=tools,
            max_output_tokens=MAX_OUTPUT_TOKENS,
        )

        for _ in range(MAX_TOOL_ROUNDS):
            function_calls = [
                item for item in response.output if item.type == "function_call"
            ]

            if not function_calls:
                break

            # Preserve the model's function-call items.
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

                        result = handler(
                            arguments,
                            authorization,
                        )

                    except HTTPException as e:
                        if e.status_code == 401:
                            raise
                        result = {"error": "The requested information is temporarily unavailable."}
                        
                    except (ValueError, TypeError):
                        result = {"error": "Invalid tool arguments."}

                conversation.append(
                    {
                        "type": "function_call_output",
                        "call_id": call.call_id,
                        "output": json.dumps(result),
                    }
                )

            response = client.responses.create(
                model=OPENAI_MODEL,
                instructions=INSTRUCTIONS,
                input=conversation,
                tools=tools,
                max_output_tokens=MAX_OUTPUT_TOKENS,
            )

        else:
            # The last response may already be a valid final answer.
            # Only reject it if it still requests another tool.
            function_calls = [
                item for item in response.output if item.type == "function_call"
            ]

            if function_calls:
                raise HTTPException(
                    status_code=502,
                    detail="The AI exceeded the tool-call limit.",
                )

        if not response.output_text:
            raise HTTPException(
                status_code=502,
                detail="The AI returned no text response.",
            )

        return response.output_text

    except OpenAIError as e:
        print(f"OpenAI error: {type(e).__name__}: {e}")

        raise HTTPException(
            status_code=502,
            detail="The AI service request failed. Check the server logs.",
        ) from e
