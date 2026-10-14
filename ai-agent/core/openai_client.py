from openai import OpenAI

from core.config import OPENAI_API_KEY

client = (
    OpenAI(
        api_key=OPENAI_API_KEY,
        timeout=30.0,
        max_retries=1,
    )
    if OPENAI_API_KEY
    else None
)
