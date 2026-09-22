# Phase six without the .NET SDK

The Windows machine does not currently have a usable .NET SDK. The following portable substitutes unblock development:

- `tools/mock-api.ps1` mirrors the health, user listing, and invitation contract.
- `tools/smoke-test.ps1` verifies health, correlation IDs, filtering, and invitations.
- `database/migrations/001_initial_schema.sql` defines the SQL Server persistence foundation.
- `config/authorization-policies.json` defines deny-by-default roles and broker scope.
- `config/auth0.example.json` documents the browser/API identity configuration without real secrets.

Run the mock API in one PowerShell terminal:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\mock-api.ps1
```

Run the smoke test in another:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\smoke-test.ps1
```

The mock is for local contract testing only. It does not provide JWT validation, SQL persistence, or production authorization enforcement; those remain responsibilities of the ASP.NET Core service.
