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
            "Retrieve vehicles accessible to the currently "
            "authenticated user. Use this when the user asks "
            "about their registered vehicles. "
            "Do not ask for a customer ID."
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
