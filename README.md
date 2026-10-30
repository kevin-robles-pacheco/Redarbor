# Inventory API

API de inventario de productos: categorías, productos y movimientos de entrada y salida. .NET 8, PostgreSQL, CQRS con MediatR, Entity Framework Core en lecturas y Dapper en escrituras.

Desarrollado con Spec-Driven Development ([GitHub Spec Kit](https://github.com/github/spec-kit)): constitución, especificación, clarificación, plan y tareas.

| Documento | Ruta |
|-----------|------|
| Constitución | [`.specify/memory/constitution.md`](.specify/memory/constitution.md) |
| Especificación | [`specs/001-inventory-api/spec.md`](specs/001-inventory-api/spec.md) |
| Clarificaciones | [`specs/001-inventory-api/clarifications.md`](specs/001-inventory-api/clarifications.md) |
| Plan | [`specs/001-inventory-api/plan.md`](specs/001-inventory-api/plan.md) |
| Tareas | [`specs/001-inventory-api/tasks.md`](specs/001-inventory-api/tasks.md) |
| Documentación del API | [`docs/api.md`](docs/api.md) |
| Manual de uso | [`docs/manual.md`](docs/manual.md) |
| Informe SonarQube | [`docs/sonar-report.md`](docs/sonar-report.md) |
| Plan remediación Sonar | [`docs/sonar-remediation-plan.md`](docs/sonar-remediation-plan.md) |
| Verificación / converge | [`specs/001-inventory-api/verification.md`](specs/001-inventory-api/verification.md) |
| Checklist del agente | [`PLAN.md`](PLAN.md) |

## Requisitos

- Docker y Docker Compose
- .NET 8 SDK, solo si vas a ejecutar los tests fuera de Docker

## Arranque

Desde la raíz del repositorio:

```bash
docker compose up --build
```

Eso levanta PostgreSQL 16 y la API. La API espera a que Postgres acepte conexiones y aplica el esquema al arrancar. El script también se monta en el contenedor de la base, y es idempotente: no crea foreign keys.

- API: `http://localhost:8088`
- Salud: `http://localhost:8088/health`
- Swagger: `http://localhost:8088/swagger`

El contenedor escucha en el puerto 8080. En la máquina se publica como 8088, para convivir con otros servicios locales que ya ocupan el 8080.

Para depurar en local sin Docker, copia `.env.example` a `.env`, exporta esas variables y cambia el host de la cadena a `localhost`. Luego:

```bash
dotnet run --project src/Inventory.Api
```

La cadena del compose usa el host `postgres`, que es el nombre del servicio dentro de la red de Docker. En la máquina local el host es `localhost`.

Las variables las lee el proceso desde el entorno. Los valores de `.env.example` y del compose son de desarrollo, no secretos de producción.

## Token

`POST /oauth/token` es público y usa `application/x-www-form-urlencoded`.

```bash
curl -s -X POST http://localhost:8088/oauth/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials&client_id=inventory-api&client_secret=local-dev-only"
```

La respuesta trae `access_token`. El resto de endpoints exige `Authorization: Bearer <token>`.

En Swagger, el esquema OAuth2 client credentials apunta a `/oauth/token`.

## Ejemplos

Crea una categoría:

```bash
curl -s -X POST http://localhost:8088/api/categories \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Beverages","description":"Drinks"}'
```

Crea un producto en esa categoría. El stock inicial es 0:

```bash
curl -s -X POST http://localhost:8088/api/products \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"Water","sku":"WATER-1","price":1.5,"categoryId":"<category-id>"}'
```

Registra una entrada:

```bash
curl -s -X POST http://localhost:8088/api/inventory-movements \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"productId":"<product-id>","type":"In","quantity":5}'
```

Una salida mayor que el stock responde 409.

El listado de productos acepta `categoryId`. Borrar una categoría con productos, o un producto con movimientos, también responde 409.

## Rate limit

Hay dos ventanas fijas:

- `api`: 100 peticiones cada 60 segundos, por `client_id` del JWT o por IP si no hay token.
- `oauth`: 10 peticiones cada 60 segundos, por IP, sobre `POST /oauth/token`.

Al superar el cupo la API responde 429 con ProblemDetails y la cabecera `Retry-After`. `/health` no entra en el cupo.

Los cupos salen de `RATE_LIMIT_PERMIT_LIMIT`, `RATE_LIMIT_WINDOW_SECONDS` y `OAUTH_RATE_LIMIT_PERMIT_LIMIT`.

## Tests

```bash
dotnet test Inventory.sln
```

Cubren las reglas de stock, los handlers con puertos falsos, el script sin foreign keys y el mapeo de errores HTTP.

## Decisiones

- .NET 8. El enunciado admite .NET 6 o .NET Core. .NET 8 es la línea con soporte.
- PostgreSQL en lugar de SQL Server. Sigue siendo SQL relacional, la imagen oficial corre en ARM y el script de arranque es nativo. No hay foreign keys: la integridad la aplican el dominio y la aplicación.
- Lecturas con EF Core y escrituras con Dapper, como pide la prueba.

## Documentación operativa

- Arranque paso a paso, flujo curl y troubleshooting: [`docs/manual.md`](docs/manual.md)
- Contratos HTTP, cuerpos y códigos: [`docs/api.md`](docs/api.md)
- Quickstart de la feature Spec Kit: [`specs/001-inventory-api/quickstart.md`](specs/001-inventory-api/quickstart.md)

Los ejemplos cortos de token y CRUD de este README se mantienen como atajo; el detalle vive en el manual y en la doc del API.

## Uso de IA

Herramienta: Cursor.

Fases SDD: constitución → especificación → clarificación → plan → tareas (Spec Kit), checklist operativo en `PLAN.md`, instrucciones en `AGENTS.md`, implementación por capas con las skills `redarbor-implement`, `redarbor-review` y `redarbor-verify`, y verificación con build, tests y smoke de endpoints.

La salida se validó compilando la solución, ejecutando `dotnet test` y recorriendo token, alta de categoría, alta de producto, entrada de stock, salida rechazada por stock y la respuesta 429.
