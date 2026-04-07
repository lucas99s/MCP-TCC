#!/bin/sh
set -eu

echo "Subindo backend em http://0.0.0.0:5080"
ASPNETCORE_URLS=http://0.0.0.0:5080 dotnet /app/backend/ExperimentAnalytics.Api.dll &
BACKEND_PID=$!

cleanup() {
  echo "Encerrando processos..."
  kill "$BACKEND_PID" 2>/dev/null || true
}

trap cleanup INT TERM

echo "Aguardando backend ficar disponível..."
TRIES=0
until node -e "fetch('http://127.0.0.1:5080/health').then(r => process.exit(r.ok ? 0 : 1)).catch(() => process.exit(1))"
do
  TRIES=$((TRIES + 1))
  if [ "$TRIES" -ge 30 ]; then
    echo "Backend não respondeu em http://127.0.0.1:5080/health"
    exit 1
  fi
  sleep 1
done

echo "Subindo frontend em http://0.0.0.0:3000"
exec node /app/frontend/server.js
