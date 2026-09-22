# BrokerPortal FastAPI service

## Run locally

1. Create a virtual environment: `python -m venv .venv`.
3. Install dependencies: `python -m pip install -r requirements.txt`.
4. Copy `.env.example` to `.env` and set the database password.
5. Run the API: `.\run-local.ps1`.

Interactive OpenAPI documentation is available at `http://127.0.0.1:5082/docs`.

The runner uses PostgreSQL when `127.0.0.1:5432` is available. Without PostgreSQL, it automatically uses the file-backed SQLite database `brokerportal.local.db`, which requires no administrator access. The runner uses `--reload-dir app` so WatchFiles does not scan `.venv`.

SQLite is for local development only. Set `BROKERPORTAL_DATABASE_URL` to a PostgreSQL `postgresql+asyncpg://...` connection string in shared or production environments.

In development, the API uses a local-admin claim set when Auth0 is not configured. Production environments must set Auth0 domain and audience; the authentication adapter must then validate JWT issuer, audience, signature, expiry, permissions, and broker scope.
