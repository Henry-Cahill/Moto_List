<#
.SYNOPSIS
    Deploy & verify Moto_List to remote Docker host.
.DESCRIPTION
    Syncs source files (no bin/obj), builds Docker images on the remote,
    starts containers, and verifies both services are healthy.
.PARAMETER Action
    deploy  - Full deploy (sync + build + up + verify)  [default]
    verify  - Only check if services are running & responding
    logs    - Tail live container logs
    stop    - Stop all containers
    restart - Restart containers without rebuild
    rebuild - Rebuild images and restart
    status  - Show container status
#>
param(
    [ValidateSet("deploy","verify","logs","stop","restart","rebuild","status")]
    [string]$Action = "deploy"
)

$ErrorActionPreference = "Stop"

# ── Config ──────────────────────────────────────────────────────────
$RemoteUser = "htsadmin"
$RemoteHost = "192.168.1.202"
$RemoteDir  = "~/motolist"
$Remote     = "$RemoteUser@$RemoteHost"
$ApiUrl     = "http://${RemoteHost}:7101"
$WebUrl     = "http://${RemoteHost}:7201"

# ── Helpers ─────────────────────────────────────────────────────────
function Write-Step($n, $total, $msg) {
    Write-Host "[$n/$total] $msg" -ForegroundColor Yellow
}

function Invoke-Remote($cmd) {
    ssh $Remote $cmd
    if ($LASTEXITCODE -ne 0) { throw "Remote command failed: $cmd" }
}

