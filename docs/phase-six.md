# Phase Six: ASP.NET Core API foundation

## Delivered

- .NET 8 minimal API project under `services/identity-api`.
- `/health` endpoint backed by ASP.NET Core health checks.
- Correlation ID propagation through `X-Correlation-ID` response headers and logging scope.
- User listing with search, status, and broker filters.
- Idempotent pending invitation check and mock Duck Creek provisioning boundary.
- User status update endpoint.
- Configuration placeholders for Auth0, SQL Server, and Duck Creek without committed secrets.

## Local verification

Install the .NET 8 SDK, then run from `services/identity-api`:

```powershell
dotnet run
```

The browser prototype continues to use local data until the Angular client is configured against this service. The current store is intentionally in-memory; SQL Server persistence, JWT validation, and policy authorization are the next production hardening steps.
