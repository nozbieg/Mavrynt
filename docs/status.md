# Mavrynt — Project Status

Single source of truth for phase progress. Update when scope changes.
For high-signal context, pair with `docs/ai-context.md`.

---

## Phase

**Phase 1 — foundation.** In progress. Administrative slice and Notifications
shipped; remaining items are CI/CD and staging.

---

## Completed (Phase 1)

### Backend foundation
- [x] BuildingBlocks: Domain, Application, Infrastructure, Contracts.
- [x] `Result` / `Error` model in `BuildingBlocks.Domain`.
- [x] Internal `MavryntMediator` with Logging / Validation / Resilience / Audit / Transaction behaviors (ADR-020).
- [x] `IValidator<TRequest>` discovery via `AddMavryntMediator`.
- [x] `IUnitOfWork`, `IRepository<TEntity, TId>` abstractions.

### Users module
- [x] Aggregate (`User`) with value objects (`UserId`, `Email`, `PasswordHash`, `UserDisplayName`).
- [x] EF Core 9 + Npgsql persistence (`users` schema).
- [x] JWT Bearer login (`Mavrynt.Api`) with HS256 (ADR-020 / `docs/adr/ADR-020-jwt-bearer-authentication.md`).
- [x] Role assignment endpoint `PATCH /api/admin/users/{userId}/role` (`AdminOnly`).
- [x] Auth event audit via `IAuditService` (BuildingBlocks).

### FeatureManagement module
- [x] `FeatureFlag` aggregate (key, name, description, enabled, timestamps).
- [x] CRUD commands: Create / Update / Toggle.
- [x] Queries: List, Get-by-key.
- [x] AdminApp endpoints under `/api/admin/feature-flags*` (`AdminOnly`).
- [x] EF Core migrations, `feature_management` schema.
- [x] Audit emission via `IAuditLogWriter` (cross-module FM→Audit, ADR-022).

### Audit module
- [x] `AuditLogEntry` append-only record.
- [x] PostgreSQL `audit` schema and table.
- [x] `IAuditLogWriter` abstraction in `Audit.Application`.
- [x] EF Core migrations.

### Notifications module (ADR-023)
- [x] `SmtpSettings` aggregate; exactly-one-active provider invariant.
- [x] `EmailTemplate` storage with three predefined keys (`auth.login_confirmation`, `auth.password_reset`, `auth.two_factor_code`).
- [x] `EmailTemplateRenderer` (Application layer, no infrastructure deps).
- [x] `IEmailNotificationService.SendAsync<TModel>(...)` cross-module API.
- [x] `IEmailModel` typed model contract.
- [x] `SmtpEmailSender` using BCL `System.Net.Mail.SmtpClient`.
- [x] `NotificationsStartupService` for migration + idempotent template seeding.
- [x] AdminApp endpoints for SMTP settings and templates (`AdminOnly`).
- [x] `ISecretProtector` extension point (default = dev-only `PassThroughSecretProtector`).

### Backend tests
- [x] `tests/backend/Mavrynt.Architecture.Tests` — modular dependency rules.
- [x] `tests/backend/Mavrynt.BuildingBlocks.Domain.Tests`.
- [x] `tests/Mavrynt.Modules.Users.Domain.Tests`.
- [x] `tests/Mavrynt.Modules.Users.Application.Tests`.
- [x] `tests/Mavrynt.Modules.Users.Infrastructure.Tests` (Testcontainers).
- [x] `tests/Mavrynt.Modules.FeatureManagement.Domain.Tests`.
- [x] `tests/Mavrynt.Modules.FeatureManagement.Application.Tests`.
- [x] `tests/Mavrynt.Modules.FeatureManagement.Infrastructure.Tests` (Testcontainers).
- [x] `tests/Mavrynt.Modules.Notifications.Domain.Tests`.
- [x] `tests/Mavrynt.Modules.Notifications.Application.Tests`.
- [x] `tests/Mavrynt.Modules.Notifications.Infrastructure.Tests` (Testcontainers).
- [x] `tests/Mavrynt.Api.IntegrationTests`.
- [x] `tests/Mavrynt.AdminApp.IntegrationTests` (role assignment, flags, notifications).

### Frontend
- [x] Three SPAs scaffolded under Aspire SpaProxy hosts (`Mavrynt.Web.App`, `Mavrynt.Web.Admin`, `Mavrynt.Web.Landing`).
- [x] Shared workspace packages: `@mavrynt/{auth-ui,config,design-tokens,eslint-config,tsconfig-base,ui}`.
- [x] Auth UI in `@mavrynt/auth-ui` (login + register, `AuthService` port, PL/EN i18n).
- [x] Cross-SPA URL helper `resolveAppUrls()` in `@mavrynt/config/app-urls` (ADR-016).
- [x] Marketing landing — full lifecycle: foundation → content → WCAG 2.1 AA → performance → Vitest + Playwright e2e.
- [x] Admin SPA shell — login/register/protected routes, dashboard, feature flags page, SMTP settings page, settings page.

