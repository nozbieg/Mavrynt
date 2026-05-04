# Mavrynt — Repository Structure

## 1. Purpose

This document describes the current repository layout and the rules for adding
new code, tests, and documentation. It is intended to be a navigation reference
for both humans and AI agents.

For high-signal context use `docs/ai-context.md`. For status use `docs/status.md`.

---

## 2. Top-level layout

```text
Mavrynt/
├── AGENTS.md                    AI-agent quick start (read first)
├── README.md                    Human-facing overview
├── Mavrynt.sln
├── Directory.Build.props
├── Directory.Packages.props
├── docs/                        Architecture, decisions, status, next-work, ADR detail notes
├── build/                       Reserved (empty) — future build automation
├── deploy/                      Reserved (empty) — future deployment assets
├── scripts/                     Reserved (empty) — future operational scripts
├── src/
│   ├── backend/
│   └── frontend/
└── tests/
```

`build/`, `deploy/`, and `scripts/` exist as placeholders. Do not put unrelated
files there.

---

## 3. Backend (`src/backend/`)

Modular monolith — each module is split into Domain / Application / Infrastructure
following ADR-005.

### 3.1. Hosts

| Project | Purpose |
|---|---|
| `Mavrynt.Api` | Main user-facing API host. Composes modules; no business logic. |
| `Mavrynt.AdminApp` | Admin API host; all endpoints `AdminOnly` by default. |
| `Mavrynt.AppHost` | Aspire local orchestration of backend + all SPAs. |
| `Mavrynt.ServiceDefaults` | Shared technical defaults: observability, health checks, hosting. |

### 3.2. BuildingBlocks

| Project | Purpose |
|---|---|
| `Mavrynt.BuildingBlocks.Domain` | Base types: `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Result`, `Error`, domain abstractions. |
| `Mavrynt.BuildingBlocks.Application` | Mediator, behavior contracts, validator contract, `IDateTimeProvider`, `ICurrentUserContext`. |
| `Mavrynt.BuildingBlocks.Infrastructure` | `IRepository<TEntity, TId>`, `IUnitOfWork`, `PostgreSqlOptions`, configuration helpers. |
| `Mavrynt.BuildingBlocksContracts` | Integration events and request/response contracts. |

BuildingBlocks must not become a helper dump.

### 3.3. Domain modules

| Module | Schema | Status |
|---|---|---|
| `Mavrynt.Modules.Users.{Domain,Application,Infrastructure}` | `users` | complete |
| `Mavrynt.Modules.FeatureManagement.{Domain,Application,Infrastructure}` | `feature_management` | complete |
| `Mavrynt.Modules.Audit.{Domain,Application,Infrastructure}` | `audit` | complete |
| `Mavrynt.Modules.Notifications.{Domain,Application,Infrastructure}` | `notifications` | complete |

The `Users` module is the canonical template for new modules.

---

## 4. Frontend (`src/frontend/`)

Each SPA is wrapped by an ASP.NET SpaProxy host project so that Aspire AppHost
can launch the dev server. **The SPA source lives one level below the host.**

```text
src/frontend/
├── Mavrynt.Web.App/
│   └── mavrynt-web/          User-facing SPA (React 19 + Vite + TS)
├── Mavrynt.Web.Admin/
│   └── mavrynt-admin/        Admin SPA (login, dashboard, flags, SMTP, settings)
├── Mavrynt.Web.Landing/
│   └── mavrynt-landing/      Marketing landing (decoupled from backend, ADR-015)
└── shared/
    ├── auth-ui/              @mavrynt/auth-ui     Login/register, AuthService port
    ├── config/               @mavrynt/config      env, app-urls, i18n bootstrap
    ├── design-tokens/        @mavrynt/design-tokens
    ├── eslint-config/        @mavrynt/eslint-config
    ├── tsconfig-base/        @mavrynt/tsconfig-base
    └── ui/                   @mavrynt/ui          Presentational primitives
```

Per-SPA scripts (run from the SPA folder):

```bash
npm run dev          # Vite dev server (HMR)
npm run build        # tsc -b && vite build
npm run test         # Vitest
npm run typecheck    # tsc -b --noEmit
npm run lint
npm run test:e2e     # landing only — Playwright
```

The frontend must not reference backend projects. All integration goes through
HTTP API or ports (auth, analytics, lead capture).

---

