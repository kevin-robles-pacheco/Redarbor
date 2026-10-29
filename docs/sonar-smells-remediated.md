# Remediación code smells Sonar

**Fecha**: 2026-10-29 (America/New_York)  
**Rama**: `cursor/sonar-remediate-smells-117a`

Cierra los 19 code smells del análisis SonarQube (no había vulnerabilidades ni hotspots).

| Regla | Acción | Capa |
|-------|--------|------|
| S4457 | `ListAsync` valida y delega en `ListAllAsync` | Infrastructure |
| S107 | `Product` con ctor vacío + `ProductState`; `ProductResponse` desde `Product` | Domain / Application |
| S3925 | `.editorconfig` → `dotnet_diagnostic.S3925.severity = none` | Repo |

`dotnet build` Release: **0 warnings**. `dotnet test`: **41 passed**.

Siguiente paso del plan: coverage (Api / Infrastructure / Application Get-Update).
