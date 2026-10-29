# Manual de uso — Inventory API

Guía operativa para arrancar, autenticarse y recorrer el flujo de inventario.
Para el contrato detallado ver [`api.md`](./api.md). Para el marco SDD ver [`.specify/memory/constitution.md`](../.specify/memory/constitution.md) y [`specs/001-inventory-api/`](../specs/001-inventory-api/).

## 1. Requisitos

- Docker y Docker Compose
- (Opcional) .NET 8 SDK para tests o ejecución fuera de Docker
- `curl` y, recomendado, `jq`

## 2. Configuración

Copia de referencia: [`.env.example`](../.env.example).

| Variable | Uso |
|----------|-----|
| `POSTGRES_CONNECTION_STRING` | Cadena Npgsql |
| `OAUTH_CLIENT_ID` / `OAUTH_CLIENT_SECRET` | Client credentials |
| `JWT_SIGNING_KEY` / `JWT_ISSUER` / `JWT_AUDIENCE` | Emisión y validación JWT |
| `JWT_EXPIRES_IN_SECONDS` | Vida del token (default 3600) |
| `RATE_LIMIT_PERMIT_LIMIT` | Cupo política `api` |
| `RATE_LIMIT_WINDOW_SECONDS` | Ventana en segundos |
| `OAUTH_RATE_LIMIT_PERMIT_LIMIT` | Cupo `POST /oauth/token` |

En Docker Compose el host de Postgres es `postgres`. En máquina local usa `localhost`.

Los valores del ejemplo son solo de desarrollo.

## 3. Arranque con Docker

Desde la raíz del repositorio:

```bash
docker compose up --build
```

Servicios:

| Servicio | URL / puerto |
|----------|----------------|
| API | http://localhost:8088 (contenedor 8080) |
| Health | http://localhost:8088/health |
| Swagger | http://localhost:8088/swagger |
| PostgreSQL | localhost:5432 |

El API espera a que Postgres esté sano y aplica el esquema idempotente al iniciar. El script también se monta en `docker-entrypoint-initdb.d`. No crea foreign keys.

Detener:

```bash
docker compose down
```

Datos: volumen `postgres-data`. Para reset total: `docker compose down -v`.

## 4. Arranque local (sin contenedor de API)

1. Levanta solo Postgres o usa el del compose.
2. Copia `.env.example` → `.env` y exporta las variables (`set -a; source .env; set +a`).
3. Ajusta el host de la cadena a `localhost`.
4. Ejecuta:

```bash
dotnet run --project src/Inventory.Api
```

## 5. Obtener un token

```bash
TOKEN=$(curl -s -X POST http://localhost:8088/oauth/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=inventory-api&client_secret=local-dev-only" \
  | jq -r .access_token)
echo "$TOKEN"
```

Usa el token en todas las llamadas `/api/*`:

```bash
-H "Authorization: Bearer $TOKEN"
```

En Swagger, autoriza con el esquema OAuth2 client credentials apuntando a `/oauth/token`.

## 6. Flujo de negocio recomendado

### 6.1 Crear categoría

```bash
CATEGORY=$(curl -s -X POST http://localhost:8088/api/categories \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Beverages","description":"Drinks"}')
CATEGORY_ID=$(echo "$CATEGORY" | jq -r .id)
```

### 6.2 Crear producto

Stock inicial: 0.

```bash
PRODUCT=$(curl -s -X POST http://localhost:8088/api/products \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"name\":\"Water\",\"sku\":\"WATER-1\",\"price\":1.5,\"categoryId\":\"$CATEGORY_ID\"}")
PRODUCT_ID=$(echo "$PRODUCT" | jq -r .id)
```

### 6.3 Registrar entrada

```bash
curl -s -X POST http://localhost:8088/api/inventory-movements \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"productId\":\"$PRODUCT_ID\",\"type\":\"In\",\"quantity\":5,\"notes\":\"Receipt\"}"
```

### 6.4 Registrar salida

```bash
curl -s -X POST http://localhost:8088/api/inventory-movements \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d "{\"productId\":\"$PRODUCT_ID\",\"type\":\"Out\",\"quantity\":2}"
```

### 6.5 Probar stock insuficiente

Una salida mayor que el stock responde **409** y no altera el inventario.

### 6.6 Consultas útiles

```bash
curl -s http://localhost:8088/api/products?categoryId=$CATEGORY_ID \
  -H "Authorization: Bearer $TOKEN"
```

## 7. Reglas que el operador debe conocer

1. Cantidad de movimiento siempre > 0.
2. Tipos HTTP: `In` y `Out`.
3. No borrar categoría con productos ni producto con movimientos (409).
4. SKU único en todo el catálogo.
5. El stock solo cambia por movimientos, no por `PUT` de producto.
6. Tras agotar el cupo, esperar `Retry-After` o ampliar variables de rate limit en desarrollo.

## 8. Pruebas automatizadas

```bash
dotnet test Inventory.sln
```

Cubren reglas de stock, handlers con puertos falsos, esquema sin FK y mapeo de errores.

## 9. Solución de problemas

| Síntoma | Qué revisar |
|---------|-------------|
| Connection refused :8088 | `docker compose ps` y logs del servicio `api` |
| 401 en `/api/*` | Token expirado o ausente; reemitir en `/oauth/token` |
| 401 en token | `OAUTH_CLIENT_ID` / `OAUTH_CLIENT_SECRET` |
| 409 en salida | Stock actual del producto |
| 409 al borrar | Dependencias (productos o movimientos) |
| 429 | Cupo rate limit; bajar frecuencia o subir límites de entorno |
| Esquema vacío | Volumen corrupto o API arrancó antes de Postgres; recrear con `-v` |

## 10. Metodología SDD (Spec Kit)

Orden de lectura para agentes o revisores:

1. Constitución — `.specify/memory/constitution.md`
2. Especificación — `specs/001-inventory-api/spec.md`
3. Clarificaciones — `specs/001-inventory-api/clarifications.md`
4. Plan — `specs/001-inventory-api/plan.md`
5. Tareas — `specs/001-inventory-api/tasks.md`
6. Checklist diario — `PLAN.md`
7. Reglas cortas del agente — `AGENTS.md`
