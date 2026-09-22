from pydantic import BaseModel

class DCTImportResponse(BaseModel):
    imported: int
    updated: int
    skipped: int
    errors: list[dict]
    columns: list[str]

class DCTUserAdminResponse(BaseModel):
    id: int
    intparentid: int | None
    inttype: int
    name: str
    contact: str | None
    phone: str | None
    phoneext: str | None
    address1: str | None
    address2: str | None
    city: str | None
    state: str | None
    zip: str | None
    reference: str | None
