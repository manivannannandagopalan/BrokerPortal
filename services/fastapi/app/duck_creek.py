from dataclasses import dataclass
from .config import get_settings

@dataclass
class DuckCreekGateway:
    async def provision_user(self, email: str, broker_id: str) -> None:
        if get_settings().duck_creek_mode == "mock":
            return
        raise NotImplementedError("Configure the Duck Creek adapter before production use")

    async def health(self) -> dict:
        return {"name": "duck_creek", "status": "operational" if get_settings().duck_creek_mode == "mock" else "unconfigured", "latency_ms": 0}
