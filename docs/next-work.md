# Mavrynt — Next Work

Practical, task-oriented list of what to do next. Pair with `docs/status.md`
(authoritative progress) and `docs/ai-context.md` (project snapshot).

Priority order is top-to-bottom inside each section.

---

## 1. CI/CD pipeline

**Goal:** automated build + test on every push/PR, before any deployment work.

Tasks:
- Add `.github/workflows/backend.yml`:
  - Trigger: `push`, `pull_request`.
  - Steps: checkout → setup .NET 10 → `dotnet restore Mavrynt.sln` → `dotnet build --no-restore` → `dotnet test --no-build` → publish test results.
  - Run with Docker available so Testcontainers works (ubuntu-latest with Docker, or a self-hosted runner).
- Add `.github/workflows/frontend.yml`:
  - Setup Node 22.
  - Run, per SPA folder (`Mavrynt.Web.App/mavrynt-web`, `Mavrynt.Web.Admin/mavrynt-admin`, `Mavrynt.Web.Landing/mavrynt-landing`):
    - `npm ci`
    - `npm run typecheck`
    - `npm run lint`
    - `npm run test`
    - `npm run build`
  - Upload landing `dist/` as a workflow artifact.
- Add `.github/workflows/landing-e2e.yml`:
  - Run `npm run test:e2e:install` then `npm run test:e2e` for `mavrynt-landing` against the locally built bundle.
- Branch protection: require backend + frontend workflows to pass before merging into `master`.
- (Optional) Decide whether to introduce `pnpm` workspaces. If yes, add a root `package.json` and `pnpm-workspace.yaml`; if no, keep per-SPA `npm` and remove `pnpm --filter` examples from older docs.

Deliverables:
- `.github/workflows/*.yml`
- README/AGENTS updated with the canonical CI commands.

---

## 2. Staging environment

**Goal:** a deploy target where backend and frontends can be exercised end-to-end.

Tasks:
- Choose hosting (e.g., Azure App Service / Container Apps, AWS ECS, or a single VM with Docker Compose).
- Provision PostgreSQL for staging.
- Add `appsettings.Staging.json` for `Mavrynt.Api` and `Mavrynt.AdminApp`:
  - Connection string slot.
  - `Jwt` signing key slot.
  - SMTP slot (or rely on DB-stored settings).
- Decide migration strategy:
  - Run `dotnet ef database update --project Mavrynt.Modules.<Name>.Infrastructure --startup-project Mavrynt.<Host>` from the pipeline.
  - Or rely on `NotificationsStartupService`-style hosted services to migrate on boot (requires confirming all modules support this).
- Pick static hosting for `mavrynt-landing` (CDN + object storage).
- Pick hosting for `mavrynt-web` and `mavrynt-admin` (static SPAs against `Mavrynt.Api` / `Mavrynt.AdminApp`).
- Document the staging URL set and required env (`VITE_APP_URL_LANDING`, `VITE_APP_URL_WEB`, `VITE_APP_URL_ADMIN`).

Deliverables:
- Environment-specific `appsettings.*.json` files.
- A reproducible deploy from a green CI build.

---

## 3. Secret handling

**Goal:** stop relying on dev-only `PassThroughSecretProtector`.

Tasks:
- Implement a real `ISecretProtector` (DPAPI on Windows targets, Azure Key Vault / AWS KMS / GCP KMS for cloud staging).
- Register it in `NotificationsInfrastructure.AddNotificationsInfrastructure(...)` for non-development environments.
- Add an integration test that fails if a non-dev environment falls back to `PassThroughSecretProtector`.
- Document key rotation procedure.

Notes:
- Do not store SMTP passwords or any secret in `appsettings.json` checked into the repository.
- Audit the existing tests to ensure none depend on the pass-through behavior.

---

## 4. Deployment validation and smoke tests

**Goal:** every deploy is verified before traffic is shifted.

Tasks:
- After CI deploy step, run a smoke job that hits:
  - `/health/ready` on `Mavrynt.Api` and `Mavrynt.AdminApp`.
  - One read endpoint that exercises the database (for example `GET /api/admin/feature-flags` with a service-account token).
  - The landing page root and one secondary route.
- Fail the deploy on smoke failure.
- Capture pipeline logs and surface them in the run summary.

---

## 5. Backend gaps to close

These are not blocking Phase 1 but are explicit known gaps. All are scoped narrowly.

- **`Modules.Audit` test project.** Add `tests/Mavrynt.Modules.Audit.Domain.Tests`, `…Application.Tests`, `…Infrastructure.Tests` mirroring the FeatureManagement layout. Today, only `Mavrynt.Architecture.Tests` covers Audit.
- **List-users admin endpoint.** Add a query handler in `Modules.Users.Application` and an endpoint in `AdminApp` so the admin SPA Users page can render real data. The admin SPA already has the page wiring.
- **Propagate audit actor.** Inject `ICurrentUserContext` into the writer side and pass `actorUserId` into `IAuditLogWriter.WriteAsync(...)`. Today all admin audit entries are written with `actorUserId: null` (ADR-022 deferred this).
- **Password reset by email (next).** SMTP foundation is now ready (default local SMTP seed + per-configuration test send, ADR-024). Remaining work for password reset:
  - Domain: a `PasswordResetToken` value object/aggregate with hashed token and expiration.
  - Application: `RequestPasswordResetCommand` (issues token, sends `auth.password_reset` email via `IEmailNotificationService`), `ResetPasswordCommand` (verifies hash + expiration, replaces hash, invalidates token).
  - Infrastructure: persistence for tokens, single-use enforcement.
  - `Mavrynt.Api`: public endpoints `POST /api/auth/forgot-password` and `POST /api/auth/reset-password`. Forgot-password must respond identically for unknown and known emails to avoid account enumeration.
  - Public frontend pages: `/forgot-password` and `/reset-password?token=...` in `mavrynt-web`.
  - Tests: domain unit tests for token expiration/uniqueness, integration tests for the two endpoints.

---

## 6. Documentation maintenance

- Renumber the standalone ADR files in `docs/adr/` so they no longer collide with `docs/decisions.en.md` ADR numbers (two parallel ADR-020..023 sets exist today). Suggestion: rename to `ADR-D01..D04` and add a "supersedes / detail-of" cross-reference at the top of each.
- Keep `docs/status.md` in sync with each merged PR that closes a checklist item.
- Append all future architecture decisions to `docs/decisions.en.md`. Never edit past ADRs; mark them `Superseded` instead.

---

## 7. Phase 2 preparation (informational)

Not work for now, but worth keeping visible:

- A separate `Authorization` module if roles/permissions outgrow the simple `Admin` / `User` model.
- User-facing feature-flag read endpoints (flags are AdminApp-only today).
- Polly-backed resilience policies behind `IResilientRequest`.
- Asynchronous communication (RabbitMQ / Kafka) when an actual integration appears.
- MailKit or transactional email API in place of `SmtpClient` if features like OAuth or DKIM require it.
