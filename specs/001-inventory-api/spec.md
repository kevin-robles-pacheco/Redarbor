# Feature Specification: Inventory API

**Feature Branch**: `001-inventory-api`

**Created**: 2026-10-29

**Status**: Converged (implementation, docs and verification aligned — see `verification.md`)

**Input**: Prueba técnica Redarbor — API de inventario de productos con categorías, productos y movimientos; Clean Code; CQRS; EF lectura / Dapper escritura; OAuth2; rate limit; Docker.

## Clarifications

### Session 2026-10-29

- Q: ¿SQL Server es obligatorio o basta SQL relacional? → A: PostgreSQL 16 es aceptable; el enunciado prefiere SQL Server pero exige SQL relacional. Justificado en README. Sin foreign keys.
- Q: ¿Qué versión de .NET usar (.NET 6 / .NET Core)? → A: .NET 8 LTS. El TFM solo cambia si exigen literal `net6.0`.
- Q: ¿Cómo autenticar el API? → A: OAuth2 mínimo con `grant_type=client_credentials` en `POST /oauth/token` y JWT Bearer en el resto. Sin authorization code ni UI de login.
- Q: ¿Cómo limitar abuso? → A: Rate limiter de ASP.NET Core. Política `api` 100/60s y `oauth` 10/60s, configurables por entorno. Respuesta `429` con ProblemDetails y `Retry-After`. `/health` exento.
- Q: ¿Se borran categorías o productos con dependencias? → A: No. Categoría con productos → 409. Producto con movimientos → 409. La integridad la aplica la aplicación, no FK.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Authenticate As API Client (Priority: P1)

Un cliente máquina obtiene un access token con client id y secret para llamar al API.

**Why this priority**: Sin token no hay acceso al resto de operaciones.

**Independent Test**: `POST /oauth/token` con credenciales válidas devuelve Bearer; credenciales inválidas → 401; grant distinto → 400.

**Acceptance Scenarios**:

1. **Given** credenciales válidas de entorno, **When** se pide token con `client_credentials`, **Then** la respuesta incluye `access_token`, `token_type=Bearer` y `expires_in`.
2. **Given** secret incorrecto, **When** se pide token, **Then** responde 401 ProblemDetails.
3. **Given** más de N peticiones oauth en la ventana, **When** se supera el cupo, **Then** responde 429.

---

### User Story 2 - Manage Categories (Priority: P1)

Un cliente autenticado crea, lista, consulta, actualiza y elimina categorías de producto.

**Why this priority**: Los productos dependen de una categoría existente.

**Independent Test**: CRUD completo de `/api/categories` con Bearer token.

**Acceptance Scenarios**:

1. **Given** un cliente autenticado, **When** crea una categoría con nombre, **Then** responde 201 con id y metadatos.
2. **Given** categorías existentes, **When** lista o consulta por id, **Then** responde 200 con los datos.
3. **Given** una categoría sin productos, **When** la elimina, **Then** responde 204.
4. **Given** una categoría con productos, **When** intenta eliminarla, **Then** responde 409.

---

### User Story 3 - Manage Products (Priority: P1)

Un cliente autenticado administra el catálogo: nombre, descripción, SKU, precio y categoría. El stock inicial es cero.

**Why this priority**: El inventario opera sobre productos.

**Independent Test**: CRUD de `/api/products`, filtro opcional por `categoryId`, SKU único.

**Acceptance Scenarios**:

1. **Given** una categoría existente, **When** crea un producto con SKU único, **Then** responde 201 con `quantityOnHand=0`.
2. **Given** un SKU ya usado, **When** crea o actualiza otro producto con ese SKU, **Then** responde 409.
3. **Given** una categoría inexistente, **When** crea un producto, **Then** responde error de negocio mapeado (categoría no encontrada).
4. **Given** un producto con movimientos, **When** intenta borrarlo, **Then** responde 409.

---

### User Story 4 - Register Inventory Movements (Priority: P1)

Un cliente autenticado registra entradas y salidas. El stock se actualiza de forma atómica junto al movimiento.

**Why this priority**: Es el núcleo del inventario.

**Independent Test**: `POST /api/inventory-movements` con tipos `In`/`Out`; salida excesiva rechazada.

**Acceptance Scenarios**:

