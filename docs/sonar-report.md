# Informe SonarQube — Inventory API

**Fecha**: 2026-10-29 (America/New_York)  
**Motor**: SonarQube 9.9 LTS Community (`sonarqube:lts-community`)  
**Scanner**: SonarScanner for .NET 9.0.2 + `dotnet-coverage` (VS Coverage XML)  
**Proyecto**: `inventory-api`  
**Quality Gate**: OK (perfil por defecto Community)

## Resumen ejecutivo

| Métrica | Valor | Lectura |
|---------|-------|---------|
| Bugs | **0** | Reliability A |
| Vulnerabilities | **0** | Security A |
| Security Hotspots | **0** | Sin hotspots abiertos |
| Code Smells | **19** | Maintainability A (deuda ~375 min) |
| Coverage | **47.9%** | Bajo el umbral habitual 80% |
| Duplicación | **1.8%** | Aceptable |
| ncloc | 2013 | — |
| Complejidad / cognitiva | 398 / 56 | — |

**Conclusión**: el código está limpio en bugs y seguridad Sonar. El foco de remediación es **cobertura** (sobre todo Api e Infrastructure) y **19 code smells** concentrados en 3 reglas.

## Seguridad

| Tipo | Cantidad |
|------|----------|
| Vulnerabilities | 0 |
| Security Hotspots | 0 |
| Security Rating | A (1.0) |

No hay hallazgos de seguridad en el análisis estático Sonar.

### Riesgos residuales (fuera de reglas Sonar, contexto del proyecto)

1. Secretos de desarrollo en `docker-compose.yml` / `.env.example` (`local-dev-only`) — aceptados para la demo; no usar en producción.
2. JWT firmado con clave simétrica desde entorno — correcto para el alcance; rotación y longitud de clave quedan operativos.
3. Rate limiting y OAuth están en el borde HTTP — validados por smoke; no aparecen como hotspot.

## Code smells (19)

Todos **MAJOR**. Desglose por regla:

| Regla | # | Severidad | Significado |
|-------|---|-----------|-------------|
| [S3925](https://rules.sonarsource.com/csharp/RSPEC-3925) | 16 | MAJOR | Excepciones sin patrón de serialización `ISerializable` |
| [S107](https://rules.sonarsource.com/csharp/RSPEC-107) | 2 | MAJOR | Constructores con > 7 parámetros |
| [S4457](https://rules.sonarsource.com/csharp/RSPEC-4457) | 1 | MAJOR | Validar parámetros fuera del método `async` |

### S3925 — excepciones (16)

Archivos Domain:

- `InvalidCategoryNameException`, `InsufficientStockException`, `InvalidMovementTypeException`, `ProductReferenceIsRequiredException`, `QuantityMustBePositiveException`
- `CategoryReferenceIsRequiredException`, `InvalidPriceException`, `InvalidProductNameException`, `InvalidSkuException`, `MovementProductMismatchException`

Archivos Application:

- `InvalidClientCredentialsException`, `CategoryHasProductsException`, `CategoryNotFoundException`, `ProductHasMovementsException`, `DuplicateSkuException`, `ProductNotFoundException`

### S107 — demasiados parámetros (2)

- `src/Inventory.Domain/Products/Product.cs` (ctor privado, 9 params)
- `src/Inventory.Application/Products/ProductResponse.cs` (ctor público, 9 params)

### S4457 — async + validación (1)

- `src/Inventory.Infrastructure/Persistence/CategoryEfReadRepository.cs` — `ListAsync` hace `ThrowIfNull` dentro del método async

## Coverage (47.9%)

Importación: `coverage/coverage.xml` vía `sonar.cs.vscoveragexml.reportsPaths`.  
Estadística del sensor: 95 archivos main con datos de coverage.

### Por ensamblado (`dotnet-coverage`)

| Ensamblado | Line coverage |
|------------|---------------|
| Inventory.Domain | **97.0%** |
| Inventory.Application | **63.0%** |
| Inventory.Infrastructure | **25.6%** |
| Inventory.Api | **6.2%** |

### Zonas con más líneas sin cubrir (prioridad)

- Api: `InventoryApiRegistration`, controllers (`Categories`, `Products`, `OAuth`, `Movements`), `ApiSettings`, auth JWT, rate limit, Swagger
- Infrastructure: `ProductEfReadRepository`, Dapper writers/repos, schema/connection factory
- Application: handlers de lectura/update (`Get*`, `Update*`)

Domain está bien cubierto por tests unitarios existentes.

## Duplicación

Densidad **1.8%**. No es prioritaria frente a coverage y smells.

## Cómo reproducir el análisis

Requisitos: Docker, .NET 8 SDK, Java 17+, herramientas globales `dotnet-sonarscanner` y `dotnet-coverage`.

```bash
# 1) SonarQube local
docker run -d --name sonarqube -p 9000:9000 \
  -e SONAR_ES_BOOTSTRAP_CHECKS_DISABLE=true \
  sonarqube:lts-community

# 2) Token de usuario (UI o API) y luego:
export SONAR_TOKEN=...
dotnet sonarscanner begin /k:"inventory-api" /d:sonar.host.url="http://127.0.0.1:9000" \
  /d:sonar.login="$SONAR_TOKEN" \
  /d:sonar.cs.vscoveragexml.reportsPaths="$PWD/coverage/coverage.xml" \
  /d:sonar.scanner.scanAll=false
dotnet build Inventory.sln -c Release
dotnet-coverage collect -f xml -o coverage/coverage.xml \
  "dotnet test Inventory.sln -c Release --no-build"
dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
```

Dashboard local: `http://127.0.0.1:9000/dashboard?id=inventory-api`

## Plan de remediación

Ver [`sonar-remediation-plan.md`](./sonar-remediation-plan.md).
