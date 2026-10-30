# Tasks: Inventory API

**Input**: Design documents from `/specs/001-inventory-api/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/

**Tests**: Incluidos — el enunciado exige reglas de negocio con test primero.

**Organization**: Fases Spec Kit + historias de usuario. El checklist operativo diario sigue en [`PLAN.md`](../../PLAN.md) (ids cortos `domain-stock`, `api-oauth`, etc.).

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Puede ejecutarse en paralelo (archivos distintos, sin dependencia)
- **[Story]**: US1…US5 según `spec.md`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Solución, ignores y configuración base

- [x] T001 Crear `Inventory.sln` y proyectos Domain, Application, Infrastructure, Api
- [x] T002 [P] Crear `Inventory.Domain.Tests` e `Inventory.Application.Tests`
- [x] T003 [P] Añadir `.gitignore` que excluye `.env`
- [x] T004 [P] Añadir `.env.example` con variables sin secretos reales
- [x] T005 [P] Documentar constitución Spec Kit en `.specify/memory/constitution.md`
- [x] T006 [P] Documentar especificación, clarificaciones, plan y contratos bajo `specs/001-inventory-api/`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Dominio base, puertos, persistencia, auth y borde HTTP comunes

**⚠️ CRITICAL**: Las historias de negocio dependen de esta fase

- [x] T007 [P] Entidad `Category` + tests en Domain
- [x] T008 [P] Entidad `Product` + tests en Domain
- [x] T009 [P] Enum `MovementType` en archivo propio
- [x] T010 Entidad `InventoryMovement` + reglas de cantidad/tipo
- [x] T011 Stock adjustment: entrada, salida, insuficiente, cantidad no positiva (test primero)
- [x] T012 [P] Excepciones de dominio, una por archivo
- [x] T013 Puertos Application: categorías, productos, movimientos, conexión, esquema, token
- [x] T014 Script PostgreSQL sin FK, con PK e índice único SKU
- [x] T015 [P] `NpgsqlConnectionFactory`
- [x] T016 Inicializador idempotente de esquema
- [x] T017 Repositorios EF de lectura
- [x] T018 Repositorios Dapper de escritura + `IInventoryMovementWriter` transaccional
- [x] T019 OAuth token issuer/authenticator + JWT Bearer
- [x] T020 ProblemDetails / mapper de excepciones
- [x] T021 Rate limiting políticas `api` y `oauth`
- [x] T022 Swagger con OAuth2 client credentials

**Checkpoint**: Fundación lista — historias de usuario pueden cerrarse de punta a punta

---

## Phase 3: User Story 1 - Authenticate As API Client (Priority: P1) 🎯 MVP

**Goal**: Emitir y validar JWT vía client credentials

**Independent Test**: `POST /oauth/token` + llamada autenticada

- [x] T023 [US1] Handler `IssueAccessToken` + tests de credenciales
- [x] T024 [US1] `POST /oauth/token` form-urlencoded
- [x] T025 [US1] Proteger controllers de negocio con `[Authorize]`

**Checkpoint**: US1 operable

---

## Phase 4: User Story 2 - Manage Categories (Priority: P1)

**Goal**: CRUD de categorías con bloqueo de borrado si hay productos

**Independent Test**: CRUD `/api/categories`

- [x] T026 [P] [US2] Commands/queries/handlers/DTOs de categorías + tests
- [x] T027 [US2] `CategoriesController` CRUD
- [x] T028 [US2] 409 al borrar categoría con productos

**Checkpoint**: US1 + US2

---

## Phase 5: User Story 3 - Manage Products (Priority: P1)

**Goal**: CRUD de productos, SKU único, stock inicial 0

**Independent Test**: CRUD `/api/products?categoryId=`

- [x] T029 [P] [US3] Commands/queries/handlers/DTOs de productos + tests
- [x] T030 [US3] `ProductsController` CRUD + filtro `categoryId`
- [x] T031 [US3] 409 SKU duplicado y producto con movimientos

**Checkpoint**: US1–US3

---

## Phase 6: User Story 4 - Register Inventory Movements (Priority: P1)

**Goal**: Entradas/salidas atómicas con rechazo de stock insuficiente

**Independent Test**: `POST /api/inventory-movements`

- [x] T032 [US4] `RegisterInventoryMovement` handler + test con puerto falso
- [x] T033 [US4] `InventoryMovementsController`
- [x] T034 [US4] Persistir movimiento y `QuantityOnHand` en una transacción Dapper

**Checkpoint**: Flujo de inventario completo

---

## Phase 7: User Story 5 - Rate Limits And Observability (Priority: P2)

**Goal**: 429, health y Swagger demostrables

**Independent Test**: ráfaga 429; `/health`; Swagger

- [x] T035 [US5] Aplicar `[EnableRateLimiting]` en controllers
- [x] T036 [US5] Health endpoint fuera del cupo de smoke
- [x] T037 [US5] Documentar límites en README / manual

**Checkpoint**: Todas las historias independientes

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Empaque, docs de usuario y verificación

- [x] T038 [P] Dockerfile multi-stage
- [x] T039 [P] `docker-compose` api + postgres:16-alpine
- [x] T040 [P] README de arranque, pruebas, rate limit y declaración de IA
- [x] T041 [P] Documentación API en `docs/api.md`
- [x] T042 [P] Manual de uso en `docs/manual.md`
- [x] T043 `dotnet build`
- [x] T044 `dotnet test`
- [x] T045 Smoke: token, categoría, producto, entrada, salida rechazada, 429
- [x] T046 Revisión final de inglés, responsabilidades y capas (`verify-review`)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup** → sin dependencias
- **Foundational** → bloquea US1–US5
- **US1** → base de auth para US2–US5
- **US2** → categorías antes de productos (US3)
- **US3** → productos antes de movimientos (US4)
- **US5** → puede solaparse tras el borde HTTP fundacional
- **Polish** → tras historias deseadas

### Parallel Opportunities

- T002–T006 en Setup
- T007–T009 y T012 en Domain
- T015 y lecturas/escrituras tras el script SQL
- T041–T042 documentación en paralelo tras contratos estables

### Mapping to PLAN.md

| tasks.md | PLAN.md id |
|----------|------------|
| T001–T004 | skeleton-* |
| T007–T012 | domain-* |
| T013, T026, T029, T032 | app-* |
| T014–T018 | infra-* |
| T019–T025, T027, T030, T033, T035–T036 | api-* |
| T038–T040 | docker-*, docs-readme |
| T043–T046 | verify-* |

## Implementation Strategy

1. Fundación Domain → Application ports → Infrastructure → Api edge.
2. MVP = US1 + US2 + US3 + US4.
3. US5 y Docker cierran la demo.
4. T046 permanece abierta hasta la revisión de capas.

## Notes

- Identificadores y mensajes de API en inglés; explicaciones de avance del agente en español.
- Una tarea Spec Kit puede mapear a varias casillas `PLAN.md` ya cerradas; no reabrir trabajo hecho.
- Nueva feature: crear `specs/002-...` y no mezclar alcance aquí.
