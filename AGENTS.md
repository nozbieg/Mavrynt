# AGENTS.md — AI Agent Quick Start

**Read this file first.** It is the canonical, token-efficient entry point for any AI agent
working on Mavrynt. Read deeper documents only when the task requires it.

---

## 1. Read first / context strategy

For most tasks, only three files are needed:

1. `AGENTS.md` (this file) — rules and routing.
2. `docs/ai-context.md` — compact project snapshot.
3. `docs/status.md` — what is done, in progress, pending.

Read deeper docs only when the task demands it (see section 11).

---

## 2. Project at a glance

- **Name:** Mavrynt — modular monolith product platform in a single repository.
- **Backend:** .NET 10, C# nullable + implicit usings, EF Core 9, PostgreSQL, Aspire AppHost for local orchestration, internal `MavryntMediator` (no MediatR), JWT Bearer auth.
- **Frontend:** React 19 + Vite 8 + TypeScript, Tailwind v4, three SPAs hosted under Aspire SpaProxy projects.
- **Languages:** PL/EN docs exist; **English is the canonical technical language** going forward.
- **Repo style:** documentation lives next to code, ADRs are kept in `docs/decisions.en.md`.

---

## 3. Current status (summary)

- **Phase 1 Admin vertical slice:** complete (2026-04-29) — role assignment, FeatureManagement, persistent Audit, AdminApp endpoints, full backend test pyramid.
- **Notifications/email module:** complete (2026-04-30) — DB-backed SMTP, templates, `IEmailNotificationService`, AdminApp endpoints.
- **Admin SPA shell:** complete — login/register/protected routes, dashboard, feature flags, SMTP settings, settings.
- **Pending Phase 1:** CI/CD pipeline, staging environment, production secret handling, deployment automation.

Authoritative status: `docs/status.md`.

---

## 4. Architecture rules (must not violate)

- **Layers per module:** `Domain` → `Application` → `Infrastructure`. Dependencies point inward only.
- **Domain** must not reference Infrastructure, hosts, EF Core, ASP.NET, or any framework.
- **Application** must not reference Infrastructure or hosts.
- **Hosts** (`Mavrynt.Api`, `Mavrynt.AdminApp`) compose modules; they must not contain business logic.
- **Cross-module application dependencies** are forbidden, except the explicitly approved ones:
  - `FeatureManagement.Application` → `Audit.Application` (`IAuditLogWriter`) — ADR-022.
  - `Notifications.Application` → `Audit.Application` (`IAuditLogWriter`) — ADR-023.
  - Any new cross-module dependency requires a new ADR.
- **Frontend** must not reference backend projects. Integration is through HTTP API only.
- **Marketing landing** (`mavrynt-landing`) must remain decoupled from the backend (ADR-015) — integrations only via ports/adapters.

---

## 5. Mediator rules (ADR-020)

- Use the internal `MavryntMediator`. **Do not add MediatR** or any external mediator.
- Endpoints inject `IMediator` and call `mediator.SendAsync(command)` / `SendAsync(query)`.
- Handlers implement `ICommandHandler<...>` or `IQueryHandler<...>` and return `Result` / `Result<T>`.
- Validation lives in `IValidator<TRequest>`, not in handlers. Validators are auto-discovered by `AddMavryntMediator`.
- Pipeline behaviors run in this order: Logging → Validation → Resilience → Audit → Transaction.
- Optional behavior markers:
  - `IAuditableRequest` — emits an audit entry via `IAuditService`.
  - `ITransactionalRequest` — `IUnitOfWork.SaveChangesAsync()` after success.
  - `IResilientRequest` — retry/timeout hook (do not apply to non-idempotent commands).
- Never duplicate logging, audit, or transaction logic inside a handler — pipeline already does it.
- Never serialize the full request to logs (passwords, tokens).

---

## 6. Notifications rules (ADR-023)

- Send email through `IEmailNotificationService.SendAsync(...)`. Do not use `SmtpClient` directly.
- Templates: `EmailTemplateKey.LoginConfirmation`, `PasswordReset`, `TwoFactorCode`. Each has an `IEmailModel` type.
- Never log full email bodies. Never return SMTP passwords in DTOs.
- `PassThroughSecretProtector` is a dev-only placeholder; replace with real encryption before production.

---

## 7. Module map (backend)

| Module | Status | Notes |
|---|---|---|
| `BuildingBlocks.Domain/Application/Infrastructure/Contracts` | complete | Foundational types, mediator, behaviors. |
| `Modules.Users.*` | complete | Aggregates, JWT login, role assignment. |
| `Modules.FeatureManagement.*` | complete | Flag CRUD, AdminApp-only endpoints. |
| `Modules.Audit.*` | complete | Append-only `audit_log_entries` (PostgreSQL `audit` schema). |
| `Modules.Notifications.*` | complete | SMTP settings, templates, email send. |

Use `Mavrynt.Modules.Users.*` as the reference template when adding a new module.

---

## 8. Frontend map

Actual disk layout:

```
src/frontend/
├── Mavrynt.Web.App/      → ASP.NET host (SpaProxy) wrapping mavrynt-web/
├── Mavrynt.Web.Admin/    → ASP.NET host (SpaProxy) wrapping mavrynt-admin/
├── Mavrynt.Web.Landing/  → ASP.NET host (SpaProxy) wrapping mavrynt-landing/
└── shared/
    ├── auth-ui/          @mavrynt/auth-ui     (login/register, AuthService port)
    ├── config/           @mavrynt/config      (env, app-urls, i18n bootstrap)
    ├── design-tokens/    @mavrynt/design-tokens
    ├── eslint-config/    @mavrynt/eslint-config
    ├── tsconfig-base/    @mavrynt/tsconfig-base
    └── ui/               @mavrynt/ui          (presentational primitives)
```

