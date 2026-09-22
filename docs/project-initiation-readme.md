# Broker Portal

Secure, modular broker portal for commercial insurance workflows.

## Repository Status

This repository is an empty scaffold being initialized from the Broker Portal master delivery prompt. The first increment is documentation-first because the local machine currently has no .NET SDK, Node.js/npm, Angular CLI, or Git executable available.

## Delivery Principles

- Treat Duck Creek interfaces, Auth0 tenant settings, environments, data contracts, and non-functional targets as unconfirmed until validated.
- Keep authentication in Auth0 and enforce authorization in the API using deny-by-default policies.
- Prefer a modular monolith initially, with explicit integration ports and adapters for future independent deployment.
- Keep secrets out of source control and use placeholders for external configuration.
- Add tests and operational evidence with each implementation increment.

## Planned Repository Areas

```text
/docs
/src
/tests
/deployment
/scripts
```

See [docs/architecture/project-initiation.md](docs/architecture/project-initiation.md) for the current initiation package and the proposed first implementation increment.

## Local Prerequisites

The following are required before code generation and validation can begin:

- Supported .NET SDK
- Supported Node.js and npm
- Organization-approved Angular CLI/version
- SQL Server or an approved local development substitute
- Auth0 development tenant details
- Confirmed Duck Creek User Administration interface
- Git client

No application code, credentials, or external integration claims are included yet.