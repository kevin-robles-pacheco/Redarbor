# Implementation Plan: Inventory API

**Branch**: `001-inventory-api` | **Date**: 2026-10-29 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-inventory-api/spec.md`

## Summary

API REST de inventario con categorías, productos y movimientos.
Arquitectura hexagonal en .NET 8, PostgreSQL 16, CQRS con MediatR, EF Core solo lecturas, Dapper solo escrituras, OAuth2 client credentials, rate limiting y Docker Compose.

## Technical Context

**Language/Version**: C# / .NET 8

**Primary Dependencies**: ASP.NET Core, MediatR, EF Core Npgsql, Dapper, Npgsql

**Storage**: PostgreSQL 16 (sin foreign keys; PK + índice único SKU)

**Testing**: xUnit en `Inventory.Domain.Tests` e `Inventory.Application.Tests`

**Target Platform**: Linux container / Docker Compose; desarrollo local con SDK .NET 8

**Project Type**: web-service (API REST)

**Performance Goals**: Suficiente para demo técnica; límites explícitos vía rate limit (100 req/60s API, 10 req/60s oauth)

**Constraints**: Siete reglas Clean Code del enunciado; sin FK; sin frontend; configuración por entorno

**Scale/Scope**: Un servicio, tres agregados de negocio, auth mínima, smoke manual

## Constitution Check

*GATE: Must pass before research/design. Re-check after design.*

| Gate | Resultado |
|------|-----------|
| Capas hexagonales y CQRS | PASS — Domain / Application / Infrastructure / Api |
| EF lectura / Dapper escritura | PASS — repositorios separados; writer transaccional |
| Clean Code (7 reglas) | PASS — enforced by AGENTS.md y skills |
| OAuth2 client credentials + JWT | PASS |
| Rate limit + 429 | PASS |
| Sin FK / sin frontend / env config | PASS |
| Test-first en reglas de stock | PASS — Domain.Tests |

Violaciones justificadas: ninguna pendiente. PostgreSQL en lugar de SQL Server está documentado como decisión aceptada (clarificación #1).

## Project Structure

### Documentation (this feature)

```text
specs/001-inventory-api/
├── plan.md              # Este archivo
├── research.md          # Decisiones técnicas
├── data-model.md        # Modelo de datos
├── quickstart.md        # Arranque rápido de la feature
├── clarifications.md    # Índice de clarificaciones
├── contracts/           # Contratos HTTP
│   └── rest-api.md
├── tasks.md             # Desglose ejecutable
└── spec.md              # Especificación + clarificaciones embebidas

.specify/memory/
└── constitution.md      # Principios del proyecto

docs/
├── api.md               # Documentación completa del API
└── manual.md            # Manual de uso
```

### Source Code (repository root)

```text
src/
├── Inventory.Domain/
├── Inventory.Application/
├── Inventory.Infrastructure/
└── Inventory.Api/
tests/
├── Inventory.Domain.Tests/
└── Inventory.Application.Tests/
```

**Structure Decision**: Opción servicio web en capas hexagonales bajo `src/` y `tests/`, alineada con AGENTS.md.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Cuatro proyectos + tests | Separar Domain/Application/Infrastructure/Api | Un solo proyecto mezclaría I/O y dominio y viola el enunciado |
| Puertos específicos (no `IRepository<T>`) | Lectura/escritura y casos de uso distintos | Un repositorio genérico oculta el split EF/Dapper |
| PostgreSQL vs SQL Server | ARM / Docker oficial / SQL relacional | SQL Server no era obligatorio tras clarificación |

## Phase Mapping

| Phase Spec Kit | Artefacto | Contenido |
|----------------|-----------|-----------|
| 0 Research | `research.md` | Stack y trade-offs |
| 1 Design | `data-model.md`, `contracts/`, `quickstart.md` | Modelo y contratos |
| 2 Tasks | `tasks.md` | Orden de implementación |
| Ops | `PLAN.md`, skills | Ejecución por el agente |
