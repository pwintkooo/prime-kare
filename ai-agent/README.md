py -3 -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install fastapi "uvicorn[standard]" openai python-dotenv
python -m pip freeze > requirements.txt

//to run the project
python -m uvicorn main:app --reload