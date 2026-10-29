---
name: redarbor-verify
description: Builds, tests, and smoke-checks the Redarbor inventory API, then marks passing verification tasks in PLAN.md. Use when the user asks to verify, run tests, validate the build, or check rate limit and OAuth behavior.
---

# Verify the inventory API

Read [AGENTS.md](../../../AGENTS.md) and [PLAN.md](../../../PLAN.md). Run the checks below in order. Explain each result in Spanish. Mark a `verify-*` box `[x]` only when that check passes. Leave it open when it fails, and fix only the failure the user asked to fix.

## Build and tests

1. `dotnet build`
2. `dotnet test`

Close `verify-build` and `verify-test` when each command exits 0.

## Review gate

Run the `redarbor-review` checklist against the solution. Close `verify-review` when no critical finding remains.

## Smoke

With `docker compose up --build` and the values from `.env.example`:

1. `POST /oauth/token` with `grant_type=client_credentials` returns a bearer token.
2. Create a category, create a product in that category, and register an `In` movement.
3. Register an `Out` movement larger than `QuantityOnHand` and expect a rejected stock change.
4. Send more requests than `OAUTH_RATE_LIMIT_PERMIT_LIMIT` to `/oauth/token` and expect `429` with `Retry-After`.

Close `verify-smoke` when all four steps pass. Do not log the client secret or the signing key.