SPA folders:

| App | Path |
|---|---|
| User-facing | `src/frontend/Mavrynt.Web.App/mavrynt-web/` |
| Admin | `src/frontend/Mavrynt.Web.Admin/mavrynt-admin/` |
| Landing | `src/frontend/Mavrynt.Web.Landing/mavrynt-landing/` |

Per-SPA scripts: `npm run dev`, `npm run build`, `npm run test`, `npm run typecheck`.
The landing additionally provides `test:e2e` (Playwright). Each SPA has its own `package.json`.

---

## 9. Test map

| Project | Layer |
|---|---|
| `tests/backend/Mavrynt.Architecture.Tests` | NetArchTest dependency rules |
| `tests/backend/Mavrynt.BuildingBlocks.Domain.Tests` | Domain primitives |
| `tests/Mavrynt.Modules.Users.{Domain,Application,Infrastructure}.Tests` | Module unit + integration |
| `tests/Mavrynt.Modules.FeatureManagement.{Domain,Application,Infrastructure}.Tests` | Module unit + integration |
| `tests/Mavrynt.Modules.Notifications.{Domain,Application,Infrastructure}.Tests` | Module unit + integration |
| `tests/Mavrynt.Api.IntegrationTests` | API host smoke + integration |
| `tests/Mavrynt.AdminApp.IntegrationTests` | AdminApp endpoints (roles, flags, notifications) |

> **Gap:** `Modules.Audit` has no dedicated test project; its boundaries are covered only by architecture tests. See `docs/next-work.md`.

Integration tests use **Testcontainers** with real PostgreSQL — **Docker is required**.

---

## 10. Validation commands

**Backend (run from repo root):**

```bash
dotnet restore Mavrynt.sln
dotnet build Mavrynt.sln --no-restore
dotnet test Mavrynt.sln --no-build
```

**Local stack via Aspire (backend + all SPAs):**

```bash
dotnet run --project src/backend/Mavrynt.AppHost/Mavrynt.AppHost.csproj
```

**Per-SPA (run from the SPA folder, e.g., `src/frontend/Mavrynt.Web.Landing/mavrynt-landing/`):**

```bash
npm run dev
npm run build
npm run test
npm run typecheck
npm run test:e2e        # landing only
```

> Tests using Testcontainers require Docker.

---

## 11. Task routing — what to read for what task

| Task | Read these files |
|---|---|
| Quick context | `AGENTS.md`, `docs/ai-context.md` |
| Current progress / what to do next | `docs/status.md`, `docs/next-work.md` |
| Add/change a backend module | `docs/architecture.en.md` (sections 5–7), `docs/repo-structure.en.md`, `Modules.Users.*` as template |
| Architecture decision change | `docs/decisions.en.md` — append a new ADR; do not edit history |
| New endpoint in `Mavrynt.Api` or `AdminApp` | Existing endpoint in same host as a template, then handler under the relevant `*.Application` |
| Frontend SPA work | The SPA's own `README.md`, `src/frontend/shared/*/README.md` for shared packages |
| Auth UI work | `docs/auth-ui.en.md`, `src/frontend/shared/auth-ui` |
| Cross-SPA URL resolution | ADR-016, `@mavrynt/config/app-urls` |
| CI/CD or staging | `docs/status.md`, `docs/next-work.md` |

---

## 12. Forbidden actions

- Adding MediatR, FluentValidation, AutoMapper, or any external mediator/validator.
- Putting business logic in `Program.cs` or any host project.
- Referencing Infrastructure from Domain, or Infrastructure from Application.
- Adding a cross-module Application reference outside the two approved ones (FM→Audit, Notifications→Audit).
- Frontend importing from `src/backend/*`.
- Logging full email bodies, request bodies with credentials, or returning SMTP passwords in DTOs.
- Using `SmtpClient` directly outside `Notifications.Infrastructure`.
- Bypassing `IMediator` in endpoints (no direct handler injection in HTTP layer).
- Editing `Mavrynt.AppHost` to add domain logic — it only wires processes.

---

## 13. Documentation update rules

- Add new architecture decisions as a new ADR in `docs/decisions.en.md`. Never edit or delete past ADRs; mark them `Superseded` if replaced.
- Keep `docs/status.md` current — it is the single source of truth for phase progress.
- Update `docs/ai-context.md` only when a high-signal fact changes (new module, completed phase, new architecture rule).
- The PL counterparts (`*.pl.md`) are non-canonical summaries; only update them if you are sure they should remain consistent.

---

## 14. Before editing — checklist

- [ ] Did I read `AGENTS.md`, `docs/ai-context.md`, and the matching deeper doc for this task?
- [ ] Do I know which module / host / SPA the change belongs in?
- [ ] Will my change cross a layer or module boundary that requires an ADR?
- [ ] Am I using `IMediator`, `IEmailNotificationService`, `IAuditLogWriter`, etc. instead of bypassing them?

## 15. Before finishing — checklist

- [ ] Backend build clean: `dotnet build Mavrynt.sln`.
- [ ] Backend tests green: `dotnet test Mavrynt.sln --no-build` (Docker running).
- [ ] If frontend changed: `npm run typecheck` + `npm run test` in the SPA folder.
- [ ] If architecture changed: new ADR appended to `docs/decisions.en.md`.
- [ ] If status changed: `docs/status.md` updated.
- [ ] No business logic landed in a host or `Program.cs`.
