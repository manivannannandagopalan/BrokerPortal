$ErrorActionPreference = 'Stop'
$servicePath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $servicePath

$databaseHost = if ($env:BROKERPORTAL_DATABASE_HOST) { $env:BROKERPORTAL_DATABASE_HOST } else { '127.0.0.1' }
$databasePort = if ($env:BROKERPORTAL_DATABASE_PORT) { [int]$env:BROKERPORTAL_DATABASE_PORT } else { 5432 }
$tcpClient = [System.Net.Sockets.TcpClient]::new()
$connectTask = $tcpClient.ConnectAsync($databaseHost, $databasePort)
$postgresAvailable = $connectTask.Wait(500)
$tcpClient.Dispose()
if ($postgresAvailable) {
    Write-Host "Using PostgreSQL at ${databaseHost}:${databasePort}."
} else {
    Write-Warning "PostgreSQL is unavailable. Using the local SQLite database at $servicePath\brokerportal.local.db."
    $env:BROKERPORTAL_DATABASE_URL = 'sqlite+aiosqlite:///./brokerportal.local.db'
}

& '.\.venv\Scripts\python.exe' -m uvicorn app.main:app --reload --reload-dir app --host 127.0.0.1 --port 5082
