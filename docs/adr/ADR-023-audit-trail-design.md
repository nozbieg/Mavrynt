# ADR-023 — Audit Trail Design

**Date:** 2026-04-28  
**Status:** Accepted

---

## Context

Auth events (registration, login success/failure, password change) must be durably recorded for security and compliance purposes. The audit trail needs to be queryable (by user, event type, time range) without requiring schema changes for every new event type.

## Decision

Implement a **minimal EF Core audit trail** using a dedicated `AuditEvent` entity stored in `users.audit_events`.

Design:

- `AuditEvent` is an infrastructure entity (not a domain entity) owned by `Mavrynt.Modules.Users.Infrastructure`
- The `IAuditService` abstraction lives in `Mavrynt.BuildingBlocks.Application` (cross-cutting concern); the EF Core implementation `EfAuditService` is in Infrastructure
- Application handlers call `IAuditService.RecordAsync(AuditEntry)` — they have no dependency on EF Core
- `AuditEntry` is an immutable record with typed fields (`EventType`, `OccurredAt`, `UserId?`, `Email?`, `IpAddress?`, `UserAgent?`, `Metadata?`)
- `Metadata` is stored as PostgreSQL `jsonb` — event-specific context without per-type schema migrations
- Event type constants are centralized in `AuditEventTypes` (`user_registered`, `login_success`, `login_failed`, `password_changed`, `email_changed`)
- Indexes on `occurred_at`, `user_id`, `event_type` support the primary query patterns

Schema:

```
users.audit_events
  id           uuid         PK
  event_type   text         NOT NULL, indexed
  occurred_at  timestamptz  NOT NULL, indexed
  user_id      uuid         NULL, indexed
  email        text         NULL
  source       text         NULL
  ip_address   text         NULL
  user_agent   text         NULL
  metadata     jsonb        NULL
```

Limitations at current phase:
- `IpAddress` and `UserAgent` are null in command handlers (no HTTP context access at that layer). Endpoint-level enrichment can be added later via middleware or explicit handler injection.
- No audit event pagination or admin UI yet.

## Consequences

**Positive:**
- Clean abstraction: handlers depend on `IAuditService`, not EF Core
- `jsonb` metadata column avoids migrations for new event-specific fields
- Indexes on the three primary query dimensions keep reporting queries fast
- Centralized `AuditEventTypes` constants prevent magic string scatter

**Negative / Trade-offs:**
- `EfAuditService` writes synchronously within the same DB transaction scope as the command handler — a failing audit write can roll back a successful business operation (acceptable trade-off for correctness)
- No event streaming / outbox pattern; audit events are fire-and-forget within the request. High-throughput scenarios may need an async outbox

**Future work:**
- Enrich `IpAddress` / `UserAgent` at the endpoint layer via `HttpContext` injection or middleware
- Add admin query endpoint for audit log browsing
- Consider outbox pattern for audit events if decoupling from the write transaction is needed
