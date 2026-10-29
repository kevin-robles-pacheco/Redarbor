# Quickstart: Inventory API

Guía corta de la feature. Manual completo: [`docs/manual.md`](../../docs/manual.md). Referencia HTTP: [`docs/api.md`](../../docs/api.md).

## Arranque

```bash
docker compose up --build
```

- API: http://localhost:8088
- Health: http://localhost:8088/health
- Swagger: http://localhost:8088/swagger

## Token

```bash
export TOKEN=$(curl -s -X POST http://localhost:8088/oauth/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=inventory-api&client_secret=local-dev-only" \
  | jq -r .access_token)
```

## Flujo mínimo

1. Crear categoría → `POST /api/categories`
2. Crear producto → `POST /api/products` (stock 0)
3. Entrada → `POST /api/inventory-movements` con `"type":"In"`
4. Salida → mismo endpoint con `"type":"Out"`
5. Salida excesiva → 409

## Tests

```bash
dotnet test Inventory.sln
```

## Validación Spec Kit

1. Constitución sin contradicciones con el código.
2. Escenarios P1 de `spec.md` reproducibles con curl.
3. Tareas de `tasks.md` marcadas según el estado real.
