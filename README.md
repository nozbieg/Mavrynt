# Mavrynt

Mavrynt is an incrementally developed product platform built as a **modular monolith**
in a single repository. It contains the backend, three frontend applications,
documentation, tests, and deployment assets.

> **AI agents:** start with [`AGENTS.md`](AGENTS.md) and
> [`docs/ai-context.md`](docs/ai-context.md). Do not scan the whole repository.

---

## Status

- Phase 1 administrative vertical slice (roles, FeatureManagement, Audit) — **complete** (2026-04-29).
- Notifications / email module (SMTP, templates, `IEmailNotificationService`) — **complete** (2026-04-30).
- Admin SPA shell (login, protected routes, dashboard, feature flags, SMTP settings) — **complete**.
- **Pending Phase 1:** CI/CD pipeline, staging environment, production secret handling.

Authoritative status: [`docs/status.md`](docs/status.md).
Recommended next work: [`docs/next-work.md`](docs/next-work.md).

---

## Stack

- **Backend:** .NET 10, C#, EF Core 9, PostgreSQL, JWT Bearer auth, internal `MavryntMediator`, Testcontainers for integration tests.
- **Frontend:** React 19 + Vite 8 + TypeScript (strict), Tailwind v4, Vitest, Playwright (landing).
- **Local orchestration:** Aspire AppHost (`Mavrynt.AppHost`) launches backend hosts and all three SPAs.

---

## Requirements

- .NET 10 SDK
- Node.js 22+ (per-SPA `npm` scripts)
- Docker — required for Testcontainers-based integration tests

---

## Quick start

**Full stack via Aspire (backend + all SPAs):**

```bash
dotnet run --project src/backend/Mavrynt.AppHost/Mavrynt.AppHost.csproj
```

`Mavrynt.AppHost` orchestrates `Mavrynt.Api`, `Mavrynt.AdminApp`, `mavrynt-web`,
`mavrynt-admin`, and `mavrynt-landing`. The landing is intentionally decoupled from
the API and runs even when the backend is stopped (ADR-015).

**Per-SPA (run from the SPA folder):**

```bash
npm run dev          # Vite dev server
npm run build        # tsc -b && vite build
npm run test         # Vitest
npm run typecheck    # tsc -b --noEmit
npm run test:e2e     # Playwright (landing only)
```

SPA folders:

- `src/frontend/Mavrynt.Web.App/mavrynt-web/`
- `src/frontend/Mavrynt.Web.Admin/mavrynt-admin/`
- `src/frontend/Mavrynt.Web.Landing/mavrynt-landing/`

---

## Validation

```bash
dotnet restore Mavrynt.sln
dotnet build Mavrynt.sln --no-restore
dotnet test Mavrynt.sln --no-build
```

The test pyramid has three layers:

1. **Architecture tests** — `tests/backend/Mavrynt.Architecture.Tests` (NetArchTest).
2. **Unit tests** — domain primitives and command/query handlers with in-memory fakes.
3. **Integration tests** — real PostgreSQL via Testcontainers (Docker required).

---

## Documentation map

| Document | Purpose |
|---|---|
| [`AGENTS.md`](AGENTS.md) | AI-agent quick start, rules, validation, task routing |
| [`docs/ai-context.md`](docs/ai-context.md) | Compact project context snapshot |
| [`docs/status.md`](docs/status.md) | Current phase, completed and pending items |
| [`docs/next-work.md`](docs/next-work.md) | Recommended next implementation tasks |
| [`docs/architecture.en.md`](docs/architecture.en.md) | Solution architecture (canonical, English) |
| [`docs/decisions.en.md`](docs/decisions.en.md) | ADR log (canonical, English) |
| [`docs/repo-structure.en.md`](docs/repo-structure.en.md) | Repository layout |
| [`docs/frontends.en.md`](docs/frontends.en.md) | Frontend overview |
| [`docs/auth-ui.en.md`](docs/auth-ui.en.md) | `@mavrynt/auth-ui` reference |
| [`docs/adr/`](docs/adr/) | Detailed ADR-style notes (auth, persistence, audit) — see numbering note in `docs/decisions.en.md` |

The Polish-language documents (`docs/*.pl.md`) remain as supporting summaries; the
**English versions are canonical**. Add new technical content in English and only
update the Polish counterpart if you can keep it consistent.

---

## Repository layout (compact)

```
Mavrynt/
├── AGENTS.md
├── README.md
├── Mavrynt.sln
├── Directory.Build.props
├── Directory.Packages.props
├── docs/                    docs (architecture, decisions, status, next-work, ADR detail)
├── build/                   reserved (empty) — future build automation
├── deploy/                  reserved (empty) — future deployment assets
├── scripts/                 reserved (empty) — future operational scripts
├── src/
│   ├── backend/
│   │   ├── Mavrynt.Api/                        main API host
│   │   ├── Mavrynt.AdminApp/                   admin API host
│   │   ├── Mavrynt.AppHost/                    Aspire orchestration
│   │   ├── Mavrynt.ServiceDefaults/            shared technical defaults
│   │   ├── Mavrynt.BuildingBlocks.{Domain,Application,Infrastructure}/
│   │   ├── Mavrynt.BuildingBlocksContracts/
│   │   ├── Mavrynt.Modules.Users.{Domain,Application,Infrastructure}/
│   │   ├── Mavrynt.Modules.FeatureManagement.{Domain,Application,Infrastructure}/
│   │   ├── Mavrynt.Modules.Audit.{Domain,Application,Infrastructure}/
│   │   └── Mavrynt.Modules.Notifications.{Domain,Application,Infrastructure}/
│   └── frontend/
│       ├── Mavrynt.Web.App/      ASP.NET host wrapping mavrynt-web/
│       ├── Mavrynt.Web.Admin/    ASP.NET host wrapping mavrynt-admin/
│       ├── Mavrynt.Web.Landing/  ASP.NET host wrapping mavrynt-landing/
│       └── shared/               @mavrynt/{auth-ui,config,design-tokens,eslint-config,tsconfig-base,ui}
└── tests/                   architecture, unit, and integration tests
```

Full layout and rules: [`docs/repo-structure.en.md`](docs/repo-structure.en.md).

---

## Project rules (summary)

- Hosts must not contain business logic.
- Domain has no Infrastructure / framework / host references.
- Cross-module application dependencies are forbidden except FM→Audit and Notifications→Audit (ADR-022, ADR-023).
- Use `IMediator` (`MavryntMediator`) — do not introduce MediatR.
- Use `IEmailNotificationService` — do not call `SmtpClient` directly.
- Marketing landing must remain decoupled from the backend (ADR-015).
- New architecture decisions go into `docs/decisions.en.md` as a new ADR.
