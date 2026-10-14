from fastapi import APIRouter, Depends, Request
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer

from agent.service import generate_reply
from core.limiter import limiter
from schemas.chat import ChatRequest, ChatResponse
from core.auth import validate_access_token

router = APIRouter()
bearer_scheme = HTTPBearer()


@router.post("/chat", response_model=ChatResponse)
@limiter.limit("10/minute")
def chat(
    request: Request,
    chat_request: ChatRequest,
    credentials: HTTPAuthorizationCredentials = Depends(bearer_scheme),
):
    authorization = f"Bearer {credentials.credentials}"

    # Validate the session before processing the conversation.
    validate_access_token(credentials.credentials)

    reply = generate_reply(
        messages=chat_request.messages,
        authorization=authorization,
    )

    return ChatResponse(reply=reply)
