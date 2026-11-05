# Coverage progress — after Fase A

**Fecha**: 2026-10-29 (America/New_York)  
**Herramienta**: `dotnet-coverage` (line coverage)

## Antes → Después

| Ensamblado | Antes | Después | Meta plan |
|------------|-------|---------|-----------|
| Domain | 97.0% | **99.4%** | ≥ 95% |
| Application | 63.0% | **96.7%** | ≥ 85% |
| Api | 6.2% | **84.0%** | ≥ 70% |
| Infrastructure | 25.6% | **63.6%** | ≥ 60% |

## Qué se añadió

- Application: `CategoryQueryHandlerTests`, `ProductQueryHandlerTests`
- Api: `InventoryApiFactory` + `InMemoryInventoryStore` + tests de integración HTTP
- Infrastructure: tests EF InMemory para categorías, productos y movimientos; `InventoryDbContext` admite `allowWrites` solo para tests
- `Program` parcial para `WebApplicationFactory`

## Tests

68 tests passed (Domain 21, Application 22, Api 14, Infrastructure 11).

## Pendiente opcional

- Cubrir writers Dapper con SQL real (requeriría Postgres o ampliar alcance a Testcontainers)
- Subir Infrastructure hacia 80%+
