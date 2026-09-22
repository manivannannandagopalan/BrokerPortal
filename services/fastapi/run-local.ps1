$ErrorActionPreference = 'Stop'
$servicePath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $servicePath

$databaseHost = if ($env:BROKERPORTAL_DATABASE_HOST) { $env:BROKERPORTAL_DATABASE_HOST } else { '127.0.0.1' }
$databasePort = if ($env:BROKERPORTAL_DATABASE_PORT) { [int]$env:BROKERPORTAL_DATABASE_PORT } else { 5432 }
$databaseCheck = Test-NetConnection $databaseHost -Port $databasePort -WarningAction SilentlyContinue
if (-not $databaseCheck.TcpTestSucceeded) {
    Write-Error "PostgreSQL is not reachable at ${databaseHost}:${databasePort}. Start PostgreSQL or Docker before launching FastAPI."
    exit 1
}

& '.\.venv\Scripts\python.exe' -m uvicorn app.main:app --reload --reload-dir app --host 127.0.0.1 --port 5082
