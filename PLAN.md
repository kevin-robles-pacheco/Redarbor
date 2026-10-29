# Plan de trabajo — Prueba Redarbor

API de inventario en .NET 8 con PostgreSQL, capas hexagonales, identificadores en inglés, CQRS, EF Core en lecturas, Dapper en escrituras, OAuth2, rate limit y Docker.

Marca cada tarea en este archivo. Pendiente: `- [ ]`. Completada: `- [x]`. En la vista previa de Markdown la casilla se puede pulsar. La skill `redarbor-implement` toma la primera casilla abierta, hace solo esa tarea y la cierra.

Fuente del enunciado: `Redarbor - Prueba técnica developer uso IA.pdf`. El enunciado anuncia 8 reglas de Clean Code y enumera 7. Se implementan las 7 escritas.

## Spec-Driven Development (Spec Kit)

Este checklist es la vista operativa del agente. La fuente SDD está en:

| Fase | Ruta |
|------|------|
| Constitución | [`.specify/memory/constitution.md`](.specify/memory/constitution.md) |
| Especificación | [`specs/001-inventory-api/spec.md`](specs/001-inventory-api/spec.md) |
| Clarificación | [`specs/001-inventory-api/clarifications.md`](specs/001-inventory-api/clarifications.md) |
| Plan | [`specs/001-inventory-api/plan.md`](specs/001-inventory-api/plan.md) |
| Tareas | [`specs/001-inventory-api/tasks.md`](specs/001-inventory-api/tasks.md) |
| API / Manual | [`docs/api.md`](docs/api.md), [`docs/manual.md`](docs/manual.md) |

Si el alcance cambia, actualiza primero `spec.md` y luego alinea este archivo con `tasks.md`.

## Decisiones

- .NET 8. El enunciado pide .NET 6 o .NET Core. .NET 8 es la línea LTS actual. El cambio a `net6.0` es solo el TFM si lo exigen literal.
- PostgreSQL 16. El enunciado pide SQL relacional y prefiere SQL Server. PostgreSQL cumple la obligación y queda justificado en el README. Sin foreign keys.
- EF Core (Npgsql) solo lee. Dapper (Npgsql) solo escribe. Los handlers dependen de un puerto.
- OAuth2 mínimo: `POST /oauth/token` con `grant_type=client_credentials` y JWT Bearer en el resto.
- Rate limit con `AddRateLimiter` de ASP.NET Core. Políticas `api` (100/60s) y `oauth` (10/60s), por variables de entorno. Respuesta `429`.
- Capas: Api → Application → Domain ← Infrastructure. El composition root es el único punto de Api que registra Infrastructure.

## Capas

- **Domain.** Entidades, `MovementType`, stock y excepciones. Sin ASP.NET, EF, Dapper ni Npgsql.
- **Application.** Commands, queries, handlers, DTOs y puertos. Sin SQL ni `HttpContext`.
- **Infrastructure.** EF de lectura, Dapper de escritura, script SQL y fábrica de conexiones.
- **Api.** Controllers delgados, OAuth2, rate limit, Swagger y ProblemDetails.

Puertos: `ICategoryReadRepository`, `ICategoryWriteRepository`, `IProductReadRepository`, `IProductWriteRepository`, `IInventoryMovementWriter`, `IDbConnectionFactory`, `IDatabaseSchemaInitializer`, `IClientCredentialAuthenticator`, `IAccessTokenIssuer`.

Reglas de negocio: cantidad mayor que cero; una salida no deja el stock bajo cero; el producto exige categoría existente; no se borra una categoría con productos ni un producto con movimientos; stock y movimiento se escriben en la misma transacción Dapper.

Identificadores en inglés. Una clase o enum por archivo. Métodos de hasta 25 líneas. Una línea en blanco solo antes del `return`. Parámetros como objeto. Configuración por variables de entorno.

Skills: `.cursor/skills/redarbor-implement`, `.cursor/skills/redarbor-review`, `.cursor/skills/redarbor-verify`. Reglas detalladas en `AGENTS.md`.

