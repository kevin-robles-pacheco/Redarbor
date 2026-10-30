#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
: "${SONAR_HOST_URL:=http://127.0.0.1:9000}"
: "${SONAR_TOKEN:?Set SONAR_TOKEN to a SonarQube user token}"
mkdir -p coverage
dotnet sonarscanner begin \
  /k:"inventory-api" \
  /n:"Inventory API" \
  /d:sonar.host.url="$SONAR_HOST_URL" \
  /d:sonar.login="$SONAR_TOKEN" \
  /d:sonar.cs.vscoveragexml.reportsPaths="$ROOT/coverage/coverage.xml" \
  /d:sonar.scanner.scanAll=false
dotnet build Inventory.sln -c Release
dotnet-coverage collect -f xml -o "$ROOT/coverage/coverage.xml" \
  "dotnet test Inventory.sln -c Release --no-build --verbosity quiet"
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
echo "Dashboard: $SONAR_HOST_URL/dashboard?id=inventory-api"
