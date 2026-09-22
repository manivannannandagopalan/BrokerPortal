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

The ASP.NET Core project is ready for `dotnet restore` and `dotnet build` in CI. The local machine still lacks the .NET SDK, so compilation remains delegated to the existing GitHub Actions .NET 8 job.