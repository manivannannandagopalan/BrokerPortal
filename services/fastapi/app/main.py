from datetime import datetime, timezone
from uuid import UUID
from fastapi import Depends, FastAPI, File, Header, HTTPException, Query, Response, UploadFile, status
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy import select
from sqlalchemy.ext.asyncio import AsyncSession
from .auth import require_permission
from .config import get_settings
from .database import get_db, init_db
from .duck_creek import DuckCreekGateway
from .models import AuditEvent, DCTUserAdmin, Guideline, Submission, User
from .dct_schemas import DCTImportResponse, DCTUserAdminResponse
from .dct_useradmin import ALL_COLUMNS, _validate, read_rows
from .schemas import AuditResponse, GuidelineResponse, IntegrationHealth, SubmissionCreate, SubmissionResponse, UserCreate, UserResponse, UserStatusUpdate

app = FastAPI(title="BrokerPortal API", version="1.0.0", description="Headless broker operations API")
app.add_middleware(CORSMiddleware, allow_origins=["http://127.0.0.1:4175", "http://localhost:4175", "http://127.0.0.1:4174", "http://localhost:4174", "http://127.0.0.1:4173", "http://localhost:4173"], allow_credentials=True, allow_methods=["*"], allow_headers=["*"])

def correlation(value: str | None) -> str:
    return value or UUID(int=0).hex[:12]

@app.on_event("startup")
async def startup() -> None:
    await init_db()
    if get_settings().environment == "development":
        async for db in get_db():
            if not await db.scalar(select(AuditEvent.id).limit(1)):
                db.add_all([
                    AuditEvent(event_type="User invitation created", actor="local-admin", broker_scope="Northstar Financial", correlation_id="local-seed-001", outcome="success"),
                    AuditEvent(event_type="DCTUserAdmin bulk import", actor="local-admin", broker_scope="All brokers", correlation_id="local-seed-002", outcome="success"),
                    AuditEvent(event_type="Duck Creek health check", actor="System", broker_scope="All brokers", correlation_id="local-seed-003", outcome="success"),
                ])
                await db.commit()
            break

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

@app.get("/api/policies")
async def policies(_: dict = Depends(require_permission("policies.read"))):
    return [
        {"name": "broker_admin", "permissions": ["users.read", "users.invite", "users.status.write", "policies.read", "integrations.read", "audit.read"], "status": "published"},
        {"name": "operations", "permissions": ["users.read", "users.invite", "integrations.read"], "status": "published"},
        {"name": "viewer", "permissions": ["users.read"], "status": "published"},
    ]

@app.get("/api/integrations/health", response_model=list[IntegrationHealth])
async def integrations(_: dict = Depends(require_permission("integrations.read"))):
    duck = await DuckCreekGateway().health(); now = datetime.now(timezone.utc)
    return [IntegrationHealth(name=duck["name"], status=duck["status"], latency_ms=duck["latency_ms"], checked_at=now), IntegrationHealth(name="postgresql", status="operational", latency_ms=None, checked_at=now)]

@app.get("/api/audit-events", response_model=list[AuditResponse])
async def audit_events(search: str | None = None, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("audit.read"))):
    query = select(AuditEvent)
    if search: query = query.where((AuditEvent.event_type.ilike(f"%{search}%")) | (AuditEvent.correlation_id.ilike(f"%{search}%")))
    return list((await db.execute(query.order_by(AuditEvent.occurred_at.desc()))).scalars().all())

@app.get("/api/dct-useradmin", response_model=list[DCTUserAdminResponse])
async def list_dct_useradmin(search: str | None = None, db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("users.read"))):
    query = select(DCTUserAdmin).order_by(DCTUserAdmin.name)
    if search: query = query.where(DCTUserAdmin.name.ilike(f"%{search}%"))
    return list((await db.execute(query)).scalars().all())

@app.post("/api/dct-useradmin/import", response_model=DCTImportResponse)
async def import_dct_useradmin(file: UploadFile = File(...), db: AsyncSession = Depends(get_db), _: dict = Depends(require_permission("users.invite"))):
    if not file.filename: raise HTTPException(status.HTTP_400_BAD_REQUEST, "A CSV, XLS, or XLSX file is required")
    try:
        rows, columns = read_rows(file.filename, await file.read())
    except (ValueError, ImportError) as error:
        raise HTTPException(status.HTTP_400_BAD_REQUEST, str(error)) from error
    missing = [column for column in ALL_COLUMNS if column in ("id", "inttype", "name") and column not in columns]
    if missing: return DCTImportResponse(imported=0, updated=0, skipped=0, errors=[{"row": 1, "message": f"Missing required columns: {', '.join(missing)}"}], columns=columns)
    imported = updated = skipped = 0
    errors = []
    for row_number, row in enumerate(rows, start=2):
        row = {column: row.get(column, "") for column in ALL_COLUMNS}
        row_errors = _validate(row, row_number)
        if row_errors: errors.extend(row_errors); skipped += 1; continue
        values = {column: (int(row[column]) if column in ("id", "intparentid", "inttype") and row[column] else row[column] or None) for column in ALL_COLUMNS}
        entity = await db.get(DCTUserAdmin, values["id"])
        if entity is None: db.add(DCTUserAdmin(**values)); imported += 1
        else:
            for column, value in values.items(): setattr(entity, column, value)
            updated += 1
    await db.commit()
    return DCTImportResponse(imported=imported, updated=updated, skipped=skipped, errors=errors, columns=columns)
