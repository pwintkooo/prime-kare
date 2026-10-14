import jwt

from fastapi import Request
from jwt import InvalidTokenError
from slowapi import Limiter
from slowapi.util import get_remote_address

from core.config import (
    JWT_SECRET,
    JWT_ISSUER,
    JWT_AUDIENCE,
)


def get_user_rate_limit_key(request: Request) -> str:
    authorization = request.headers.get("Authorization", "")

    if not authorization.lower().startswith("bearer "):
        return f"ip:{get_remote_address(request)}"

    token = authorization.split(" ", 1)[1].strip()

    if not all([JWT_SECRET, JWT_ISSUER, JWT_AUDIENCE]):
        return f"ip:{get_remote_address(request)}"

    try:
        payload = jwt.decode(
            token,
            JWT_SECRET,
            algorithms=["HS256"],
            issuer=JWT_ISSUER,
            audience=JWT_AUDIENCE,
        )

        user_id = (
            payload.get("sub")
            or payload.get("nameid")
            or payload.get(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
            )
        )

        if user_id is not None:
            return f"user:{user_id}"

    except InvalidTokenError:
        pass

    return f"ip:{get_remote_address(request)}"


limiter = Limiter(key_func=get_user_rate_limit_key)
