    # Inventory API agent guide

This repository is the Redarbor inventory technical test. Follow this file before writing or reviewing code. The working checklist is [PLAN.md](PLAN.md). Skills live in `.cursor/skills/`.

Spec-Driven Development (GitHub Spec Kit) artifacts:

- Constitution: [.specify/memory/constitution.md](.specify/memory/constitution.md)
- Specification, clarifications, plan, tasks: [specs/001-inventory-api/](specs/001-inventory-api/)
- API docs and user manual: [docs/api.md](docs/api.md), [docs/manual.md](docs/manual.md)

When scope changes, update `spec.md` first, then `plan.md` / `tasks.md`, then `PLAN.md`.

## Stack

- .NET 8, C#, ASP.NET Core, MediatR
- PostgreSQL 16
- EF Core with Npgsql for reads only
- Dapper with Npgsql for writes only
- OAuth2 client credentials plus JWT Bearer
- ASP.NET Core rate limiter
- Docker Compose for the API and PostgreSQL

The written test prefers SQL Server. This delivery uses PostgreSQL because it is relational SQL, runs on Apple Silicon, and the request chose it. Do not add foreign keys.

## Layers

Dependency direction:

- `Inventory.Domain` references nothing outside the BCL.
- `Inventory.Application` references Domain and defines ports.
- `Inventory.Infrastructure` references Application and Domain and implements ports.
- `Inventory.Api` references Application. Controllers call MediatR. Only the composition root references Infrastructure, to register implementations.

Domain holds stock invariants. Handlers orchestrate one use case. Read repositories query with EF `AsNoTracking`. Write repositories persist with Dapper. `IInventoryMovementWriter` inserts the movement and updates `QuantityOnHand` in one transaction. Controllers map HTTP. Rate limiting stays in the API edge.

Pass a command or entity object into methods. Do not pass long parameter lists of ids and names.

Ports:

- `ICategoryReadRepository`, `ICategoryWriteRepository`
- `IProductReadRepository`, `IProductWriteRepository`
- `IInventoryMovementWriter`
- `IDbConnectionFactory`, `IDatabaseSchemaInitializer`
- `IClientCredentialAuthenticator`, `IAccessTokenIssuer`

There is no generic `IRepository<T>`.

## Clean Code rules for this test

The test lists seven rules. They override the usual C# formatting style in this repository.

1. Code and comments are English. API messages are English too.
2. No blank line between code lines, except one blank line before `return`.
3. Method names describe the action. Do not add a comment that explains the method.
4. One responsibility. Split a method that exceeds 25 lines, nests a loop or conditional more than one level, or needs a comment to be understood.
5. Read configuration from environment variables.
6. One class or one enum per file.
7. Pass objects, not loose properties, when the operation has more than one value.

## Workflow

1. Open [PLAN.md](PLAN.md) and take the first unchecked task.
2. For a business rule, write the failing test first.
3. Implement only that task, in the layer that owns it.
4. Mark the task `[x]` in `PLAN.md`.
5. Explain the change in Spanish: what was added, which layer owns it, and which task was closed.
