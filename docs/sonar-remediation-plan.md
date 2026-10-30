# Plan de remediación Sonar — Inventory API

Basado en [`sonar-report.md`](./sonar-report.md) (análisis 2026-10-29).  
Objetivo: cerrar smells accionables, subir coverage hacia **≥ 80%** en capas críticas y mantener **0** bugs/vulnerabilities.

Alineación: no violar `AGENTS.md` (capas, Clean Code de la prueba, sin Testcontainers si se mantiene el fuera de alcance actual).

## Prioridad

| Prio | Tema | Esfuerzo relativo | Impacto |
|------|------|-------------------|---------|
| P0 | Coverage Api + Infrastructure | Alto | Cierra el mayor gap del informe |
| P1 | Coverage Application (Get/Update) | Medio | Sube Application ~63% → 85%+ |
| P2 | S107 Product / ProductResponse | Bajo–medio | 2 smells; alinea con “pasar objetos” |
| P3 | S4457 CategoryEfReadRepository | Bajo | 1 smell |
| P4 | S3925 excepciones (16) | Bajo (suppress) o medio (ctors) | Ruido Sonar vs .NET 8 |

Seguridad Sonar: **nada que remediar** en el informe. Solo endurecimiento operativo opcional (P5).

---

## Fase A — Coverage (P0 / P1)

### A1. Api con `WebApplicationFactory` (P0)

**Meta**: Api de ~6% → ≥ 70%.

Tareas:

1. Añadir paquete `Microsoft.AspNetCore.Mvc.Testing` en `Inventory.Api.Tests`.
2. Factory de prueba que:
   - Usa variables de entorno de test (JWT, OAuth, rate limit holgado o bypass controlado).
   - Sustituye puertos de Application por fakes in-memory (no Postgres real, coherente con fuera de alcance Testcontainers).
3. Tests de contrato HTTP:
   - `POST /oauth/token` ok / 401 / 400 grant
   - CRUD categories/products vía MediatR fake o handlers reales con repos fake
   - `POST /api/inventory-movements` In/Out/409
   - ProblemDetails en 404/409
   - Cabecera `Retry-After` en 429 (política oauth con límite bajo en test)
4. Cubrir `ApiExceptionMapper` / `ApiExceptionHandler` con excepciones representativas.

**DoD**: coverage Api ≥ 70% en nuevo informe Sonar; tests verdes en CI.

### A2. Infrastructure sin Testcontainers (P0)

**Meta**: Infrastructure ~26% → ≥ 60%.

Opciones compatibles con el alcance:

1. **EF InMemory o SQLite** solo para repositorios de lectura (aceptando diferencias con Npgsql) *o*
2. Tests de mapeo/SQL string + mocks de `IDbConnection` para Dapper (más frágil) *o*
3. Si se autoriza ampliar alcance: Testcontainers PostgreSQL (mejor fidelidad; hoy está en “fuera de alcance” del `PLAN.md` — requiere clarificación Spec Kit).

Recomendación inmediata: tests de **unidades alrededor del writer** con abstracción ya existente (`IDbConnectionFactory` fake que ejecuta contra SQLite/Npgsql embebido si se aprueba), y tests del script `schema.sql` ya existentes ampliados.

**DoD**: `InventoryMovementDapperWriter`, repos Dapper/EF principales con al menos caminos felices y un error; coverage Infrastructure ≥ 60%.

### A3. Application Get/Update (P1)

**Meta**: Application ≥ 85%.

Tareas:

1. Tests de `GetCategories`, `GetCategory`, `GetProducts`, `GetProduct` con repos fake.
2. Tests de `UpdateCategory`, `UpdateProduct` (ok, not found, duplicate SKU).
3. Cubrir `ApplicationServiceRegistration` solo si aporta valor (o excluir de coverage en Sonar).

**DoD**: handlers de lectura/actualización con tests; Application ≥ 85%.

### A4. Exclusiones Sonar de coverage (opcional)