## 5. Tests (`tests/`)

Backend test layout (see ADR-021 for the strategy):

```text
tests/
├── backend/
│   ├── Mavrynt.Architecture.Tests              dependency rules across all modules
│   └── Mavrynt.BuildingBlocks.Domain.Tests
├── Mavrynt.Modules.Users.Domain.Tests
├── Mavrynt.Modules.Users.Application.Tests
├── Mavrynt.Modules.Users.Infrastructure.Tests           Testcontainers
├── Mavrynt.Modules.FeatureManagement.Domain.Tests
├── Mavrynt.Modules.FeatureManagement.Application.Tests
├── Mavrynt.Modules.FeatureManagement.Infrastructure.Tests   Testcontainers
├── Mavrynt.Modules.Notifications.Domain.Tests
├── Mavrynt.Modules.Notifications.Application.Tests
├── Mavrynt.Modules.Notifications.Infrastructure.Tests       Testcontainers
├── Mavrynt.Api.IntegrationTests                              Testcontainers
└── Mavrynt.AdminApp.IntegrationTests                         Testcontainers
```

> **Gap:** `Modules.Audit` has no dedicated test project. Tracked in `docs/next-work.md`.

Run the whole suite: `dotnet test Mavrynt.sln --no-build`. Docker is required
for Testcontainers-based integration tests.

---

## 6. Documentation (`docs/`)

| File | Purpose |
|---|---|
| `architecture.en.md` | Solution architecture (canonical) |
| `architecture.pl.md` | Polish summary (non-canonical) |
| `decisions.en.md` | ADR log (canonical) — append new ADRs here |
| `decisions.pl.md` | Polish summary (non-canonical) |
| `repo-structure.en.md` | This file (canonical) |
| `repo-structure.pl.md` | Polish summary (non-canonical) |
| `ai-context.md` | Compact AI agent context snapshot |
| `status.md` | Current phase progress (single source of truth) |
| `next-work.md` | Recommended next implementation tasks |
| `frontends.en.md` | Frontend overview |
| `auth-ui.en.md` | `@mavrynt/auth-ui` reference |
| `adr/` | Detailed implementation notes (auth, persistence, audit). See note in `decisions.en.md` about numbering. |

---

## 7. Adding new elements

### New backend module

Create three projects under `src/backend/`:

```text
Mavrynt.Modules.{Name}.Domain
Mavrynt.Modules.{Name}.Application
Mavrynt.Modules.{Name}.Infrastructure
```

Mirror three test projects under `tests/`:

```text
Mavrynt.Modules.{Name}.Domain.Tests
Mavrynt.Modules.{Name}.Application.Tests
Mavrynt.Modules.{Name}.Infrastructure.Tests
```

Register the module in the relevant host (`Mavrynt.Api` and/or `Mavrynt.AdminApp`)
through its `AddXxxApplication(...)` and `AddXxxInfrastructure(...)` extension
methods. Use `Mavrynt.Modules.Users.*` as the template.

### New frontend SPA

Add an ASP.NET SpaProxy wrapper under `src/frontend/Mavrynt.Web.{Name}/` and the
SPA folder one level below. Reuse shared packages from `src/frontend/shared/*`
where possible.

### New architecture decision

Append a new ADR to `docs/decisions.en.md` (and update its index table). Never
edit past ADRs; mark them `Superseded` instead.

---

## 8. Current Phase 1 direction

Phase 1 focuses on the platform foundation:

- users, login, roles and permissions,
- feature flags managed from AdminApp,
- system audit,
- email communication,
- observability,
- testability,
- preparation for CI/CD.

The administrative vertical slice (roles, FeatureManagement, persistent Audit,
AdminApp endpoints, full backend test pyramid) is **complete** as of 2026-04-29.
The Notifications module (DB-backed SMTP, templates, `IEmailNotificationService`,
AdminApp endpoints) is **complete** as of 2026-04-30.

Remaining Phase 1 items: **CI/CD pipeline configuration and staging environment
wiring** (`docs/next-work.md`).

---

## 9. Summary

The repository contains the modular-monolith backend, three React SPAs hosted
under Aspire SpaProxy projects, shared `@mavrynt/*` workspace packages, and a
multi-layer backend test pyramid. Layer and module boundaries (ADR-001 / ADR-005
/ ADR-022 / ADR-023) must be preserved when extending the codebase.
