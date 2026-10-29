---
name: redarbor-implement
description: Implements the next unchecked Redarbor inventory task from PLAN.md using hexagonal layers, English identifiers, EF Core reads, Dapper writes, and the seven Clean Code rules. Use when implementing, coding, or continuing the inventory API, a handler, entity, repository, endpoint, or Docker setup in this repository.
---

# Implement the next inventory task

## Read first

1. Read [AGENTS.md](../../../AGENTS.md). Those rules override general C# formatting in this repository.
2. Read [PLAN.md](../../../PLAN.md).

## Steps

1. Choose the first task whose box is `- [ ]`. Do not skip ahead.
2. If the task encodes a business rule, write the failing test first, then the production code.
3. Implement only that task.
4. Put the code in the layer that owns it. Domain has no I/O. Handlers call ports. EF reads and Dapper writes stay in Infrastructure. Controllers call MediatR.
5. Keep identifiers in English, one class or enum per file, methods at or under 25 lines, and a blank line only before `return`.
6. Change that task box to `- [x]` in `PLAN.md`.
7. Explain the change in Spanish: what was added, which layer owns it, and which task id was closed.

## Stop

Stop after one task unless the user asked to continue. Do not start the API, Docker, or verification tasks while an earlier box is still open.
