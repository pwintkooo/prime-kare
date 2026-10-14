import jwt

from fastapi import HTTPException
from jwt import InvalidTokenError, ExpiredSignatureError

from core.config import JWT_SECRET, JWT_ISSUER, JWT_AUDIENCE


def validate_access_token(token: str) -> dict:
    """Validate the access token issued by the PrimeKare backend."""

    if not JWT_SECRET:
        raise HTTPException(
            status_code=500,
            detail="JWT authentication is not configured.",
        )

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

        if not user_id:
            raise HTTPException(
                status_code=401,
                detail={
                    "code": "SESSION_EXPIRED",
                    "message": (
                        "Your session is invalid. Please log in again "
                        "to use the AI assistant."
                    ),
                },
                headers={"WWW-Authenticate": "Bearer"},
            )

        return payload

    except ExpiredSignatureError:
        raise HTTPException(
            status_code=401,
            detail={
                "code": "SESSION_EXPIRED",
                "message": (
                    "Your session has expired. Please log in again "
                    "to use the AI assistant."
                ),
            },
            headers={"WWW-Authenticate": "Bearer"},
        )

    except InvalidTokenError:
        raise HTTPException(
            status_code=401,
            detail={
                "code": "SESSION_EXPIRED",
                "message": (
                    "Your session is invalid or has expired. "
                    "Please log in again to use the AI assistant."
                ),
            },
            headers={"WWW-Authenticate": "Bearer"},
        )
