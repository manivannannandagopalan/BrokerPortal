from datetime import datetime
from uuid import UUID
from pydantic import BaseModel, ConfigDict, EmailStr, Field

class UserCreate(BaseModel):
    email: EmailStr
    broker_id: str
    role: str = Field(pattern="^(broker_admin|operations|viewer)$")
class UserStatusUpdate(BaseModel):
    status: str = Field(pattern="^(active|suspended)$")
    reason: str | None = Field(default=None, max_length=500)
class UserResponse(BaseModel):
    model_config = ConfigDict(from_attributes=True)
    id: UUID; name: str; email: EmailStr; broker_id: str; role: str; status: str; last_sign_in: datetime | None

class SubmissionCreate(BaseModel):
    applicant: str = Field(min_length=2, max_length=200)
    line_of_business: str
    broker_id: str
class SubmissionResponse(BaseModel):
    model_config = ConfigDict(from_attributes=True)
    id: UUID; applicant: str; line_of_business: str; broker_id: str; stage: str; updated_at: datetime

class GuidelineResponse(BaseModel):
    model_config = ConfigDict(from_attributes=True)
    id: UUID; title: str; line: str; summary: str; version: str; status: str; effective_date: datetime
class IntegrationHealth(BaseModel):
    name: str; status: str; latency_ms: int | None = None; checked_at: datetime
class AuditResponse(BaseModel):
    model_config = ConfigDict(from_attributes=True)
    id: UUID; event_type: str; actor: str; broker_scope: str | None; correlation_id: str; outcome: str; occurred_at: datetime