## Tareas

### Documentación del flujo

- [x] `docs-plan` — `PLAN.md` con las casillas de seguimiento
- [x] `docs-agents` — `AGENTS.md` con stack, capas y las 7 reglas
- [x] `docs-skill-implement` — skill que toma la siguiente tarea pendiente
- [x] `docs-skill-review` — skill de revisión contra las reglas del proyecto
- [x] `docs-skill-verify` — skill de build, tests y humo
- [x] `docs-sdd-constitution` — constitución Spec Kit en `.specify/memory/constitution.md`
- [x] `docs-sdd-spec` — especificación y clarificaciones en `specs/001-inventory-api/`
- [x] `docs-sdd-plan-tasks` — plan y tareas Spec Kit alineados al checklist
- [x] `docs-api` — documentación HTTP en `docs/api.md`
- [x] `docs-manual` — manual de uso en `docs/manual.md`
- [x] `sonar-fixells-s4457` — validación fuera del método async en `CategoryEfReadRepository`
- [x] `sonar-smells-s107` — `Product`/`ProductResponse` sin constructores de 9 parámetros
- [x] `sonar-smells-s3925` — `.editorconfig` desactiva S3925 (serialización obsoleta en .NET 8)

### Esqueleto

- [x] `skeleton-solution` — `Inventory.sln` y proyectos Domain, Application, Infrastructure, Api
- [x] `skeleton-tests` — `Inventory.Domain.Tests` e `Inventory.Application.Tests`
- [x] `skeleton-ignore` — `.gitignore` que excluye `.env`
- [x] `skeleton-env` — `.env.example` con las variables, sin secretos reales

### Dominio (test primero)

- [x] `domain-category` — entidad `Category` y sus tests
- [x] `domain-product` — entidad `Product` y sus tests
- [x] `domain-movement-type` — enum `MovementType` en su propio archivo
- [x] `domain-movement` — entidad `InventoryMovement`
- [x] `domain-stock` — tests de entrada, salida, stock insuficiente y cantidad no positiva; después la regla
- [x] `domain-exceptions` — excepciones de dominio, una por archivo

### Aplicación

- [x] `app-ports` — puertos de lectura, escritura, movimiento, conexión, esquema y token
- [x] `app-categories` — commands, queries, handlers y DTOs de categorías, con tests
- [x] `app-products` — commands, queries, handlers y DTOs de productos, con tests
- [x] `app-movements` — `RegisterInventoryMovement` y su test con puerto falso

### Infraestructura

- [x] `infra-sql` — script PostgreSQL sin foreign keys, con primary keys e índice único de SKU
- [x] `infra-ef-read` — repositorios EF de lectura
- [x] `infra-dapper-write` — repositorios Dapper de escritura y writer transaccional
- [x] `infra-connection` — `NpgsqlConnectionFactory`
- [x] `infra-schema` — inicializador idempotente del esquema

### API

- [x] `api-oauth` — `POST /oauth/token` y validación JWT
- [x] `api-categories` — CRUD `/api/categories`
- [x] `api-products` — CRUD `/api/products`
- [x] `api-movements` — `POST /api/inventory-movements`
- [x] `api-rate-limit` — políticas `api` y `oauth`, respuesta 429
- [x] `api-errors` — ProblemDetails
- [x] `api-swagger` — Swagger con OAuth2 client credentials

### Docker y README

- [x] `docker-api` — Dockerfile multi-stage
- [x] `docker-compose` — `api` + `postgres:16-alpine`
- [x] `docs-readme` — README de arranque, pruebas, rate limit y declaración de IA

### Verificación

- [x] `verify-build` — `dotnet build`
- [x] `verify-test` — `dotnet test`
- [ ] `verify-review` — revisión de inglés, responsabilidades y capas
- [x] `verify-smoke` — token, categoría, producto, entrada, salida rechazada y ráfaga 429

## Fuera de alcance

No hay frontend, ni authorization code, ni foreign keys, ni caché, ni Testcontainers, ni una interfaz que envuelva el rate limiter.
