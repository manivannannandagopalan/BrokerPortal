# Phase Nine: FastAPI headless implementation

## Delivered

- FastAPI service under `services/fastapi`.
- Async SQLAlchemy persistence using PostgreSQL and `asyncpg`.
- User Management endpoints for listing, invitations, and status updates.
- Small Commercial endpoints for submission intake and queue filtering.
- Underwriting Guidelines endpoint with line and text search.
- Integration health endpoint for PostgreSQL and Duck Creek.
- Audit event search endpoint.
- Development permission claims with an explicit production Auth0 validation boundary.
- Duck Creek adapter boundary with a mock implementation for local development.
- Automatic OpenAPI documentation at `/docs` and `/openapi.json`.

## Local runtime

The FastAPI service requires PostgreSQL. Start the repository's database with:

```powershell
docker compose -f docker-compose.postgres.yml up -d
```

Then run:

```powershell
Set-Location services/fastapi
.\.venv\Scripts\python.exe -m uvicorn app.main:app --reload --port 5082
```

The current workstation has Python and the FastAPI dependencies installed, but Docker/PostgreSQL is not yet available. For no-admin local work, the launcher automatically uses SQLite and persists to `services/fastapi/brokerportal.local.db`; the same SQLAlchemy models can switch to PostgreSQL through `BROKERPORTAL_DATABASE_URL`. The service has passed live local persistence testing for user invitations and commercial submissions.

## Production hardening

Configure Auth0 issuer/audience validation, broker-scope claim enforcement, managed PostgreSQL credentials, migrations, rate limits, and a real Duck Creek adapter before production deployment. The browser portal should call these endpoints rather than own business data; local seed data remains a temporary UI fallback until the API is reachable.