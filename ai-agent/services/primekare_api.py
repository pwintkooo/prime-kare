import httpx

from fastapi import HTTPException

from core.config import (
    PRIMEKARE_API_URL,
    PRIMEKARE_SERVICES_URL,
)


def call_protected_api(
    path: str,
    authorization: str,
):
    if not PRIMEKARE_API_URL:
        raise HTTPException(
            status_code=500,
            detail="PrimeKare API URL is not configured.",
        )

    try:
        response = httpx.get(
            f"{PRIMEKARE_API_URL.rstrip('/')}/{path.lstrip('/')}",
            headers={"Authorization": authorization},
            timeout=10.0,
        )

        
        if response.status_code == 401:
            raise HTTPException(
                status_code=401,
                detail={
                    "code": "SESSION_EXPIRED",
                    "message": "Your session has expired. Please log in again to use the AI assistant.",
                },
            )

        if response.status_code == 403:
            raise HTTPException(
                status_code=403,
                detail={
                    "code": "FORBIDDEN",
                    "message": "You do not have permission to access this information.",
                },
            )

        response.raise_for_status()
        return response.json()

    except httpx.HTTPStatusError as e:
        print(f"PrimeKare API returned HTTP {e.response.status_code}")
        raise HTTPException(
            status_code=502,
            detail="The PrimeKare backend request failed.",
        ) from e

    except httpx.RequestError as e:
        print(f"PrimeKare API connection error: {type(e).__name__}")
        raise HTTPException(
            status_code=502,
            detail="Unable to connect to the PrimeKare backend.",
        ) from e

    except ValueError as e:
        # For example, the backend returned invalid JSON.
        raise HTTPException(
            status_code=502,
            detail="The PrimeKare backend returned invalid data.",
        ) from e


def get_available_services():
    """Retrieve active services from the PrimeKare backend."""

    if not PRIMEKARE_SERVICES_URL:
        return {"error": "PrimeKare services API URL is not configured."}

    try:
        response = httpx.get(
            PRIMEKARE_SERVICES_URL,
            timeout=10.0,
        )
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
            if isinstance(service, dict) and service.get("isActive") is True
        ]

    except (httpx.HTTPError, ValueError, KeyError) as e:
        print(f"PrimeKare services API error: {type(e).__name__}")
        return {"error": "Unable to retrieve services from PrimeKare."}


def get_my_vehicles(authorization: str):
    """Retrieve vehicles accessible to the authenticated user."""

    vehicles = call_protected_api(
        path="api/Vehicles",
        authorization=authorization,
    )

    if not isinstance(vehicles, list):
        return {"error": "Unexpected response from the vehicles API."}

    try:
        return [
            {
                "plateNumber": vehicle["plateNumber"],
                "make": vehicle["make"],
                "model": vehicle["model"],
                "year": vehicle["year"],
                "status": vehicle["status"],
            }
            for vehicle in vehicles
            if isinstance(vehicle, dict) and not vehicle.get("isDeleted", False)
        ]

    except KeyError as e:
        print(f"PrimeKare vehicle data is missing a field: {e}")
        return {"error": "Unexpected vehicle data from PrimeKare."}
