# Phase Seven: API authentication and authorization

## Delivered

- Auth0 JWT bearer validation configured from `Auth0:Domain` and `Auth0:Audience`.
- HTTPS metadata required for token discovery.
- Health checks remain anonymous for platform probes.
- User endpoints require explicit permissions:
  - `users.read`
  - `users.invite`
  - `users.status.write`
- Authorization defaults to deny when the Auth0 configuration or required permission is absent.

## Required Auth0 token shape

The API expects an Auth0 access token with a `permissions` claim containing the permission strings above. Broker-scoped access must be represented by a trusted custom claim such as `broker_id`; the next hardening increment must validate that claim against the requested broker and persisted user scope.

## Verification

The .NET 8.0.425 SDK is installed per-user at `C:\Users\Manivannan.N\.dotnet`. Local verification passed:

- `dotnet build` completed with zero warnings and zero errors.
- `GET /health` returned `200` with an `X-Correlation-ID` response header.
- `GET /api/users` without a token returned `401` when Auth0 configuration is absent.

Configure `Auth0:Domain` and `Auth0:Audience` to exercise valid JWT claims locally. The API intentionally fails closed until those values are supplied.