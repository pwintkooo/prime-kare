from services.primekare_api import (
    get_available_services,
    get_my_vehicles,
)


def handle_get_available_services(
    arguments: dict,
    authorization: str,
):
    return get_available_services()


def handle_get_my_vehicles(
    arguments: dict,
    authorization: str,
):
    return get_my_vehicles(authorization)


tool_handlers = {
    "get_available_services": handle_get_available_services,
    "get_my_vehicles": handle_get_my_vehicles,
}
