# ADR-021 — EF Core + PostgreSQL Persistence for Users Module

**Date:** 2026-04-28  
**Status:** Accepted

---

## Context

The Users module requires durable persistence for user accounts and an audit trail. The project is a .NET 10 modular monolith using Aspire for local orchestration. A relational database is appropriate for this domain.

## Decision

Use **EF Core 9 + Npgsql** with PostgreSQL as the persistence backend for the Users module.

Key design choices:

- `UsersDbContext` lives in `Mavrynt.Modules.Users.Infrastructure` and is scoped to that module. Other modules must not reference it directly.
- Domain value objects (`UserId`, `Email`, `PasswordHash`, `UserDisplayName`) are mapped using EF Core value converters. SQL queries use `.Value` access patterns (e.g. `u.Email.Value == emailValue`) to ensure EF Core can translate them to SQL predicates.
- `UserStatus` and `UserRole` enums are stored as strings (`HasConversion<string>()`) for readability and schema stability.
- Schema: all Users module tables live in the `users` PostgreSQL schema (`users.users`, `users.audit_events`).
- `AuditEvent.metadata` is stored as `jsonb` to support flexible, queryable audit metadata without schema migrations per event type.
- Migrations are applied manually during this phase: `dotnet ef database update --project Mavrynt.Modules.Users.Infrastructure --startup-project Mavrynt.Api`.
- The `IUnitOfWork` abstraction (`BuildingBlocks.Application`) is implemented by `UsersDbContext`. Repository methods call `SaveChangesAsync` directly for now; the `IUnitOfWork` pattern is in place for future cross-repository transactional flows.

## Consequences

**Positive:**
- EF Core's value converter support maps cleanly onto DDD value objects
- PostgreSQL `jsonb` gives flexible audit metadata querying without extra migrations
- Schema-per-module convention isolates table ownership at the DB level
- Aspire `AddPostgres` / `AddDatabase` wiring provides connection string injection in dev

**Negative / Trade-offs:**
- EF Core value converters prevent some LINQ operations from translating to SQL (e.g., complex expressions over value object internals); use `.Value` extraction before filtering
- Manual migrations require developer discipline; automatic on-startup migration deferred
- Shared PostgreSQL instance in dev means all modules land in the same server (acceptable for monolith phase)

**Future work:**
- Automated migration runner on startup (Aspire lifecycle hook or `IHostedService`)
- Per-module migration assembly discovery if a second module adds its own `DbContext`