function Sync-SourceFiles {
    Write-Host "  Syncing docker-compose.yml + .dockerignore ..." -ForegroundColor DarkGray

    # Create full directory tree in one shot
    Invoke-Remote "mkdir -p $RemoteDir/src/Moto_List.Shared/DTOs $RemoteDir/src/Moto_List.API/Controllers $RemoteDir/src/Moto_List.API/Data/Migrations $RemoteDir/src/Moto_List.API/Models $RemoteDir/src/Moto_List.API/Services $RemoteDir/src/Moto_List.API/Properties $RemoteDir/src/Moto_List.Web/Pages/Account $RemoteDir/src/Moto_List.Web/Pages/Checklist $RemoteDir/src/Moto_List.Web/Pages/Shared $RemoteDir/src/Moto_List.Web/Properties $RemoteDir/src/Moto_List.Web/Services $RemoteDir/src/Moto_List.Web/wwwroot/css"

    # Root files
    scp -q docker-compose.yml .dockerignore "${Remote}:${RemoteDir}/"

    # Shared
    Write-Host "  Syncing Moto_List.Shared ..." -ForegroundColor DarkGray
    scp -q src/Moto_List.Shared/Moto_List.Shared.csproj "${Remote}:${RemoteDir}/src/Moto_List.Shared/"
    scp -q src/Moto_List.Shared/DTOs/*.cs               "${Remote}:${RemoteDir}/src/Moto_List.Shared/DTOs/"

    # API
    Write-Host "  Syncing Moto_List.API ..." -ForegroundColor DarkGray
    scp -q src/Moto_List.API/Moto_List.API.csproj        "${Remote}:${RemoteDir}/src/Moto_List.API/"
    scp -q src/Moto_List.API/Program.cs                  "${Remote}:${RemoteDir}/src/Moto_List.API/"
    scp -q src/Moto_List.API/Dockerfile                  "${Remote}:${RemoteDir}/src/Moto_List.API/"
    scp -q src/Moto_List.API/appsettings*.json           "${Remote}:${RemoteDir}/src/Moto_List.API/"
    scp -q src/Moto_List.API/Properties/*.json           "${Remote}:${RemoteDir}/src/Moto_List.API/Properties/"
    scp -q src/Moto_List.API/Controllers/*.cs            "${Remote}:${RemoteDir}/src/Moto_List.API/Controllers/"
    scp -q src/Moto_List.API/Data/*.cs                   "${Remote}:${RemoteDir}/src/Moto_List.API/Data/"
    scp -q src/Moto_List.API/Data/Migrations/*.cs        "${Remote}:${RemoteDir}/src/Moto_List.API/Data/Migrations/"
    scp -q src/Moto_List.API/Models/*.cs                 "${Remote}:${RemoteDir}/src/Moto_List.API/Models/"
    scp -q src/Moto_List.API/Services/*.cs               "${Remote}:${RemoteDir}/src/Moto_List.API/Services/"

    # Web
    Write-Host "  Syncing Moto_List.Web ..." -ForegroundColor DarkGray
    scp -q src/Moto_List.Web/Moto_List.Web.csproj        "${Remote}:${RemoteDir}/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/Program.cs                  "${Remote}:${RemoteDir}/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/Dockerfile                  "${Remote}:${RemoteDir}/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/appsettings*.json           "${Remote}:${RemoteDir}/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/Properties/*.json           "${Remote}:${RemoteDir}/src/Moto_List.Web/Properties/"
    scp -q src/Moto_List.Web/Services/*.cs               "${Remote}:${RemoteDir}/src/Moto_List.Web/Services/"
    scp -q src/Moto_List.Web/wwwroot/css/site.css        "${Remote}:${RemoteDir}/src/Moto_List.Web/wwwroot/css/"
    scp -q src/Moto_List.Web/Pages/_ViewImports.cshtml   "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/_ViewStart.cshtml     "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Index.cshtml          "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Index.cshtml.cs       "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Error.cshtml          "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Error.cshtml.cs       "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Shared/_Layout.cshtml "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/Shared/"
    scp -q src/Moto_List.Web/Pages/Account/*.cshtml      "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/Account/"
    scp -q src/Moto_List.Web/Pages/Account/*.cs          "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/Account/"
    scp -q src/Moto_List.Web/Pages/Checklist/*.cshtml    "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/Checklist/"
    scp -q src/Moto_List.Web/Pages/Checklist/*.cs        "${Remote}:${RemoteDir}/src/Moto_List.Web/Pages/Checklist/"
}

function Test-Services {
    $allGood = $true

    # Check containers are running
    Write-Host ""
    Write-Host "  Container status:" -ForegroundColor DarkGray
    Invoke-Remote "cd $RemoteDir && docker compose ps --format 'table {{.Name}}\t{{.Status}}\t{{.Ports}}'"

    # Wait for containers to start up
    Start-Sleep -Seconds 8

    # Test API health — retry up to 3 times
    Write-Host ""
    Write-Host "  Testing API ($ApiUrl) ..." -ForegroundColor DarkGray
    $apiOk = $false
    for ($i = 1; $i -le 3; $i++) {
        try {
            $apiResult = Invoke-Remote "curl -s -o /dev/null -w '%{http_code}' --max-time 10 $ApiUrl/api/categories 2>/dev/null || echo 'FAIL'"
            if ($apiResult -match "401|200") {
                Write-Host "    API responding (HTTP $apiResult)" -ForegroundColor Green
                $apiOk = $true
                break
            }
        } catch {}
        if ($i -lt 3) { Write-Host "    Retrying in 5s... ($i/3)" -ForegroundColor DarkGray; Start-Sleep -Seconds 5 }
    }
    if (-not $apiOk) {
        Write-Host "    API not responding after 3 attempts" -ForegroundColor Red
        $allGood = $false
    }

    # Test Web health — retry up to 3 times
    Write-Host "  Testing Web ($WebUrl) ..." -ForegroundColor DarkGray
    $webOk = $false
    for ($i = 1; $i -le 3; $i++) {
        try {
            $webResult = Invoke-Remote "curl -s -o /dev/null -w '%{http_code}' --max-time 10 $WebUrl 2>/dev/null || echo 'FAIL'"
            if ($webResult -match "200|302") {
                Write-Host "    Web responding (HTTP $webResult)" -ForegroundColor Green
                $webOk = $true
                break
            }
        } catch {}
        if ($i -lt 3) { Write-Host "    Retrying in 5s... ($i/3)" -ForegroundColor DarkGray; Start-Sleep -Seconds 5 }
    }
    if (-not $webOk) {
        Write-Host "    Web not responding after 3 attempts" -ForegroundColor Red
        $allGood = $false
    }

    # Check container logs for errors
    Write-Host ""
    Write-Host "  Recent errors (last 5 lines per container):" -ForegroundColor DarkGray
    $apiLogs = Invoke-Remote "cd $RemoteDir && docker compose logs api --tail 5 2>&1 | grep -i 'error\|exception\|fail' || echo '    (none)'"
    Write-Host "    API: $apiLogs"
    $webLogs = Invoke-Remote "cd $RemoteDir && docker compose logs web --tail 5 2>&1 | grep -i 'error\|exception\|fail' || echo '    (none)'"
    Write-Host "    Web: $webLogs"

    return $allGood
}

# ── Actions ─────────────────────────────────────────────────────────

switch ($Action) {
    "deploy" {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        Write-Host "=== Deploying Moto_List to $RemoteHost ===" -ForegroundColor Cyan
        Write-Host ""

        Write-Step 1 4 "Syncing source files..."
        Sync-SourceFiles

        Write-Step 2 4 "Building Docker images..."
        Invoke-Remote "cd $RemoteDir && docker compose build"

        Write-Step 3 4 "Starting containers..."
        Invoke-Remote "cd $RemoteDir && docker compose up -d"

        Write-Step 4 4 "Verifying services..."
        $healthy = Test-Services

        $sw.Stop()
        Write-Host ""
        if ($healthy) {
            Write-Host "=== Deploy SUCCESS ($([math]::Round($sw.Elapsed.TotalSeconds))s) ===" -ForegroundColor Green
        } else {
            Write-Host "=== Deploy DONE with WARNINGS ($([math]::Round($sw.Elapsed.TotalSeconds))s) ===" -ForegroundColor Yellow
            Write-Host "    Run: .\deploy.ps1 logs" -ForegroundColor Yellow
        }
        Write-Host "  API: $ApiUrl  (Swagger: $ApiUrl/swagger)" -ForegroundColor White
        Write-Host "  Web: $WebUrl" -ForegroundColor White
    }

    "verify" {
        Write-Host "=== Verifying Moto_List on $RemoteHost ===" -ForegroundColor Cyan
        $healthy = Test-Services
        Write-Host ""
        if ($healthy) {
            Write-Host "=== All services healthy ===" -ForegroundColor Green
        } else {
            Write-Host "=== Issues detected ===" -ForegroundColor Red
        }
    }

    "logs" {
        Write-Host "=== Tailing logs (Ctrl+C to stop) ===" -ForegroundColor Cyan
        ssh $Remote "cd $RemoteDir && docker compose logs -f --tail 50"
    }

    "stop" {
        Write-Host "=== Stopping containers ===" -ForegroundColor Cyan
        Invoke-Remote "cd $RemoteDir && docker compose down"
        Write-Host "Stopped." -ForegroundColor Green
    }

    "restart" {
        Write-Host "=== Restarting containers ===" -ForegroundColor Cyan
        Invoke-Remote "cd $RemoteDir && docker compose restart"
        Test-Services | Out-Null
        Write-Host "Restarted." -ForegroundColor Green
    }

    "rebuild" {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        Write-Host "=== Rebuilding & restarting on $RemoteHost ===" -ForegroundColor Cyan
        Invoke-Remote "cd $RemoteDir && docker compose down && docker compose build --no-cache && docker compose up -d"
        Start-Sleep -Seconds 5
        Test-Services | Out-Null
        $sw.Stop()
        Write-Host "Rebuilt in $([math]::Round($sw.Elapsed.TotalSeconds))s." -ForegroundColor Green
    }

    "status" {
        Write-Host "=== Container Status ===" -ForegroundColor Cyan
        Invoke-Remote "cd $RemoteDir && docker compose ps"
    }
}
