# Mavrynt — Repository Structure

## 1. Purpose of this document

The purpose of this document is to describe the current and target structure of the Mavrynt repository. It defines:
- the purpose of the main folders,
- responsibilities of individual projects,
- rules for organizing files and code,
- the direction of future repository expansion.

The repository should be readable for humans, predictable for AI agents, and maintainable over the long term.

---

## 2. Main organizational assumptions

The repository:
- contains the whole product,
- includes backend, frontends, documentation, tests, and deployment assets,
- avoids mixing responsibilities,
- follows consistent naming,
- supports modular monolith development,
- supports local orchestration through Aspire AppHost,
- is being prepared for Continuous Delivery.

The basic rule is: **every folder should have a clear purpose and should contain only what belongs to that responsibility.**

---

## 3. Top-level repository structure

The current repository structure is:

```text
Mavrynt/
├── AGENTS.md
├── Mavrynt.sln
├── README.md
├── Directory.Build.props
├── Directory.Packages.props
├── docs/
│   ├── architecture.pl.md
│   ├── architecture.en.md
│   ├── decisions.pl.md
│   ├── decisions.en.md
│   ├── repo-structure.pl.md
│   └── repo-structure.en.md
├── src/
│   ├── backend/
│   │   ├── Mavrynt.Api/
│   │   ├── Mavrynt.AdminApp/
│   │   ├── Mavrynt.AppHost/
│   │   ├── Mavrynt.ServiceDefaults/
│   │   ├── Mavrynt.BuildingBlocks.Domain/
│   │   ├── Mavrynt.BuildingBlocks.Application/
│   │   ├── Mavrynt.BuildingBlocks.Infrastructure/
│   │   ├── Mavrynt.BuildingBlocksContracts/
│   │   ├── Mavrynt.Modules.Users.Domain/
│   │   ├── Mavrynt.Modules.Users.Application/
│   │   └── Mavrynt.Modules.Users.Infrastructure/
│   └── frontend/
│       ├── Mavrynt.Web.App/
│       ├── Mavrynt.Web.Admin/
│       ├── Mavrynt.Web.Landing/
│       ├── mavrynt-web/
│       ├── mavrynt-admin/
│       ├── mavrynt-landing/
│       └── shared/
└── tests/
    ├── backend/
    │   ├── Mavrynt.Architecture.Tests/
    │   └── Mavrynt.BuildingBlocks.Domain.Tests/
    ├── Mavrynt.Modules.Users.Domain.Tests/
    ├── Mavrynt.Modules.Users.Application.Tests/
    ├── Mavrynt.Modules.Users.Infrastructure.Tests/
    ├── Mavrynt.Api.IntegrationTests/
    └── Mavrynt.AdminApp.IntegrationTests/
```

Note: `build/`, `deploy/`, and `scripts/` remain reserved locations for build/deploy automation and operational scripts, even if they are currently empty or not yet expanded.

---

## 4. Backend

The backend lives in `src/backend` and is organized as a modular monolith.

### 4.1. Hosts

- `Mavrynt.Api` — the main API host for the user-facing product area.
- `Mavrynt.AdminApp` — the API host for the administrative area.
- `Mavrynt.AppHost` — local orchestration through Aspire; runs backend and frontend applications.
- `Mavrynt.ServiceDefaults` — shared technical defaults: observability, health checks, service discovery, and hosting standards.

Hosts must not contain business logic. Their responsibility is module composition, HTTP pipeline configuration, and endpoint exposure.

### 4.2. Building Blocks

- `Mavrynt.BuildingBlocks.Domain` — base domain types, entities, aggregates, value objects, errors, and `Result`.
- `Mavrynt.BuildingBlocks.Application` — command/query abstractions, mediator, validation, pipeline behaviors, and application contexts.
- `Mavrynt.BuildingBlocks.Infrastructure` — shared infrastructure abstractions such as `IRepository`, `IUnitOfWork`, and PostgreSQL options.
- `Mavrynt.BuildingBlocksContracts` — integration contracts and shared contract DTOs.

