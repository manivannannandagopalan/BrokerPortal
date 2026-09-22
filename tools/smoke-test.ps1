$ErrorActionPreference = 'Stop'
$base = 'http://127.0.0.1:5080'
$health = Invoke-WebRequest "$base/health" -UseBasicParsing
if ($health.StatusCode -ne 200) { throw 'Health check failed' }
$correlation = $health.Headers['X-Correlation-ID']
if ([string]::IsNullOrWhiteSpace($correlation)) { throw 'Correlation ID missing' }
$users = Invoke-RestMethod "$base/api/users?status=active"
if (@($users).Count -lt 1) { throw 'Active user query failed' }
$payload = @{ email = 'phase-six@example.com'; brokerId = 'northstar'; role = 'viewer' } | ConvertTo-Json
$invite = Invoke-WebRequest "$base/api/users/invitations" -Method Post -Body $payload -ContentType 'application/json' -UseBasicParsing
if ($invite.StatusCode -ne 202) { throw 'Invitation failed' }
Write-Output 'Mock API smoke tests passed.'
