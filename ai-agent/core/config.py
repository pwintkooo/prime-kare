import os

from dotenv import load_dotenv

load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_MODEL = os.getenv("OPENAI_MODEL", "gpt-5.4-mini")

PRIMEKARE_SERVICES_URL = os.getenv("PRIMEKARE_SERVICES_URL")
PRIMEKARE_API_URL = os.getenv("PRIMEKARE_API_URL")
PRIMEKARE_FRONTEND_URL = os.getenv("PRIMEKARE_FRONTEND_URL")

JWT_SECRET = os.getenv("JWT_SECRET")
JWT_ISSUER = os.getenv("JWT_ISSUER")
JWT_AUDIENCE = os.getenv("JWT_AUDIENCE")
