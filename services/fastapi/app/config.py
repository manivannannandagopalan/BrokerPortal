from functools import lru_cache
from pydantic_settings import BaseSettings, SettingsConfigDict

class Settings(BaseSettings):
    app_name: str = "BrokerPortal API"
    environment: str = "development"
    database_url: str = "sqlite+aiosqlite:///./brokerportal.local.db"
    auth0_domain: str = ""
    auth0_audience: str = ""
    duck_creek_base_url: str = ""
    duck_creek_mode: str = "mock"
    model_config = SettingsConfigDict(env_file=".env", env_prefix="BROKERPORTAL_", extra="ignore")

@lru_cache
def get_settings() -> Settings:
    return Settings()
