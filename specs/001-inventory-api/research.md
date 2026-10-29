# Research: Inventory API

## R1 — Runtime .NET 8

**Decision**: Target `net8.0`.

**Rationale**: El enunciado admite .NET 6 o .NET Core. .NET 8 es LTS actual, con rate limiter y hosting maduros.

**Alternatives considered**: `net6.0` literal — solo si el evaluador lo exige de forma rígida (cambio de TFM).

## R2 — PostgreSQL 16 without foreign keys

**Decision**: PostgreSQL 16 Alpine vía Docker; esquema sin FK; integridad en dominio/aplicación.

**Rationale**: Cumple SQL relacional, corre en Apple Silicon, imagen oficial ligera. Clarificación #1.

**Alternatives considered**: SQL Server — preferido por el PDF pero no obligatorio tras clarificar; más pesado en ARM.

## R3 — EF Core reads, Dapper writes

**Decision**: Repositorios de lectura con EF `AsNoTracking`; escritura y movimientos con Dapper; una transacción para insertar movimiento y actualizar stock.

**Rationale**: Exigencia explícita de la prueba técnica y de la constitución.

**Alternatives considered**: EF para todo — viola el enunciado; Dapper para todo — pierde el split pedido.

## R4 — MediatR CQRS

**Decision**: Commands/queries + handlers en Application; controllers solo envían mensajes.

**Rationale**: Separa casos de uso, facilita tests con puertos falsos y mantiene controllers delgados.

## R5 — OAuth2 client credentials

**Decision**: `POST /oauth/token` form-urlencoded; JWT firmado con clave de entorno; Bearer en el resto.

**Rationale**: Alcance mínimo de auth máquina-a-máquina sin UI.

**Alternatives considered**: Authorization code — fuera de alcance; API keys estáticas sin JWT — menos alineado con OAuth2 pedido.

## R6 — ASP.NET Core rate limiter

**Decision**: Políticas fijas `api` y `oauth` con límites por variables de entorno; partición por `client_id` del JWT o IP.

**Rationale**: Cumple 429 sin abstracciones extra (fuera de alcance envolver el limiter).

## R7 — ProblemDetails

**Decision**: Mapper central de excepciones de dominio/aplicación a 400/401/404/409/500.

**Rationale**: Mensajes en inglés, contrato uniforme para clientes y Swagger.
