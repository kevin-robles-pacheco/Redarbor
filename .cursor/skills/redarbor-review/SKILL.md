---
name: redarbor-review
description: Reviews Redarbor inventory changes against Clean Code, SOLID, layer boundaries, English identifiers, and the EF-read Dapper-write split. Use when the user asks for a code review, a checklist review, or before marking a PLAN.md task complete.
---

# Review inventory changes

Read [AGENTS.md](../../../AGENTS.md) and inspect the current diff. Report findings in Spanish. Do not mark `verify-*` tasks in `PLAN.md`.

## Checklist

- [ ] Identifiers, comments, routes, and JSON names are English
- [ ] One class or enum per file
- [ ] Methods stay at or under 25 lines and nest control flow at most one level
- [ ] The only blank line inside a member is the line before `return`
- [ ] Methods receive an object when they need several values
- [ ] Configuration is read from environment variables
- [ ] Domain has no EF, Dapper, Npgsql, or ASP.NET reference
- [ ] Handlers contain no SQL and no `HttpContext`
- [ ] Reads use an EF repository; writes use a Dapper repository
- [ ] The movement writer updates stock and inserts the movement in one transaction
- [ ] SQL has primary keys and no foreign keys
- [ ] Controllers delegate to MediatR and return application DTOs
- [ ] Logs omit secrets, tokens, and connection strings

## Report

Group findings as:

- Critical: breaks a test rule or a layer boundary
- Suggestion: improves clarity and still follows the rules
- Clear: checked items that hold

A critical finding stays a `- [ ]` task. Do not close it in this skill.