Building Blocks must not become a random helper dump. Only genuinely shared elements should be placed there.

### 4.3. Domain modules

Current foundation module:

- `Mavrynt.Modules.Users.Domain`
- `Mavrynt.Modules.Users.Application`
- `Mavrynt.Modules.Users.Infrastructure`

The Users module is currently the template for future modules. Every future module should keep the same Domain, Application, and Infrastructure separation.

Expected future Phase 1 modules:
- `FeatureManagement`,
- `Audit`,
- `Notifications` / `Email`,
- possibly a separate `Authorization` module if roles and permissions grow beyond a simple Users model.

---

## 5. Frontend

The frontend lives in `src/frontend`.

Main applications:
- `mavrynt-web` — user-facing application,
- `mavrynt-admin` — administrative application,
- `mavrynt-landing` — static marketing landing page.

SPA host projects:
- `Mavrynt.Web.App`,
- `Mavrynt.Web.Admin`,
- `Mavrynt.Web.Landing`.

Shared packages live under `src/frontend/shared/*` and may include:
- TypeScript configuration,
- ESLint configuration,
- design tokens,
- UI primitives,
- application URL configuration,
- shared auth UI components.

The frontend must not reference backend projects directly. Integration happens through APIs, adapters, and explicit contracts.

---

## 6. Tests

Tests live in `tests`.

Current split:
- `tests/backend/Mavrynt.Architecture.Tests` — architectural tests protecting dependency boundaries.
- `tests/backend/Mavrynt.BuildingBlocks.Domain.Tests` — tests for domain primitives.
- `tests/Mavrynt.Modules.Users.Domain.Tests` — Users domain tests.
- `tests/Mavrynt.Modules.Users.Application.Tests` — Users command/query handler tests.
- `tests/Mavrynt.Modules.Users.Infrastructure.Tests` — Users infrastructure tests with PostgreSQL through Testcontainers.
- `tests/Mavrynt.Api.IntegrationTests` — main API integration tests.
- `tests/Mavrynt.AdminApp.IntegrationTests` — AdminApp integration tests.

Target backend validation has three layers:
1. architectural tests,
2. module unit tests,
3. integration tests with a real database through Testcontainers.

---

## 7. Documentation

Documentation lives in `docs`.

The most important files are:
- `architecture.pl.md` / `architecture.en.md` — solution architecture,
- `decisions.pl.md` / `decisions.en.md` — architecture decision log,
- `repo-structure.pl.md` / `repo-structure.en.md` — repository structure.

`AGENTS.md` contains short operational instructions for AI agents and should remain consistent with the documents under `docs`.

---

## 8. Rules for adding new elements

A new backend domain module should be added under `src/backend` as a set of projects:

```text
Mavrynt.Modules.{Name}.Domain/
Mavrynt.Modules.{Name}.Application/
Mavrynt.Modules.{Name}.Infrastructure/
```

New module tests should be added under `tests` as separate test projects:

```text
Mavrynt.Modules.{Name}.Domain.Tests/
Mavrynt.Modules.{Name}.Application.Tests/
Mavrynt.Modules.{Name}.Infrastructure.Tests/
```

A new frontend application should be added under `src/frontend` and should use shared packages only when they are genuinely shared.

A new architectural decision should be added to both `docs/decisions.pl.md` and `docs/decisions.en.md`.

---

## 9. Current Phase 1 direction

Phase 1 focuses on the platform foundation:
- users,
- login,
- roles and permissions,
- feature flags managed from AdminApp,
- system audit,
- basic email communication,
- observability,
- testability,
- preparation for CI/CD.

Given the current state of the Users module and backend tests, the next logical step is to close the administrative vertical slice: roles/permissions + feature flags + audit, wired through AdminApp and protected with tests.

---

## 10. Summary

The Mavrynt repository already has the modular monolith foundation, backend hosts, the base Users module, frontend applications, and the beginning of the backend test pyramid. The repository structure should remain stable, and the next work should develop concrete Phase 1 modules without breaking layer boundaries or moving business logic into hosts.