Excluir del cálculo (no del análisis) archivos de composition root puros si no se testean:

- `Program.cs` (ya excluirlo)
- `*Registration.cs` de DI/Swagger si se decide no cubrir

Documentar exclusiones en el comando `sonarscanner begin`.

---

## Fase B — Code smells

### B1. S107 — constructores con 9 parámetros (P2)

**Archivos**: `Product.cs`, `ProductResponse.cs`.

Plan:

1. Introducir un objeto de estado/inmutabilidad, p. ej. `ProductState` o reutilizar `CreateProduct`/`UpdateProduct` para hidratación.
2. `Product` ctor privado recibe **un objeto** (cumple Clean Code regla 7 y baja params Sonar).
3. `ProductResponse.From(Product)` sin ctor de 9 args sueltos; o ctor que acepte `Product` / DTO interno.

**DoD**: 0 issues S107; tests Domain/Application siguen verdes.

### B2. S4457 — `ListAsync` (P3)

**Archivo**: `CategoryEfReadRepository.cs`.

Plan:

1. Extraer validación: método sync `EnsureListQuery(GetCategoriesQuery query)` o validar en el caller.
2. Método async solo hace la consulta EF.

**DoD**: 0 issues S4457; mismo comportamiento.

### B3. S3925 — excepciones (P4)

Contexto: en .NET 8 la serialización binaria de `Exception` está obsoleta; implementar el patrón completo añade ruido.

**Opción recomendada (rápida y honesta)**:

1. Añadir `.editorconfig` o parámetros del analyzer:

   ```ini
   dotnet_diagnostic.S3925.severity = none
   ```

   o excluir la regla en Sonar (`sonar.issue.ignore.multicriteria`) con justificación en este plan.

2. Alternativa si se exige cumplimiento literal: constructores estándar `(string)`, `(string, Exception)` y ctor `SerializationInfo` obsoleto en las 16 excepciones (violaría en parte “métodos mínimos” / boilerplate).

**DoD**: 0 issues S3925 abiertos en Sonar (suppress documentado **o** patrón aplicado).

---

## Fase C — Seguridad operativa (P5, opcional)

Sin hallazgos Sonar. Mejoras de endurecimiento si se despliega fuera de la demo:

1. Secretos solo desde vault/env de despliegue; rotar `JWT_SIGNING_KEY` (≥ 32 bytes).
2. HTTPS terminado en reverse proxy; cookies N/A (API Bearer).
3. Política de rate limit distinta por ambiente.
4. No publicar Swagger en producción o protegerlo.

---

## Orden de ejecución sugerido

```text
1. B3 suppress S3925 (rápido, limpia 16/19 smells)
2. B2 S4457 (1 archivo)
3. B1 S107 Product/ProductResponse
4. A3 tests Application Get/Update
5. A1 WebApplicationFactory Api
6. A2 Infrastructure (según decisión Testcontainers)
7. Re-correr Sonar → actualizar sonar-report.md
```

## Criterios de salida globales

- Bugs = 0, Vulnerabilities = 0, Hotspots = 0 (mantener)
- Code smells accionables = 0 (o solo deuda aceptada documentada)
- Coverage global ≥ 80% **o** Domain ≥ 95% + Application ≥ 85% + Api ≥ 70% + Infrastructure ≥ 60%
- Quality Gate OK con perfil que exija coverage (cuando se configure en SonarCloud/CI)

## Seguimiento Spec Kit

Si se implementa este plan como trabajo de código:

1. Nueva feature o fase en `specs/` (p. ej. `002-sonar-remediation`) **o** tareas Convergence en `tasks.md`.
2. Actualizar `PLAN.md` con casillas `sonar-*`.
3. No mezclar remediación con cambios de negocio no especificados.

## Fuera de este plan

- Cambiar PostgreSQL → SQL Server
- Añadir frontend
- Introducir foreign keys
- Envolver rate limiter en interfaz propia
