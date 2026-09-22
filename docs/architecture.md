# Architecture foundation

## Target shape

- `apps/portal`: Angular application shell, Auth0 browser integration, route guards, and user-management screens.
- `services/identity-api`: ASP.NET Core API, JWT validation, authorization policies, OpenAPI, health checks, correlation IDs, and structured logging.
- `services/identity-api/Domain`: user and broker aggregates with application contracts independent of Duck Creek.
- `services/identity-api/Infrastructure`: SQL Server persistence and a Duck Creek gateway adapter.
- `tests`: unit tests for domain/application logic and integration tests for the API boundary.

## Decisions for the next increment

1. Auth0 authenticates users; the API remains the authority for application roles and broker-scoped permissions.
2. The Duck Creek dependency is hidden behind `IDuckCreekUserGateway`; the first development implementation is a mock gateway.
3. SQL Server stores the local user projection, broker association, audit events, and authorization assignments.
4. Every API request receives or propagates an `X-Correlation-ID`; logs must include it.
5. OpenAPI is generated from the API and secured endpoints require bearer authentication.

## Missing prerequisites

Install Node.js plus the Angular CLI for the portal, and the .NET 8 SDK for the API and test projects. SQL Server and Auth0 tenant values are environment-specific and must be supplied through local secrets or deployment configuration, never committed to source control.

## Security and delivery notes

- Validate issuer, audience, signature, and expiry for JWTs.
- Default to deny when a broker scope or policy is absent.
- Keep secrets outside Git and scan commits in CI.
- Add dependency, secret, SAST, and container scans to the pipeline skeleton.
- Treat the Duck Creek contract as provisional until the vendor API, identity mapping, rate limits, and failure semantics are confirmed.