1. **Given** un producto con stock 0, **When** registra entrada de 5, **Then** el producto queda con stock 5.
2. **Given** stock 5, **When** registra salida de 3, **Then** el producto queda con stock 2.
3. **Given** stock 2, **When** registra salida de 5, **Then** responde 409 por stock insuficiente y el stock no cambia.
4. **Given** cantidad no positiva, **When** registra movimiento, **Then** responde 400.

---

### User Story 5 - Operate Under Rate Limits And Observability (Priority: P2)

El borde del API limita ráfagas y expone salud y documentación interactiva.

**Why this priority**: Exigido por la prueba para robustez y demostración.

**Independent Test**: Ráfaga sobre API → 429; `/health` y Swagger accesibles.

**Acceptance Scenarios**:

1. **Given** cupo `api` agotado, **When** llega otra petición protegida, **Then** 429 con `Retry-After`.
2. **Given** el servicio arriba, **When** se consulta `/health`, **Then** responde éxito sin consumir el cupo oauth/api de negocio de forma bloqueante para el smoke.
3. **Given** el servicio arriba, **When** se abre Swagger, **Then** el esquema OAuth2 client credentials apunta a `/oauth/token`.

### Edge Cases

- Nombre de categoría o producto vacío → 400.
- Precio negativo → 400.
- SKU vacío → 400.
- Tipo de movimiento inválido → 400.
- Producto o categoría inexistente en lectura/actualización/borrado → 404.
- Token ausente o inválido en endpoints de negocio → 401.
- Arranque: esquema SQL idempotente aplicado sin foreign keys.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: El sistema MUST exponer CRUD de categorías en `/api/categories`.
- **FR-002**: El sistema MUST exponer CRUD de productos en `/api/products`, con listado filtrable por `categoryId`.
- **FR-003**: El sistema MUST registrar movimientos de inventario en `POST /api/inventory-movements` con tipos de entrada y salida.
- **FR-004**: El sistema MUST mantener `quantityOnHand` coherente; una salida no puede dejar stock bajo cero.
- **FR-005**: El sistema MUST exigir cantidad de movimiento estrictamente mayor que cero.
- **FR-006**: El sistema MUST exigir que un producto referencie una categoría existente.
- **FR-007**: El sistema MUST impedir borrar una categoría que tenga productos.
- **FR-008**: El sistema MUST impedir borrar un producto que tenga movimientos.
- **FR-009**: El sistema MUST garantizar unicidad de SKU.
- **FR-010**: El sistema MUST autenticar clientes con OAuth2 client credentials y proteger el API con JWT Bearer.
- **FR-011**: El sistema MUST aplicar rate limiting configurable y responder 429 al superar el cupo.
- **FR-012**: El sistema MUST devolver errores de negocio como ProblemDetails en inglés.
- **FR-013**: El sistema MUST persistir en PostgreSQL sin foreign keys, con primary keys e índice único de SKU.
- **FR-014**: El sistema MUST leer con EF Core y escribir con Dapper; movimiento + stock en una transacción.
- **FR-015**: El sistema MUST poder arrancarse con Docker Compose (API + PostgreSQL).
- **FR-016**: El sistema MUST leer toda la configuración sensible y de límites desde variables de entorno.

### Key Entities

- **Category**: Agrupa productos; nombre, descripción, activo, fecha de creación.
- **Product**: Ítem inventariable; SKU único, precio, categoría, stock on hand, activo.
- **InventoryMovement**: Entrada o salida de una cantidad sobre un producto, con notas y marca temporal.
- **AccessToken**: Credencial JWT emitida a un client id autenticado.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: `dotnet build` y `dotnet test` completan sin fallos en la solución.
- **SC-002**: Smoke manual obtiene token, crea categoría y producto, registra entrada, rechaza salida excesiva y observa 429 bajo ráfaga.
- **SC-003**: El esquema SQL no declara foreign keys y declara índice único de SKU.
- **SC-004**: Un revisor puede seguir constitución → especificación → plan → tareas sin contradicciones abiertas.
- **SC-005**: Documentación de API y manual de uso permiten operar el sistema sin leer el código fuente.

## Assumptions

- Consumidores son clientes máquina (no hay UI ni usuarios humanos en el alcance).
- Credenciales y claves JWT de desarrollo son suficientes para la entrega; producción usaría secretos externos.
- Un solo stock on hand por producto (sin almacenes múltiples).
- Tipos de movimiento expuestos como `In` / `Out` en el contrato HTTP.
- El checklist `PLAN.md` coexiste como vista operativa del agente; Spec Kit es la fuente SDD.
