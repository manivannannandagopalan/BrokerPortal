param([int]$Port = 5080)

$users = [System.Collections.Generic.List[object]]::new()
$users.Add([pscustomobject]@{ id = '2cdbf5a8-7c1e-4a14-8d0e-000000000001'; name = 'Jordan Rivers'; email = 'jordan.rivers@northstar.com'; brokerId = 'northstar'; role = 'broker_admin'; status = 'active'; lastSignIn = '2026-09-22T09:42:00Z' })
$users.Add([pscustomobject]@{ id = '2cdbf5a8-7c1e-4a14-8d0e-000000000002'; name = 'Marcus Chen'; email = 'marcus.chen@bluerock.com'; brokerId = 'bluerock'; role = 'broker_admin'; status = 'pending'; lastSignIn = $null })

function Send-Json($context, $status, $body, $correlationId) {
    $bytes = [Text.Encoding]::UTF8.GetBytes(($body | ConvertTo-Json -Depth 5))
    $context.Response.StatusCode = $status
    $context.Response.ContentType = 'application/json'
    $context.Response.Headers.Add('X-Correlation-ID', $correlationId)
    $context.Response.ContentLength64 = $bytes.Length
    $context.Response.OutputStream.Write($bytes, 0, $bytes.Length)
    $context.Response.Close()
}

$listener = [Net.HttpListener]::new()
$listener.Prefixes.Add("http://127.0.0.1:$Port/")
$listener.Start()
Write-Host "BrokerPortal mock API listening at http://127.0.0.1:$Port"
while ($listener.IsListening) {
    $context = $listener.GetContext()
    $correlationId = $context.Request.Headers['X-Correlation-ID']
    if ([string]::IsNullOrWhiteSpace($correlationId)) { $correlationId = [guid]::NewGuid().ToString('N') }
    $path = $context.Request.Url.AbsolutePath
    if ($context.Request.HttpMethod -eq 'GET' -and $path -eq '/health') {
        Send-Json $context 200 ([pscustomobject]@{ status = 'healthy'; service = 'identity-api'; checkedAt = [DateTime]::UtcNow }) $correlationId
        continue
    }
    if ($context.Request.HttpMethod -eq 'GET' -and $path -eq '/api/users') {
        $search = $context.Request.QueryString['search']
        $status = $context.Request.QueryString['status']
        $result = @($users | Where-Object { (!$search -or "$($_.name) $($_.email)" -like "*$search*") -and (!$status -or $_.status -eq $status) })
        Send-Json $context 200 $result $correlationId
        continue
    }
    if ($context.Request.HttpMethod -eq 'POST' -and $path -eq '/api/users/invitations') {
        $reader = [IO.StreamReader]::new($context.Request.InputStream)
        $request = $reader.ReadToEnd() | ConvertFrom-Json
        $existing = $users | Where-Object { $_.email -eq $request.email -and $_.status -eq 'pending' }
        if ($existing) { Send-Json $context 409 ([pscustomobject]@{ error = 'pending_invitation_exists' }) $correlationId; continue }
        $newUser = [pscustomobject]@{ id = [guid]::NewGuid().ToString(); name = ($request.email -split '@')[0]; email = $request.email; brokerId = $request.brokerId; role = $request.role; status = 'pending'; lastSignIn = $null }
        $users.Add($newUser)
        Send-Json $context 202 $newUser $correlationId
        continue
    }
    Send-Json $context 404 ([pscustomobject]@{ error = 'not_found' }) $correlationId
}
