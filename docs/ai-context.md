# Mavrynt — AI Context Snapshot

A compact, high-signal snapshot of the project for AI agents.
Pair this file with `AGENTS.md` (rules) and `docs/status.md` (progress).

---

## Product

Mavrynt is a modular monolith product platform built in a single repository.
Phase 1 focuses on the technical foundation: users, authentication, roles,
administrative processes, feature flags, audit, email notifications, and
preparation for Continuous Delivery.

---

## Current phase

**Phase 1 — foundation.**

- Administrative vertical slice: complete (2026-04-29).
- Notifications module: complete (2026-04-30).
- CI/CD and staging: pending.

Detailed status: `docs/status.md`. Recommended next work: `docs/next-work.md`.

---

## Completed capabilities

- BuildingBlocks: Domain primitives, Application abstractions, Infrastructure abstractions, integration contracts.
- Internal mediator (`MavryntMediator`) with Logging/Validation/Resilience/Audit/Transaction behaviors and `IValidator<TRequest>` discovery.
- Users module: aggregate, value objects, domain events, JWT login, role assignment endpoint, EF Core persistence.
- FeatureManagement module: flag CRUD with AdminApp endpoints, PostgreSQL schema `feature_management`.
- Audit module: append-only `audit_log_entries`, PostgreSQL schema `audit`, `IAuditLogWriter` consumed by FM and Notifications.
- Notifications module: DB-backed SMTP settings, three predefined templates (`auth.login_confirmation`, `auth.password_reset`, `auth.two_factor_code`), template renderer, `IEmailNotificationService`, AdminApp endpoints, default local SMTP seed (Mailpit/MailHog-friendly) on startup, and `POST /api/admin/notifications/smtp-settings/{id}/test` for per-configuration test sends (ADR-024).
- Auth UI: `@mavrynt/auth-ui` package with login/register, `AuthService` port, and i18n (PL/EN).
- Cross-app URL helper: `resolveAppUrls()` in `@mavrynt/config/app-urls`.
- Backend tests: architecture tests (NetArchTest), domain/application unit tests, infrastructure + host integration tests against real PostgreSQL via Testcontainers.
- Admin SPA shell: protected routes, dashboard, feature flags page, SMTP settings page.
- Marketing landing: full lifecycle through accessibility (WCAG 2.1 AA), performance, and Playwright e2e smoke.

---

## Pending capabilities (Phase 1)

- CI/CD pipeline (no `.github/workflows/` exists yet).
- Staging environment wiring.
- Production-grade secret handling (`PassThroughSecretProtector` is dev-only).
- Deployment automation (`build/`, `deploy/`, `scripts/` are empty).
- Smoke tests in pipeline.
- Audit module test project (currently covered only by architecture tests).
- Admin: list-users endpoint (frontend Users page exists but backend exposes only role-assignment PATCH).

---

## Architecture overview

- **Style:** modular monolith (ADR-001), single repo (ADR-002).
- **Hosts:** `Mavrynt.Api` (user-facing), `Mavrynt.AdminApp` (admin-facing), `Mavrynt.AppHost` (Aspire local orchestration), `Mavrynt.ServiceDefaults` (shared technical defaults).
- **Layers per module:** `Domain` ← `Application` ← `Infrastructure`. Hosts wire modules.
- **Mediator:** internal `MavryntMediator` (ADR-020). No MediatR.
- **Persistence:** EF Core 9 + Npgsql + PostgreSQL. Each module owns its own PostgreSQL schema (`users`, `feature_management`, `audit`, `notifications`).
- **Auth:** JWT Bearer (HS256) issued by `Mavrynt.Api`, validated by both API hosts. `AdminOnly` policy gates admin endpoints.
- **Cross-cutting:** `IAuditService` (BuildingBlocks, auth events) and `IAuditLogWriter` (Audit.Application, admin events). They are intentionally separate.
- **Allowed cross-module application dependencies:** FM→Audit and Notifications→Audit. Anything else needs a new ADR.
- **Frontend:** three SPAs hosted under Aspire SpaProxy projects, sharing `@mavrynt/*` workspace packages.

---

## Backend module map

| Module | Schema | Key abstractions | Notes |
|---|---|---|---|
| Users | `users` | `User`, `IUserRepository`, JWT issuance | First foundational module. ADR-007. |
| FeatureManagement | `feature_management` | `FeatureFlag`, flag CRUD commands/queries | AdminApp-only; depends on Audit (ADR-022). |
| Audit | `audit` | `AuditLogEntry`, `IAuditLogWriter` | Append-only. Distinct from `IAuditService`. |
| Notifications | `notifications` | `SmtpSettings`, `EmailTemplate`, `IEmailNotificationService`, `IEmailModel`, `ISecretProtector` | BCL `SmtpClient`. ADR-023. |

`BuildingBlocks.{Domain,Application,Infrastructure,Contracts}` provide foundation types (`Result`, `Error`, `Entity<TId>`, `AggregateRoot<TId>`, `IRepository<TEntity, TId>`, `IUnitOfWork`, mediator and behavior contracts).

---

## Frontend app map

Disk paths are inside the Aspire SpaProxy host folders:

