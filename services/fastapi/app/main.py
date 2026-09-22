from datetime import datetime, timezone
from uuid import UUID
from fastapi import Depends, FastAPI, Header, HTTPException, Query, Response, status
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession
from .auth import require_permission
from .config import get_settings
from .database import get_db, init_db
from .duck_creek import DuckCreekGateway
from .models import AuditEvent, Guideline, Submission, User
from .schemas import AuditResponse, GuidelineResponse, IntegrationHealth, SubmissionCreate, SubmissionResponse, UserCreate, UserResponse, UserStatusUpdate

app = FastAPI(title="BrokerPortal API", version="1.0.0", description="Headless broker operations API")

def correlation(value: str | None) -> str:
    return value or UUID(int=0).hex[:12]

@app.on_event("startup")
async def startup() -> None:
    await init_db()

@app.get("/health")
async def health() -> dict:
    return {"status": "healthy", "service": get_settings().app_name, "checked_at": datetime.now(timezone.utc)}

@app.get("/api/users", response_model=list[UserResponse])
async def list_users(search: str | None = None, status_filter: str | None = Query(None, alias="status"), broker_id: str | None = None, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("users.read"))):
    query = select(User)
    if search: query = query.where((User.name.ilike(f"%{search}%")) | (User.email.ilike(f"%{search}%")))
    if status_filter: query = query.where(User.status == status_filter)
    if broker_id: query = query.where(User.broker_id == broker_id)
    return list((await db.execute(query)).scalars().all())

@app.post("/api/users/invitations", response_model=UserResponse, status_code=status.HTTP_202_ACCEPTED)
async def invite_user(payload: UserCreate, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("users.invite"))):
    existing = await db.scalar(select(User).where(User.email == payload.email, User.status == "pending"))
    if existing: raise HTTPException(status.HTTP_409_CONFLICT, "Pending invitation already exists")
    user = User(name=payload.email.split("@")[0], email=payload.email, broker_id=payload.broker_id, role=payload.role, status="pending")
    db.add(user); await db.commit(); await db.refresh(user)
    await DuckCreekGateway().provision_user(user.email, user.broker_id)
    return user

@app.patch("/api/users/{user_id}/status", response_model=UserResponse)
async def update_user_status(user_id: UUID, payload: UserStatusUpdate, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("users.status.write"))):
    user = await db.get(User, user_id)
    if not user: raise HTTPException(status.HTTP_404_NOT_FOUND, "User not found")
    user.status = payload.status; await db.commit(); await db.refresh(user); return user

@app.get("/api/commercial/submissions", response_model=list[SubmissionResponse])
async def list_submissions(stage: str | None = None, broker_id: str | None = None, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("commercial.read"))):
    query = select(Submission)
    if stage: query = query.where(Submission.stage == stage)
    if broker_id: query = query.where(Submission.broker_id == broker_id)
    return list((await db.execute(query.order_by(Submission.updated_at.desc()))).scalars().all())

@app.post("/api/commercial/submissions", response_model=SubmissionResponse, status_code=status.HTTP_201_CREATED)
async def create_submission(payload: SubmissionCreate, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("commercial.write"))):
    submission = Submission(**payload.model_dump()); db.add(submission); await db.commit(); await db.refresh(submission); return submission

@app.get("/api/underwriting/guidelines", response_model=list[GuidelineResponse])
async def list_guidelines(search: str | None = None, line: str | None = None, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("guidelines.read"))):
    query = select(Guideline)
    if search: query = query.where((Guideline.title.ilike(f"%{search}%")) | (Guideline.summary.ilike(f"%{search}%")))
    if line: query = query.where(Guideline.line == line)
    return list((await db.execute(query.order_by(Guideline.effective_date.desc()))).scalars().all())

@app.get("/api/integrations/health", response_model=list[IntegrationHealth])
async def integrations(_: dict = Depends(require_permission("integrations.read"))):
    duck = await DuckCreekGateway().health(); now = datetime.now(timezone.utc)
    return [IntegrationHealth(name=duck["name"], status=duck["status"], latency_ms=duck["latency_ms"], checked_at=now), IntegrationHealth(name="postgresql", status="operational", latency_ms=None, checked_at=now)]

@app.get("/api/audit-events", response_model=list[AuditResponse])
async def audit_events(search: str | None = None, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("audit.read"))):
    query = select(AuditEvent)
    if search: query = query.where((AuditEvent.event_type.ilike(f"%{search}%")) | (AuditEvent.correlation_id.ilike(f"%{search}%")))
    return list((await db.execute(query.order_by(AuditEvent.occurred_at.desc()))).scalars().all())
