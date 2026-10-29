# Inventory API Constitution

Principios vinculantes de la prueba técnica Redarbor Inventory API.
Toda especificación, plan, tarea e implementación se evalúa contra este documento.
Marco de trabajo: Spec-Driven Development con [GitHub Spec Kit](https://github.com/github/spec-kit).

## Core Principles

### I. Spec Before Code

El qué y el porqué viven en `spec.md` antes de ampliar el código.
El cómo técnico vive en `plan.md`.
El desglose ejecutable vive en `tasks.md`.
Las ambigüedades se resuelven en clarificaciones y se pliegan a la especificación.
Ningún cambio de alcance se considera cerrado solo porque el código ya existe.

### II. Hexagonal Layers And CQRS

La dependencia apunta hacia el dominio:

- `Inventory.Domain` solo usa el BCL.
- `Inventory.Application` define puertos y orquesta casos de uso con MediatR.
- `Inventory.Infrastructure` implementa puertos.
- `Inventory.Api` mapea HTTP; solo el composition root registra Infrastructure.

Lecturas con EF Core (`AsNoTracking`). Escrituras con Dapper.
Movimiento e inventario se persisten en una sola transacción Dapper.
No hay `IRepository<T>` genérico ni foreign keys en el esquema.

### III. Clean Code Rules Of The Test

Las siete reglas del enunciado prevalecen sobre el estilo C# habitual:

1. Código, comentarios y mensajes de API en inglés.
2. Sin línea en blanco entre sentencias, excepto una antes de `return`.
3. El nombre del método describe la acción; no comentar lo obvio.
4. Una responsabilidad: partir métodos de más de 25 líneas, con anidación mayor a un nivel, o que necesiten comentario para entenderse.
5. Configuración solo por variables de entorno.
6. Una clase o un enum por archivo.
7. Pasar objetos, no listas largas de propiedades sueltas.

### IV. Domain Owns Stock Invariants

El dominio valida cantidad positiva, tipos de movimiento y que una salida no deje stock negativo.
La aplicación comprueba existencia de categoría y producto, SKU único, y bloquea borrados con dependencias.
La API traduce excepciones a ProblemDetails con códigos HTTP coherentes.

### V. Security And Edge Limits

OAuth2 client credentials emite JWT Bearer.
Los endpoints de negocio exigen `Authorization: Bearer`.
Rate limiting en el borde de la API: políticas `api` y `oauth`, respuesta `429`.
`/health` queda fuera del cupo.
Secretos de desarrollo solo en `.env.example` y compose; nunca secretos reales en el repositorio.

### VI. Test-First For Business Rules

Toda regla de negocio nueva empieza con un test que falla.
Los handlers se prueban con puertos falsos.
Build y tests deben pasar antes de marcar verificación completa.
La revisión de capas e identificadores es un gate explícito.

### VII. Delivery Constraints

Stack fijado: .NET 8, PostgreSQL 16, Docker Compose.
Sin frontend, sin authorization code, sin foreign keys, sin caché, sin Testcontainers, sin envolver el rate limiter detrás de una interfaz propia.
Las respuestas al usuario sobre el avance del agente se explican en español; el código permanece en inglés.

## Architecture Gates

Antes de planificar o implementar, comprobar:

| Gate | Criterio |
|------|----------|
| Capas | Domain sin I/O; handlers sin SQL ni `HttpContext` |
| Persistencia | EF solo lectura; Dapper solo escritura; movimiento + stock atómicos |
| Esquema | Primary keys e índice único de SKU; sin foreign keys |
| Auth | Token por client credentials; resto JWT Bearer |
| Clean Code | Las siete reglas aplicables al diff |
| Alcance | Nada de la sección fuera de alcance del plan |

## Documentation Governance

Artefactos Spec Kit en este repositorio:

| Artefacto | Ruta | Rol |
|-----------|------|-----|
| Constitución | `.specify/memory/constitution.md` | Principios del proyecto |
| Especificación | `specs/001-inventory-api/spec.md` | Qué y porqué |
| Clarificaciones | sección en `spec.md` y `specs/001-inventory-api/clarifications.md` | Ambigüedades resueltas |
| Plan | `specs/001-inventory-api/plan.md` | Cómo técnico |
| Tareas | `specs/001-inventory-api/tasks.md` | Desglose ejecutable |
| Checklist operativo | `PLAN.md` | Seguimiento diario del agente |
| Guía del agente | `AGENTS.md` | Reglas cortas de implementación |

`PLAN.md` resume el checklist operativo y apunta a Spec Kit.
Si hay conflicto entre checklist y `tasks.md`, prevalece la especificación y luego el plan; se actualizan ambos documentos.

## Governance

1. Toda PR o entrega de agente que cambie comportamiento debe alinear `spec.md`, `plan.md` y `tasks.md` cuando el alcance cambie.
2. `/speckit.clarify` (o su equivalente manual) no inventa requisitos: registra pregunta, respuesta y el ajuste en la especificación.
3. Las skills `.cursor/skills/redarbor-*` ejecutan tareas; no sustituyen la constitución.
4. Enmiendas a esta constitución requieren actualizar la versión y la fecha de enmienda.

**Version**: 1.0.0 | **Ratified**: 2026-10-29 | **Last Amended**: 2026-10-29