### Documentation
- [x] Architecture document (`docs/architecture.en.md`).
- [x] Repository structure (`docs/repo-structure.en.md`).
- [x] ADR log (`docs/decisions.en.md`).
- [x] AI agent quick start (`AGENTS.md`).
- [x] AI context snapshot (`docs/ai-context.md`).
- [x] Status (this file).
- [x] Next-work plan (`docs/next-work.md`).

---

## In progress

- [ ] No active work item recorded. The next pickup is in `docs/next-work.md`.

---

## Pending (Phase 1)

### CI/CD
- [ ] GitHub Actions workflow for backend: restore → build → test (with Docker for Testcontainers) → artifact.
- [ ] Workflow for frontend: per-SPA install → typecheck → lint → test → build artifact.
- [ ] Landing e2e (Playwright Chromium) in CI.
- [ ] Branch protection rules (build + tests must pass).
- [ ] Pipeline-driven smoke test against a deployed staging slot.

### Staging environment
- [ ] Staging hosting target chosen and provisioned.
- [ ] `appsettings.Staging.json` for `Mavrynt.Api` and `Mavrynt.AdminApp`.
- [ ] Connection strings, JWT signing key, and SMTP credentials provisioned via secret store.
- [ ] Database migration strategy in pipeline (manual command vs. startup-applied).
- [ ] Static hosting target for `mavrynt-landing` (CDN / object storage).
- [ ] Hosting for `mavrynt-web` and `mavrynt-admin`.

### Production readiness
- [ ] Replace `PassThroughSecretProtector` with a real `ISecretProtector` (DPAPI / Key Vault / equivalent).
- [ ] Health checks wired into `Mavrynt.ServiceDefaults` + exposed in CI smoke.
- [ ] OpenTelemetry exporter target chosen (no exporter is wired beyond defaults).

### Backend gaps
- [ ] `Modules.Audit` test project (currently no `Audit.*.Tests` exists; coverage is only via `Mavrynt.Architecture.Tests`).
- [ ] List-users admin endpoint (admin SPA cannot list users without it; only role assignment is exposed).
- [ ] Propagate actor `userId` from `ICurrentUserContext` into `IAuditLogWriter.WriteAsync()` (currently `null`, deferred in ADR-022).

### Documentation gaps
- [ ] Resolve ADR numbering collision: `docs/decisions.en.md` ADR-020..023 vs. `docs/adr/ADR-020..023-*.md` cover different topics with the same numbers. Recommended: rename the `docs/adr/*` files (e.g., `ADR-020 → ADR-D01`) without losing history.

---

## Deferred

- Microservice extraction (not a Phase 1 goal; ADR-001).
- Polly-based resilience (the `ResilienceBehavior` is a pass-through hook).
- FluentValidation / DataAnnotations migration (`IValidator<TRequest>` is the internal contract).
- User-facing feature-flag read endpoints (flags are AdminApp-only at this stage).
- Full RBAC beyond `Admin` / `User` roles.
- MailKit or SaaS email (replace `SmtpClient` only when needed; the `IEmailSender` seam exists).
- Redis / RabbitMQ / Kafka activation (reserved in architecture only — ADR-012).

---

## Known gaps and risks

- **No `.github/` directory.** No CI runs today; everything is local.
- **`build/`, `deploy/`, `scripts/` are empty.** Reserved locations only; no automation lives there.
- **No root `pnpm-workspace.yaml` or root `package.json`.** Per-SPA `npm` scripts work; the `pnpm --filter` commands documented in older READMEs assume workspace config that is not present.
- **Dev-only secret protection.** Real SMTP passwords must not be stored until `ISecretProtector` is replaced.
- **Two ADR numbering schemes.** Treat `docs/decisions.en.md` as canonical; the standalone files in `docs/adr/` are detailed implementation notes.

---

## Suggested next steps

1. Stand up CI (`.github/workflows/backend.yml` + `.github/workflows/frontend.yml`) — see `docs/next-work.md`.
2. Provision staging and wire secrets.
3. Add an `Audit` module test project to close the test gap.
4. Add the list-users admin endpoint.
5. Replace `PassThroughSecretProtector`.
6. Renumber the `docs/adr/*` files to remove the collision with `docs/decisions.en.md`.
