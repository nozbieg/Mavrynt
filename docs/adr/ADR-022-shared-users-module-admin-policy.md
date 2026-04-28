# ADR-022 — Shared Users Module Across Api and AdminApp

**Date:** 2026-04-28  
**Status:** Accepted

---

## Context

`Mavrynt.Api` (user-facing) and `Mavrynt.AdminApp` (admin-facing) are two separate ASP.NET Core hosts within the same monolith. Both need user identity, authentication, and role-based authorization. The question is whether to duplicate user logic or share it.

## Decision

Both backend services reference the same **`Mavrynt.Modules.Users.Application`** and **`Mavrynt.Modules.Users.Infrastructure`** project assemblies. There is no code duplication.

Differentiation between services is achieved through **authorization policies**, not separate code paths:

- `Mavrynt.Api` registers the `AdminOnly` policy but primarily serves `User`-role endpoints
- `Mavrynt.AdminApp` registers the same `AdminOnly` policy and gates all its endpoints with `RequireAuthorization("AdminOnly")` by default

The `AuthServiceCollectionExtensions` class is defined independently in each host's `DependencyInjection` namespace (not a shared library class) because host-level DI wiring is the responsibility of the host project. The configuration shape (`Jwt` section) is identical.

`UserRole` is a first-class enum on the `User` aggregate and is included in the JWT claim. No separate role store is required at this stage.

## Consequences

**Positive:**
- Single source of truth for user domain logic — no synchronization problem
- Adding a new user capability once makes it available to both hosts immediately
- Minimal surface area: admin access is enforced by JWT claim, not a separate identity system

**Negative / Trade-offs:**
- Both services share the same `UsersDbContext` and database connection pool; horizontal scaling requires awareness of connection limits
- `Mavrynt.AdminApp` depends on `Mavrynt.Modules.Users.Infrastructure` (EF Core, Npgsql) even if it only needs query-side access; a CQRS read-only context could reduce this in future
- Admin token issuance happens via `Mavrynt.Api`; `Mavrynt.AdminApp` validates but does not issue tokens

**Future work:**
- If `Mavrynt.AdminApp` needs admin-specific user management commands, add them to `Users.Application` — not a new module
- A dedicated admin token issuer endpoint (`POST /api/admin/auth/login`) can be added to `AdminApp` if separation of auth flows is required
