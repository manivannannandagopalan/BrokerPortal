from datetime import datetime, timezone
from uuid import UUID, uuid4
from sqlalchemy import DateTime, String, Text, Uuid
from sqlalchemy.orm import Mapped, mapped_column
from .database import Base

def utc_now() -> datetime:
    return datetime.now(timezone.utc)

class User(Base):
    __tablename__ = "users"
    id: Mapped[UUID] = mapped_column(Uuid, primary_key=True, default=uuid4)
    name: Mapped[str] = mapped_column(String(200))
    email: Mapped[str] = mapped_column(String(320), index=True)
    broker_id: Mapped[str] = mapped_column(String(100), index=True)
    role: Mapped[str] = mapped_column(String(40))
    status: Mapped[str] = mapped_column(String(20), default="pending")
    last_sign_in: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))

class Submission(Base):
    __tablename__ = "commercial_submissions"
    id: Mapped[UUID] = mapped_column(Uuid, primary_key=True, default=uuid4)
    applicant: Mapped[str] = mapped_column(String(200))
    line_of_business: Mapped[str] = mapped_column(String(100))
    broker_id: Mapped[str] = mapped_column(String(100), index=True)
    stage: Mapped[str] = mapped_column(String(30), default="review")
    updated_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), default=utc_now)

class Guideline(Base):
    __tablename__ = "underwriting_guidelines"
    id: Mapped[UUID] = mapped_column(Uuid, primary_key=True, default=uuid4)
    title: Mapped[str] = mapped_column(String(200))
    line: Mapped[str] = mapped_column(String(80), index=True)
    summary: Mapped[str] = mapped_column(Text)
    version: Mapped[str] = mapped_column(String(20))
    status: Mapped[str] = mapped_column(String(30), default="current")
    effective_date: Mapped[datetime] = mapped_column(DateTime(timezone=True), default=utc_now)

class Policy(Base):
    __tablename__ = "authorization_policies"
    id: Mapped[UUID] = mapped_column(Uuid, primary_key=True, default=uuid4)
    name: Mapped[str] = mapped_column(String(120), unique=True)
    permissions: Mapped[str] = mapped_column(Text)
    status: Mapped[str] = mapped_column(String(20), default="published")

class AuditEvent(Base):
    __tablename__ = "audit_events"
    id: Mapped[UUID] = mapped_column(Uuid, primary_key=True, default=uuid4)
    event_type: Mapped[str] = mapped_column(String(120), index=True)
    actor: Mapped[str] = mapped_column(String(200))
    broker_scope: Mapped[str | None] = mapped_column(String(100))
    correlation_id: Mapped[str] = mapped_column(String(80), index=True)
    outcome: Mapped[str] = mapped_column(String(20))
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), default=utc_now)