| App | Path | Purpose |
|---|---|---|
| User-facing | `src/frontend/Mavrynt.Web.App/mavrynt-web/` | Public end-user SPA |
| Admin | `src/frontend/Mavrynt.Web.Admin/mavrynt-admin/` | Admin shell, protected routes, dashboard, flags, SMTP |
| Landing | `src/frontend/Mavrynt.Web.Landing/mavrynt-landing/` | Marketing landing (decoupled from backend) |

Shared workspace packages under `src/frontend/shared/`:
`@mavrynt/auth-ui`, `@mavrynt/config`, `@mavrynt/design-tokens`, `@mavrynt/eslint-config`, `@mavrynt/tsconfig-base`, `@mavrynt/ui`.

Per-SPA scripts: `npm run dev / build / test / typecheck` (and `test:e2e` for landing).

---

## Important conventions

- Domain returns `Result` / `Result<T>`; never throws for validation failures.
- Endpoints inject `IMediator`, not concrete handlers.
- DTOs are flat records and never expose domain types.
- Validation goes in `IValidator<TRequest>`; business invariants stay in Domain.
- `IAuditableRequest`, `ITransactionalRequest`, `IResilientRequest` are the only mechanisms for cross-cutting handler concerns.
- Migrations are written manually following the FeatureManagement pattern.
- `IDateTimeProvider` is registered with `TryAddSingleton` in module Infrastructure setup so duplicate registration is safe.
- Frontend uses `resolveAppUrls()` for cross-SPA navigation; do not read `VITE_*_URL` env vars directly (ADR-016).

---

## Test strategy

Three layers, all run from `dotnet test`:

1. **Architecture tests** (NetArchTest + project-reference checks) — protect modular boundaries and dependency direction.
2. **Unit tests** — domain primitives and command/query handlers with in-memory doubles.
3. **Integration tests** — real PostgreSQL containers via Testcontainers, covering Infrastructure and host smoke scenarios.

Frontend testing: Vitest (unit/integration) and Playwright (landing e2e).

---

## Known technical decisions (cross-references)

- ADR-001 — Modular monolith.
- ADR-005 — Layered Domain/Application/Infrastructure per module.
- ADR-006 — Shared BuildingBlocks projects.
- ADR-015 — Marketing landing decoupled from backend.
- ADR-016 — Cross-app URL resolution via `@mavrynt/config/app-urls`.
- ADR-017 — `@mavrynt/auth-ui` as a separate package from `@mavrynt/ui`.
- ADR-018 — BuildingBlocks baseline without mediator lock-in.
- ADR-019 — Users module Domain + Application baseline.
- ADR-020 — Internal mediator (`MavryntMediator`) accepted; **supersedes the deferral in ADR-018**.
- ADR-021 — Backend test pyramid (architecture + unit + Testcontainers integration).
- ADR-022 — Phase 1 admin slice: role assignment, FeatureManagement, persistent Audit.
- ADR-023 — Notifications module: DB-backed SMTP, templates, `IEmailNotificationService`.
- ADR-024 — Default local SMTP seed and per-configuration test email (foundation for password reset by email).
- ADR-025 — Pipeline-owned commit: every mutating command implements `ITransactionalRequest`; `TransactionBehavior` resolves all registered `IUnitOfWork` instances and saves each on success. Repositories no longer self-save inside `AddAsync`.

Full ADRs: `docs/decisions.en.md`. Detailed implementation notes for some areas live in `docs/adr/` — see the numbering note at the top of `docs/decisions.en.md`.

---

## Current risks and gaps

- **No CI/CD.** Build/test/deploy are local-only. `build/`, `deploy/`, `scripts/`, and `.github/` are empty or absent.
- **No production secret store.** `PassThroughSecretProtector` is a dev placeholder; SMTP passwords would be stored as-is.
- **`actorUserId` not propagated to admin audit entries.** All admin audit entries are written with `actorUserId: null` (ADR-022 deferred).
- **Audit module has no test project.** Only architecture tests cover its boundaries.
- **Admin list-users endpoint missing.** Backend exposes only role-assignment PATCH; the admin SPA cannot list users without it.
- **No `pnpm-workspace.yaml` at repo root.** `npm` scripts work per-SPA; the previously documented `pnpm --filter ...` commands rely on workspace config that does not currently exist at the root.
- **Two parallel ADR numbering schemes.** `docs/decisions.en.md` ADR-020..023 and `docs/adr/ADR-020..023-*.md` cover different topics with the same numbers. Treat `docs/decisions.en.md` as canonical.

---

## Next recommended implementation areas

In priority order:

1. CI/CD pipeline (GitHub Actions) — restore, build, test, artifact, smoke.
2. Staging environment wiring (config, secrets, deployment target).
3. Production-grade `ISecretProtector` implementation.
4. Audit module test project to close the test-coverage gap.
5. List-users admin endpoint to unblock the admin Users page.
6. Propagate `ICurrentUserContext` actor to `IAuditLogWriter`.
7. Renumber or clearly mark `docs/adr/*.md` to remove the numbering collision.

Detailed plan: `docs/next-work.md`.
