#!/bin/bash
# Deploy & verify Moto_List to remote Docker host
# Usage: ./deploy.sh [deploy|verify|logs|stop|restart|rebuild|status]
set -e

REMOTE_USER="htsadmin"
REMOTE_HOST="192.168.1.202"
REMOTE_DIR="~/motolist"
REMOTE="$REMOTE_USER@$REMOTE_HOST"
API_URL="http://$REMOTE_HOST:7101"
WEB_URL="http://$REMOTE_HOST:7201"
ACTION="${1:-deploy}"

step() { echo -e "\033[33m[$1/$2] $3\033[0m"; }
ok()   { echo -e "\033[32m  $1\033[0m"; }
err()  { echo -e "\033[31m  $1\033[0m"; }
info() { echo -e "\033[90m  $1\033[0m"; }

sync_files() {
    info "Creating remote directories..."
    ssh "$REMOTE" "mkdir -p $REMOTE_DIR/src/Moto_List.Shared/DTOs \
        $REMOTE_DIR/src/Moto_List.API/{Controllers,Data/Migrations,Models,Services,Properties} \
        $REMOTE_DIR/src/Moto_List.Web/{Pages/{Account,Checklist,Shared},Properties,Services,wwwroot/css}"

    info "Syncing root files..."
    scp -q docker-compose.yml .dockerignore "$REMOTE:$REMOTE_DIR/"

    info "Syncing Moto_List.Shared..."
    scp -q src/Moto_List.Shared/Moto_List.Shared.csproj "$REMOTE:$REMOTE_DIR/src/Moto_List.Shared/"
    scp -q src/Moto_List.Shared/DTOs/*.cs               "$REMOTE:$REMOTE_DIR/src/Moto_List.Shared/DTOs/"

    info "Syncing Moto_List.API..."
    scp -q src/Moto_List.API/Moto_List.API.csproj        "$REMOTE:$REMOTE_DIR/src/Moto_List.API/"
    scp -q src/Moto_List.API/Program.cs                  "$REMOTE:$REMOTE_DIR/src/Moto_List.API/"
    scp -q src/Moto_List.API/Dockerfile                  "$REMOTE:$REMOTE_DIR/src/Moto_List.API/"
    scp -q src/Moto_List.API/appsettings*.json           "$REMOTE:$REMOTE_DIR/src/Moto_List.API/"
    scp -q src/Moto_List.API/Properties/*.json           "$REMOTE:$REMOTE_DIR/src/Moto_List.API/Properties/"
    scp -q src/Moto_List.API/Controllers/*.cs            "$REMOTE:$REMOTE_DIR/src/Moto_List.API/Controllers/"
    scp -q src/Moto_List.API/Data/*.cs                   "$REMOTE:$REMOTE_DIR/src/Moto_List.API/Data/"
    scp -q src/Moto_List.API/Data/Migrations/*.cs        "$REMOTE:$REMOTE_DIR/src/Moto_List.API/Data/Migrations/"
    scp -q src/Moto_List.API/Models/*.cs                 "$REMOTE:$REMOTE_DIR/src/Moto_List.API/Models/"
    scp -q src/Moto_List.API/Services/*.cs               "$REMOTE:$REMOTE_DIR/src/Moto_List.API/Services/"

    info "Syncing Moto_List.Web..."
    scp -q src/Moto_List.Web/Moto_List.Web.csproj        "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/Program.cs                  "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/Dockerfile                  "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/appsettings*.json           "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/"
    scp -q src/Moto_List.Web/Properties/*.json           "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Properties/"
    scp -q src/Moto_List.Web/Services/*.cs               "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Services/"
    scp -q src/Moto_List.Web/wwwroot/css/site.css        "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/wwwroot/css/"
    scp -q src/Moto_List.Web/Pages/_ViewImports.cshtml   "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/_ViewStart.cshtml     "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Index.cshtml          "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Index.cshtml.cs       "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Error.cshtml          "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Error.cshtml.cs       "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/"
    scp -q src/Moto_List.Web/Pages/Shared/_Layout.cshtml "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/Shared/"
    scp -q src/Moto_List.Web/Pages/Account/*.cshtml      "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/Account/"
    scp -q src/Moto_List.Web/Pages/Account/*.cs          "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/Account/"
    scp -q src/Moto_List.Web/Pages/Checklist/*.cshtml    "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/Checklist/"
    scp -q src/Moto_List.Web/Pages/Checklist/*.cs        "$REMOTE:$REMOTE_DIR/src/Moto_List.Web/Pages/Checklist/"
}

verify_services() {
    echo ""
    info "Container status:"
    ssh "$REMOTE" "cd $REMOTE_DIR && docker compose ps"

    sleep 3

    echo ""
    info "Testing API ($API_URL)..."
    API_CODE=$(ssh "$REMOTE" "curl -s -o /dev/null -w '%{http_code}' --max-time 10 $API_URL/api/categories 2>/dev/null || echo 'FAIL'")
    if [[ "$API_CODE" =~ ^(200|401)$ ]]; then
        ok "API responding (HTTP $API_CODE)"
    else
        err "API returned: $API_CODE"
    fi

    info "Testing Web ($WEB_URL)..."
    WEB_CODE=$(ssh "$REMOTE" "curl -s -o /dev/null -w '%{http_code}' --max-time 10 $WEB_URL 2>/dev/null || echo 'FAIL'")
    if [[ "$WEB_CODE" =~ ^(200|302)$ ]]; then
        ok "Web responding (HTTP $WEB_CODE)"
    else
        err "Web returned: $WEB_CODE"
    fi

    echo ""
    info "Recent errors (last 5 lines per container):"
    echo -n "  API: "; ssh "$REMOTE" "cd $REMOTE_DIR && docker compose logs api --tail 5 2>&1 | grep -i 'error\|exception\|fail' || echo '(none)'"
    echo -n "  Web: "; ssh "$REMOTE" "cd $REMOTE_DIR && docker compose logs web --tail 5 2>&1 | grep -i 'error\|exception\|fail' || echo '(none)'"
}

case "$ACTION" in
    deploy)
        START=$(date +%s)
        echo -e "\033[36m=== Deploying Moto_List to $REMOTE_HOST ===\033[0m"
        echo ""
        step 1 4 "Syncing source files..."
        sync_files
        step 2 4 "Building Docker images..."
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose build"
        step 3 4 "Starting containers..."
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose up -d"
        step 4 4 "Verifying services..."
        verify_services
        END=$(date +%s)
        echo ""
        echo -e "\033[32m=== Deploy complete ($((END-START))s) ===\033[0m"
        echo "  API: $API_URL  (Swagger: $API_URL/swagger)"
        echo "  Web: $WEB_URL"
        ;;
    verify)
        echo -e "\033[36m=== Verifying Moto_List on $REMOTE_HOST ===\033[0m"
        verify_services
        ;;
    logs)
        echo -e "\033[36m=== Tailing logs (Ctrl+C to stop) ===\033[0m"
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose logs -f --tail 50"
        ;;
    stop)
        echo -e "\033[36m=== Stopping containers ===\033[0m"
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose down"
        ok "Stopped."
        ;;
    restart)
        echo -e "\033[36m=== Restarting containers ===\033[0m"
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose restart"
        verify_services
        ;;
    rebuild)
        echo -e "\033[36m=== Rebuilding on $REMOTE_HOST ===\033[0m"
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose down && docker compose build --no-cache && docker compose up -d"
        sleep 5
        verify_services
        ;;
    status)
        echo -e "\033[36m=== Container Status ===\033[0m"
        ssh "$REMOTE" "cd $REMOTE_DIR && docker compose ps"
        ;;
    *)
        echo "Usage: $0 [deploy|verify|logs|stop|restart|rebuild|status]"
        exit 1
        ;;
esac
