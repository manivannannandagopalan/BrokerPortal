# BrokerPortal FastAPI service

## Run locally

1. Start PostgreSQL using `docker compose -f docker-compose.postgres.yml up -d`.
2. Create a virtual environment: `python -m venv .venv`.
3. Install dependencies: `python -m pip install -r requirements.txt`.
4. Copy `.env.example` to `.env` and set the database password.
5. Run the API: `python -m uvicorn app.main:app --reload --port 5082`.

Interactive OpenAPI documentation is available at `http://127.0.0.1:5082/docs`.

In development, the API uses a local-admin claim set when Auth0 is not configured. Production environments must set Auth0 domain and audience; the authentication adapter must then validate JWT issuer, audience, signature, expiry, permissions, and broker scope.