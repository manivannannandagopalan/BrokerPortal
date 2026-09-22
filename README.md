# BrokerPortal

A first implementation increment for a broker administration workspace. The current slice is a dependency-free browser shell that can be opened directly from `index.html` while the production Angular/.NET foundation is prepared.

## Run locally

Open `index.html` in a browser. No package installation is required for this prototype.

The machine used to create this repository currently has Git but does not have the .NET SDK or Node.js installed. The planned production stack and setup prerequisites are documented in `docs/architecture.md`.

## First increment

- Responsive operations dashboard
- Network metrics and user activity visualization
- Pending access review queue
- Broker directory with integration status
- Invite-user interaction with validation
- Navigation placeholders for user management, policies, and integrations
- Architecture notes for Angular, ASP.NET Core, SQL Server, Auth0, and Duck Creek

## Repository status

This is a new local Git repository. Add a GitHub remote after creating the remote repository:

```powershell
git remote add origin https://github.com/<owner>/BrokerPortal.git
git push -u origin main
```
