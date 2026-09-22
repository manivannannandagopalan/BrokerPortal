from fastapi import Depends, HTTPException, Request, status
from .config import get_settings

def claims(request: Request) -> dict:
    settings = get_settings()
    if settings.environment == "development" and not settings.auth0_domain:
        return {"sub": "local-admin", "permissions": ["users.read", "users.invite", "users.status.write", "commercial.read", "commercial.write", "guidelines.read", "policies.read", "integrations.read", "audit.read"], "broker_id": "*"}
    authorization = request.headers.get("Authorization", "")
    if not authorization.startswith("Bearer "):
        raise HTTPException(status.HTTP_401_UNAUTHORIZED, "Bearer token required")
    raise HTTPException(status.HTTP_501_NOT_IMPLEMENTED, "Auth0 token validation adapter is not configured")

def require_permission(permission: str):
    def dependency(token_claims: dict = Depends(claims)) -> dict:
        if permission not in token_claims.get("permissions", []):
            raise HTTPException(status.HTTP_403_FORBIDDEN, "Missing required permission")
        return token_claims
    return dependency
