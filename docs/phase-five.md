# Phase Five: API and delivery foundation

## Delivered

- Versioned OpenAPI contract in `docs/openapi.yaml`.
- Health, users, invitations, policies, integration health, and audit-event endpoints.
- Bearer JWT security scheme and explicit `401`/`403` responses.
- Broker-scoped user and policy schemas.
- GitHub Actions workflow with static asset validation, secret scanning, conditional .NET build/test, and dependency review.

## Runtime prerequisites

The API implementation requires the .NET 8 SDK. The Angular application requires Node.js and the Angular CLI. These are intentionally not installed into the repository or committed as binaries.

Required deployment configuration:

- `Auth0__Domain`
- `Auth0__Audience`
- `ConnectionStrings__BrokerPortal`
- `DuckCreek__BaseUrl`
- `DuckCreek__ClientId`
- `DuckCreek__ClientSecret`

All values must be supplied through a secret manager or environment configuration. Never place them in `appsettings.json`, the browser bundle, or Git history.

## Next phase

Generate the ASP.NET Core solution from this contract, add SQL Server migrations, implement the mock Duck Creek gateway, and add authenticated integration tests using ephemeral test data.
