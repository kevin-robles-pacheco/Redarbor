# Clarificaciones — Inventory API

Registro de la fase **Clarificación** de Spec Kit (`/speckit.clarify`).
Las respuestas ya están pliegadas en [`spec.md`](./spec.md) (sección Clarifications) y en el plan técnico.
Este archivo es el índice auditable de decisiones que cerraron ambigüedades del enunciado.

## Sesión 2026-10-29

| # | Tema | Pregunta | Respuesta adoptada | Impacto |
|---|------|----------|--------------------|---------|
| 1 | Motor SQL | ¿SQL Server obligatorio? | PostgreSQL 16; SQL relacional sin FK | `plan.md`, README, `schema.sql` |
| 2 | Runtime | ¿.NET 6 / .NET Core literal? | .NET 8 LTS | TFM de proyectos, Docker |
| 3 | Auth | ¿Qué flujo OAuth2? | Solo client credentials + JWT Bearer | `POST /oauth/token`, Swagger |
| 4 | Abuso | ¿Cómo limitar peticiones? | Rate limiter ASP.NET Core, políticas `api`/`oauth`, 429 | Variables `RATE_LIMIT_*`, `OAUTH_RATE_LIMIT_*` |
| 5 | Borrados | ¿Cascada o bloqueo? | Bloqueo con 409 si hay dependencias | Handlers de delete, sin FK |

## Criterios para nuevas clarificaciones

1. Solo preguntar si el enunciado o la especificación dejan dos comportamientos válidos.
2. Máximo cinco preguntas por sesión, las más bloqueantes primero.
3. Tras aceptar una respuesta: añadir fila aquí, bullet en `spec.md` y ajustar FR/escenarios afectados.
4. No usar clarificación para meter detalle de stack; eso va al plan.

## Estado

Ambigüedades críticas del enunciado original: **cerradas**.
No quedan marcadores `NEEDS CLARIFICATION` en `spec.md`.
