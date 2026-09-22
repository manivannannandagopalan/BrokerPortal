# Local Development Setup

## Required versions

The versions below are working assumptions until the organization approves the toolchain:

- .NET SDK 8.x
- Node.js 20.x and npm 10.x
- Angular CLI 17.x
- SQL Server or LocalDB for development
- Git 2.x

## Setup

1. Install the approved .NET SDK, Node.js, Angular CLI, SQL Server tooling, and Git.
2. Restore .NET dependencies with `dotnet restore`.
3. Install web dependencies from `src/portal-web` with `npm ci`.
4. Copy the approved local configuration template when it is added. Do not commit credentials.
5. Apply database migrations only after the SQL Server connection and migration policy are approved.
6. Run the API with `dotnet run --project src/portal-api/Portal.Api`.
7. Run the web shell with `npm start` from `src/portal-web`.

## External integrations

Auth0 values in `appsettings.json` are placeholders. Replace them only through local secret configuration. The Duck Creek adapter is a mock and must not be used as a production integration. Real endpoints, authentication, contracts, rate limits, and failure semantics require an approved integration design and contract tests.

## Validation

Expected validation commands after prerequisites are installed:

```powershell
dotnet build
dotnet test
Set-Location src/portal-web
npm ci
npm test
npm run build
```