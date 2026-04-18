# Mavrynt — Initial Documentation Bundle

## Target files

Use the following file names:

- `README.md`
- `docs/architecture.md`
- `docs/decisions.md`
- `docs/repo-structure.md`

---

# File: `README.md`

# Mavrynt

## PL

### Opis projektu

Mavrynt to rozwijana etapowo platforma produktowa budowana w architekturze **modularnego monolitu** w ramach jednego repozytorium. Projekt obejmuje backend, aplikacje frontendowe, część administracyjną, dokumentację techniczną, testy oraz zasoby wdrożeniowe.

Na obecnym etapie celem jest zbudowanie solidnego fundamentu technicznego pod dalszy rozwój produktu, w szczególności w obszarach:
- użytkowników,
- uwierzytelniania i autoryzacji,
- ról i uprawnień,
- procesów administracyjnych,
- feature flag,
- observability,
- testowalności,
- przygotowania pod Continuous Delivery.

### Główne założenia

Projekt został zaprojektowany z następującymi założeniami:
- jedno repozytorium dla całego produktu,
- modularny monolit jako model startowy,
- wyraźny podział odpowiedzialności pomiędzy hostami, modułami i warstwami wspólnymi,
- osobna część administracyjna,
- możliwość dalszej rozbudowy o kolejne moduły domenowe,
- gotowość pod lokalną orkiestrację, konteneryzację i pipeline CI/CD.

### Struktura repozytorium

    Mavrynt/
    ├── Mavrynt.sln
    ├── README.md
    ├── .gitignore
    ├── .gitattributes
    ├── Directory.Build.props
    ├── Directory.Packages.props
    ├── docs/
    ├── build/
    ├── deploy/
    ├── scripts/
    ├── src/
    │   ├── backend/
    │   ├── frontend/
    │   └── shared/
    └── tests/

### Główne elementy rozwiązania

#### Backend

W katalogu `src/backend` znajdują się projekty backendowe, w tym:
- `Mavrynt.Api` — główny host API,
- `Mavrynt.AdminApp` — host backendowy dla części administracyjnej,
- `Mavrynt.AppHost` — projekt orkiestracyjny dla developmentu lokalnego,
- `Mavrynt.ServiceDefaults` — wspólne ustawienia techniczne,
- `Mavrynt.BuildingBlocks.*` — współdzielone warstwy bazowe,
- `Mavrynt.Modules.*` — moduły domenowe.

#### Frontend

W katalogu `src/frontend` przewidziano:
- `mavrynt-web` — frontend użytkownika,
- `mavrynt-admin` — frontend administracyjny,
- `mavrynt-landing` — statyczny landing marketingowy.

#### Dokumentacja

W katalogu `docs` znajdują się dokumenty opisujące architekturę, decyzje projektowe oraz strukturę repozytorium.

#### Testy

W katalogu `tests` znajdują się miejsce na:
- testy backendowe,
- testy frontendowe,
- testy integracyjne,
- testy architektoniczne.

### Dokumentacja techniczna

Najważniejsze dokumenty znajdują się w katalogu `docs`:
- `docs/architecture.md`
- `docs/decisions.md`
- `docs/repo-structure.md`

Dokumenty te opisują:
- architekturę rozwiązania,
- najważniejsze decyzje techniczne,
- strukturę katalogów i projektów,
- kierunek dalszego rozwoju.

### Aktualny stan

Repozytorium jest budowane ręcznie od czystego pliku solution `.sln`, krok po kroku, z pełną kontrolą nad:
- strukturą projektów,
- zależnościami,
- nazewnictwem,
- odpowiedzialnością poszczególnych warstw.

To podejście ma zapewnić wysoką jakość architektury startowej oraz ułatwić dalszy rozwój projektu.

### Cele najbliższych etapów

Najbliższe etapy rozwoju obejmują:
- rozbudowę wspólnych building blocks,
- rozwój pierwszych modułów domenowych,
- dodanie projektów testowych,
- konfigurację wspólnego builda i pakietów,
- dodanie frontendów,
- przygotowanie środowiska lokalnego i wdrożeniowego.

### Zasady organizacyjne

W projekcie obowiązują następujące zasady:
- nie umieszczamy logiki biznesowej w hostach,
- nie mieszamy warstw domeny, aplikacji i infrastruktury,
- każdy moduł powinien zachowywać spójny układ,
- dokumentacja architektury jest utrzymywana razem z kodem,
- większe decyzje techniczne powinny być dopisywane do `docs/decisions.md`.

### Uruchomienie

Na obecnym etapie repozytorium stanowi bazę architektoniczną i strukturalną. Szczegółowe instrukcje uruchomienia poszczególnych komponentów będą uzupełniane wraz z dalszym rozwojem projektu.

Docelowo README powinno zawierać:
- wymagania środowiskowe,
- instrukcję uruchomienia lokalnego,
- opis zależności infrastrukturalnych,
- informacje o buildzie i testach,
- informacje o deploymencie.

### Status

Projekt jest w fazie aktywnej inicjalizacji architektury i struktury repozytorium.

## EN

### Project overview

Mavrynt is an incrementally developed product platform built as a **modular monolith** within a single repository. The project includes backend services, frontend applications, an administrative area, technical documentation, tests, and deployment assets.

At the current stage, the goal is to build a solid technical foundation for further product development, especially in areas such as:
- users,
- authentication and authorization,
- roles and permissions,
- administrative processes,
- feature flags,
- observability,
- testability,
- preparation for Continuous Delivery.

### Core assumptions

The project is designed with the following assumptions:
- a single repository for the whole product,
- a modular monolith as the starting model,
- clear responsibility boundaries between hosts, modules, and shared layers,
- a separate administrative area,
- readiness for future domain module expansion,
- readiness for local orchestration, containerization, and CI/CD pipelines.

### Repository structure

    Mavrynt/
    ├── Mavrynt.sln
    ├── README.md
    ├── .gitignore
    ├── .gitattributes
    ├── Directory.Build.props
    ├── Directory.Packages.props
    ├── docs/
    ├── build/
    ├── deploy/
    ├── scripts/
    ├── src/
    │   ├── backend/
    │   ├── frontend/
    │   └── shared/
    └── tests/

### Main solution components

#### Backend

The `src/backend` folder contains backend projects, including:
- `Mavrynt.Api` — the main API host,
- `Mavrynt.AdminApp` — the backend host for the administrative area,
- `Mavrynt.AppHost` — the orchestration project for local development,
- `Mavrynt.ServiceDefaults` — shared technical defaults,
- `Mavrynt.BuildingBlocks.*` — shared foundational layers,
- `Mavrynt.Modules.*` — domain modules.

#### Frontend

The `src/frontend` folder is intended for:
- `mavrynt-web` — the user-facing frontend,
- `mavrynt-admin` — the administrative frontend,
- `mavrynt-landing` — the static marketing landing page.

#### Documentation

The `docs` folder contains documents describing the architecture, project decisions, and repository structure.

#### Tests

The `tests` folder is reserved for:
- backend tests,
- frontend tests,
- integration tests,
- architectural tests.

### Technical documentation

The most important documents are located in the `docs` folder:
- `docs/architecture.md`
- `docs/decisions.md`
- `docs/repo-structure.md`

These documents describe:
- the solution architecture,
- the key technical decisions,
- the project and folder structure,
- the intended direction of further development.

### Current state

The repository is being built manually from a clean `.sln` file, step by step, with full control over:
- project structure,
- dependencies,
- naming,
- responsibility boundaries.

This approach is intended to ensure high architectural quality at the foundation stage and to support long-term maintainability.

### Near-term goals

The next stages of development include:
- expanding the shared building blocks,
- developing the first domain modules,
- adding test projects,
- configuring shared build and package management,
- adding frontend applications,
- preparing local and deployment environments.

### Organizational rules

The following rules apply in the project:
- business logic must not be placed in hosts,
- domain, application, and infrastructure layers must not be mixed,
- every module should follow a consistent structure,
- architecture documentation is maintained together with the code,
- major technical decisions should be added to `docs/decisions.md`.

### Running the solution

At the current stage, the repository serves as an architectural and structural foundation. Detailed instructions for running individual components will be added as the project evolves.

Eventually, this README should include:
- environment requirements,
- local setup instructions,
- infrastructure dependency details,
- build and test instructions,
- deployment information.

### Status

The project is currently in the active architecture and repository structure initialization phase.

---

# File: `docs/architecture.md`

# Mavrynt — Solution Architecture

## 1. Purpose of this document

The purpose of this document is to describe the target architecture of the Mavrynt solution at the repository, component, project responsibility, and dependency rule levels. This document is intended to serve as a shared reference point for further implementation performed manually and with the support of AI agents.

The architecture has been designed to:
- enable incremental product development,
- preserve high code readability and clear responsibilities,
- support a modular monolith as the initial operating model,
- remain ready for future expansion with additional domain modules,
- support continuous delivery, observability, testability, and feature management.

## 2. System context

Mavrynt is being built as a single product within a single repository. At the current stage, the foundation is the administrative and base product layer, covering areas such as:
- users,
- authentication and authorization,
- roles and permissions,
- password handling and reset flows,
- basic system processes,
- administrative capabilities,
- feature flag management,
- preparation for email communication,
- baseline observability and environment management.

The solution includes:
- application backend,
- a separate administrative application,
- an end-user frontend,
- an administrative frontend,
- a static marketing landing page,
- infrastructure and deployment assets.

## 3. Architectural style

### 3.1. Primary model

The selected architectural style is a **modular monolith**.

This means that:
- the system operates as one logical product,
- domain modules are structurally separated,
- each module has its own responsibility boundaries,
- dependencies between modules are controlled,
- internal communication should remain explicit and predictable,
- future service extraction is not excluded, but it is not the initial goal.

### 3.2. Rationale

The modular monolith was chosen because it:
- reduces entry cost and operational complexity,
- supports building the solution manually from a clean `.sln`,
- works well with a single repository model,
- enables fast iteration on the product foundation,
- provides a balanced trade-off between separation and maintainability.

## 4. Core architectural assumptions

### 4.1. Single repository

The entire solution is stored in one repository. The repository contains:
- backend,
- frontends,
- tests,
- documentation,
- scripts,
- build and deployment configuration.

### 4.2. Clear responsibility boundaries

Each project should have a clear and narrow responsibility. The solution should not mix:
- application hosts,
- domain logic,
- application logic,
- infrastructure concerns,
- contracts,
- shared code unrelated to the project’s responsibility.

### 4.3. Dependencies should point inward

Higher-level layers may depend on lower-level semantic layers, but not the other way around. In practice:
- `Domain` does not know `Infrastructure`,
- `Domain` does not know application hosts,
- `Application` does not depend on a host,
- `Infrastructure` implements the needs of upper layers,
- the host composes modules and runtime configuration.

### 4.4. Backend as the product core

The backend is the central element of the system. Frontends act as clients of the backend or as separate presentation layers.

### 4.5. Admin as a separate responsibility area

The administrative area is treated as a distinct part of the product. It has its own backend host and its own administrative frontend, even if some logic remains shared during the early stages.

## 5. Main solution components

### 5.1. `Mavrynt.AppHost`

A runtime project intended for local orchestration and consistent wiring of environment dependencies. Its responsibility is to:
- run backend components during development,
- wire infrastructure dependencies together,
- prepare the local development environment,
- simplify local startup of the solution.

At later stages it may integrate:
- PostgreSQL,
- Redis,
- RabbitMQ,
- Kafka,
- other components supporting development and integration testing.

### 5.2. `Mavrynt.ServiceDefaults`

A project containing shared service settings and technical standards. Example responsibilities:
- observability configuration,
- shared hosting extensions,
- standard health checks,
- shared telemetry settings,
- common technical conventions for backend services.

### 5.3. `Mavrynt.Api`

The main product API host. It is responsible for:
- exposing application endpoints,
- composing modules,
- configuring the HTTP pipeline,
- authentication and authorization at the host level,
- integrating backend modules.

This is the main entry point for the user-facing client.

### 5.4. `Mavrynt.AdminApp`

A backend host dedicated to the administrative area. It is responsible for:
- administrative functions,
- user management,
- role and permission management,
- feature flag management,
- future administrative system areas.

This is a separate host because the admin area has distinct security, permission, and responsibility requirements.

### 5.5. `Mavrynt.BuildingBlocks.*`

A set of shared projects for the entire backend.

#### `Mavrynt.BuildingBlocks.Domain`

Base code for the domain layer, for example:
- entity base classes,
- value object patterns,
- domain errors,
- domain abstractions.

#### `Mavrynt.BuildingBlocks.Application`

Base code for the application layer, for example:
- command/query abstractions,
- shared use case interfaces,
- validation and pipeline behaviors,
- dependency registration patterns.

#### `Mavrynt.BuildingBlocks.Infrastructure`

Shared infrastructure code, for example:
- persistence mechanisms,
- configuration extensions,
- integration building blocks,
- technical implementation patterns for data access.

#### `Mavrynt.BuildingBlocks.Contracts`

Shared contracts, for example:
- integration events,
- request/response contracts,
- cross-module or cross-layer messages.

### 5.6. `Mavrynt.Modules.*`

Domain modules of the system. Each module should have its own responsibility boundary.

At the current stage, the first foundational module is `Users`.

Example structure:
- `Mavrynt.Modules.Users.Domain`
- `Mavrynt.Modules.Users.Application`
- `Mavrynt.Modules.Users.Infrastructure`

In the future, modules such as the following may be added:
- Identity
- Authorization
- FeatureManagement
- Notifications
- Audit
- Billing
- Signals
- MarketData
- NewsAnalysis

The exact list will depend on future product stages.

## 6. Layer model inside a module

Each backend module should aim for a consistent layered model.

### 6.1. Domain layer

The domain layer contains:
- entities,
- value objects,
- domain enums,
- domain events,
- repository abstractions,
- business rules.

This layer should not know:
- EF Core,
- HTTP,
- hosting,
- infrastructure implementations,
- presentation frameworks.

### 6.2. Application layer

The application layer contains:
- use cases,
- commands,
- queries,
- DTOs,
- input and output contracts,
- orchestration of module logic.

The application layer uses the domain, but should not take responsibility for persistence details or hosting concerns.

### 6.3. Infrastructure layer

The infrastructure layer contains:
- repository implementations,
- database configuration,
- mappings,
- external integrations,
- technical configuration of the module.

This is where technology-specific details belong.

### 6.4. Host

The host:
- registers modules,
- exposes endpoints,
- wires environment configuration,
- handles cross-cutting concerns at the application level,
- should not contain domain logic.

## 7. Dependency rules

### 7.1. Allowed dependencies

Examples of allowed dependencies:
- `BuildingBlocks.Application` → `BuildingBlocks.Domain`
- `BuildingBlocks.Infrastructure` → `BuildingBlocks.Domain`
- `BuildingBlocks.Infrastructure` → `BuildingBlocks.Application`
- `Modules.Users.Domain` → `BuildingBlocks.Domain`
- `Modules.Users.Application` → `BuildingBlocks.Application`
- `Modules.Users.Application` → `Modules.Users.Domain`
- `Modules.Users.Infrastructure` → `Modules.Users.Application`
- `Modules.Users.Infrastructure` → `Modules.Users.Domain`
- `Mavrynt.Api` → modules and building blocks
- `Mavrynt.AdminApp` → modules and building blocks

### 7.2. Forbidden dependencies

The following must not be allowed:
- `Domain` → `Infrastructure`
- `Domain` → host
- `Application` → host
- uncontrolled direct dependency from module A to implementation details of module B
- frontend depending directly on backend projects
- marketing landing page depending on backend domain logic

## 8. Frontend and presentation layer

The repository is expected to contain three main frontend applications:

### `mavrynt-web`

The main end-user frontend.

### `mavrynt-admin`

The administrative frontend.

### `mavrynt-landing`

A static marketing landing page without dependency on the backend domain model. It may use only minimal integration mechanisms or remain entirely static.

Frontends are maintained separately from the backend, but within the same repository.

## 9. Data and infrastructure

The target architecture assumes the use of:
- PostgreSQL as the primary database,
- Redis as cache and supporting infrastructure,
- RabbitMQ and Kafka as asynchronous communication components where needed,
- Docker and Compose as the basis for local environments and technical deployment flows.

At the current stage, this document describes the architectural direction rather than the full infrastructure implementation.

## 10. Feature flags

The feature flag mechanism is an important part of the architecture. The assumptions are:
- feature flags should be supported from the beginning,
- flag management should be available from the administrative area,
- feature flags should support controlled rollout of functionality,
- the architecture should allow conditional activation per environment, user segment, or other segmentation strategy.

Feature flags are treated as a foundational Continuous Delivery capability, not as an optional add-on.

## 11. Observability

The architecture assumes full observability from the beginning. This includes:
- logging,
- metrics,
- traces,
- health checks,
- environment diagnostics.

Shared observability standards should be maintained centrally, mainly through `Mavrynt.ServiceDefaults` and shared technical layers.

## 12. Testability

The solution should be designed to support:
- unit tests for modules,
- integration tests for hosts and infrastructure layers,
- architectural tests for dependency rules,
- frontend tests for client applications.

Tests are treated as a first-class part of the repository, not as an afterthought.

## 13. Continuous Delivery

The repository and project architecture should support full Continuous Delivery. This includes:
- predictable build structure,
- repeatable environment startup,
- the ability to validate applications independently,
- safe rollout using feature flags,
- a clear split of environment-specific configuration.

## 14. Future development directions

In later stages, the architecture will be expanded with:
- additional domain modules,
- architectural tests,
- shared package configuration,
- containerization,
- CI/CD pipelines,
- data and migration configuration,
- security and access policies,
- notification and asynchronous communication mechanisms.

## 15. Summary

Mavrynt is being built as a modular monolith in a single repository, with a clear split between hosts, building blocks, domain modules, frontends, and infrastructure assets. The main goal of the architecture is to preserve order, organizational and technical scalability, and readiness for future growth without unnecessary initial complexity.

This document serves as a reference baseline for further implementation decisions and should be updated as the solution evolves.

---

# File: `docs/decisions.md`

# Mavrynt — Architecture Decision Log

## Purpose of this document

This document collects the most important architectural and organizational decisions related to the Mavrynt solution. Each decision should be recorded in a concise but unambiguous way so that in the future it is clear:
- what was decided,
- why the decision was made,
- what consequences follow from that decision.

This is a living document and should be extended whenever important changes are made.

## ADR-001 — Solution model: modular monolith

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The Mavrynt system is built as a modular monolith.

### Rationale

At the product foundation stage, the following are more important:
- simpler code organization,
- lower operational cost,
- faster setup,
- easier manual creation and control of the solution,
- preserving logical module separation without running many services.

### Consequences

- the system remains one solution and one backend product,
- modules are separated inside a single repository,
- dependency boundaries must be actively protected,
- future microservice extraction may happen later, but it is not the initial goal.

## ADR-002 — Single repository for the whole product

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The entire product is maintained in a single repository.

### Rationale

A single repository simplifies:
- architecture management,
- consistent versioning,
- cooperation with AI agents,
- alignment of backend, frontends, documentation, and deployment assets,
- maintenance of shared technical standards.

### Consequences

- the repository must have a clear structure,
- project and folder responsibilities must be carefully enforced,
- consistent organization rules become more important,
- shared pipelines and standards become easier to maintain.

## ADR-003 — Separation of backend hosts

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The backend is split into separate hosts:
- `Mavrynt.Api`
- `Mavrynt.AdminApp`
- `Mavrynt.AppHost`
- `Mavrynt.ServiceDefaults` as a project supporting shared technical standards

### Rationale

The user-facing and administrative areas have different responsibilities and different usage scenarios. In addition, local development requires a project that wires environment dependencies together.

### Consequences

- `Mavrynt.Api` becomes the main product API host,
- `Mavrynt.AdminApp` serves the administrative area,
- `Mavrynt.AppHost` handles local orchestration,
- `Mavrynt.ServiceDefaults` stores standards shared across backend hosts.

## ADR-004 — Separate administrative area

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The admin area is treated as a separate product area, not just a set of extra screens.

### Rationale

The administrative panel has a different nature than the user-facing area. It requires:
- separate permission handling,
- stronger security control,
- system configuration management,
- feature flag management,
- user and access management.

### Consequences

- the admin area can evolve independently from the user-facing layer,
- the administrative part preserves clear responsibility boundaries,
- the architecture supports dedicated administrative workflows from the beginning.

## ADR-005 — Layered module structure

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

Each backend module should follow the structure:
- `Domain`
- `Application`
- `Infrastructure`

### Rationale

This approach supports:
- clear responsibility separation,
- testability,
- dependency control,
- incremental delivery of logic,
- better cooperation with code generated or developed by AI agents.

### Consequences

- domain logic does not belong in the host,
- persistence details do not belong in the domain,
- use cases are placed in the application layer,
- infrastructure implements the needs of upper layers.

## ADR-006 — Shared BuildingBlocks projects

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

A set of shared projects is created:
- `Mavrynt.BuildingBlocks.Domain`
- `Mavrynt.BuildingBlocks.Application`
- `Mavrynt.BuildingBlocks.Infrastructure`
- `Mavrynt.BuildingBlocks.Contracts`

### Rationale

Shared projects allow the solution to store:
- shared abstractions,
- implementation patterns,
- foundational types and contracts,
- technical elements reused across modules.

### Consequences

- BuildingBlocks must not become an unstructured dump for everything,
- only genuinely reusable elements should go there,
- module responsibility boundaries must remain intact.

## ADR-007 — First foundational module: Users

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The first foundational backend module is `Users`.

### Rationale

Users are the natural base for further areas such as:
- authentication,
- authorization,
- roles and permissions,
- password resets,
- administrative operations.

### Consequences

- future modules will build on the established boundaries of the user area,
- the `Users` module becomes one of the core foundations of the product.

## ADR-008 — Feature flags as a core architectural capability

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

Feature flags must be present from the beginning of the architecture and managed from the administrative area.

### Rationale

Feature flags support:
- safe rollout of changes,
- controlled activation of functionality,
- separation of deployment from release,
- Continuous Delivery.

### Consequences

- the architecture must reserve a place for a feature management module or component,
- the administrative panel must support feature flag management,
- the project must be ready for environment- and context-dependent flag configuration.

## ADR-009 — Full observability from the start

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The solution should support full observability from the beginning.

### Rationale

The project is intended to grow and be deployed in a controlled way. This requires:
- logs,
- metrics,
- traces,
- health checks,
- predictable diagnostics.

### Consequences

- part of the shared setup should live in `Mavrynt.ServiceDefaults`,
- hosts and modules must be ready for common telemetry standards,
- observability cannot be postponed until the end of implementation.

## ADR-010 — Frontends in the same repository but separated from the backend

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

Frontends remain in the same repository, but are physically separated under `src/frontend`.

### Rationale

This approach makes it easier to:
- preserve product consistency,
- evolve client applications and the landing page together,
- avoid mixing backend and frontend concerns,
- build shared pipelines in one repository.

### Consequences

- the frontend must not depend directly on backend projects,
- integration happens through APIs and contracts, not direct project references,
- the marketing landing page should remain as independent as possible.

## ADR-011 — PostgreSQL as the primary database

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The primary relational database of the project is PostgreSQL.

### Rationale

PostgreSQL is a good fit for:
- modular application systems,
- .NET-based development,
- containerized environments,
- modern deployment scenarios.

### Consequences

- the infrastructure layer will be designed with PostgreSQL as the standard database,
- development and deployment configuration will assume PostgreSQL by default.

## ADR-012 — Redis, RabbitMQ, and Kafka reserved in the architecture

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The architecture reserves a place for:
- Redis,
- RabbitMQ,
- Kafka.

### Rationale

The product should be ready for:
- caching,
- asynchronous communication,
- event-driven flows,
- integrations and internal scaling.

### Consequences

- not all of these components need to be active in the first stage,
- the repository and AppHost should be prepared for their later introduction,
- the architecture must not block future integration scenarios.

## ADR-013 — Manual solution setup from a clean `.sln`

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

The solution is built manually from an empty `.sln`, step by step, with full control over structure and naming.

### Rationale

The manual approach provides:
- full control over naming and layout,
- better initial architectural quality,
- lower risk of accidental dependencies,
- greater predictability for further development.

### Consequences

- initial setup is slower, but more controlled,
- structure and decision documentation become critical,
- each next step should be consciously reviewed.

## ADR-014 — Architecture documentation stored inside the repository

**Status:** Accepted  
**Date:** 2026-04-18

### Decision

Architectural documents are stored directly in the repository under the `docs` folder.

### Rationale

Documentation kept close to the code:
- stays easier to update,
- becomes part of the development process,
- helps both people and AI agents,
- reduces the risk of divergence between implementation and design.

### Consequences

- documentation should be updated when important changes are made,
- architectural decisions should be recorded continuously,
- the repository becomes the single source of truth for the product.

## Rules for adding future decisions

Each new decision should include:
- an identifier,
- a status,
- a date,
- the decision itself,
- rationale,
- consequences.

Recommended statuses:
- Proposed
- Accepted
- Rejected
- Superseded

## Open areas for future decisions

The following areas require future decisions:
- the exact authorization and role model,
- the feature management module design,
- API endpoint and versioning standards,
- library and pattern selection for validation and mediators,
- migration strategy,
- architectural test strategy,
- asynchronous communication model,
- environment deployment standards,
- secret and configuration management model,
- CI/CD design.

## Summary

This document is the architectural decision register for Mavrynt. It is intended to preserve consistency while the solution is being built manually and extended over time. Every significant decision that changes architectural direction should be added here.

---

# File: `docs/repo-structure.md`

# Mavrynt — Repository Structure

## 1. Purpose of this document

The purpose of this document is to describe the target and current structure of the Mavrynt repository. It defines:
- the purpose of the main folders,
- responsibilities of the individual projects,
- rules for organizing files and code,
- the direction of future repository expansion.

The repository should be readable for humans, predictable for AI agents, and maintainable over the long term.

## 2. Main organizational assumptions

The repository:
- contains the whole product,
- includes backend, frontend, documentation, tests, and deployment assets,
- avoids mixing responsibilities,
- follows consistent naming,
- supports modular monolith development.

The basic rule is: **every folder should have a clear purpose and should contain only what belongs to that responsibility.**

## 3. Top-level repository structure

The current target repository structure is:

    Mavrynt/
    ├── Mavrynt.sln
    ├── README.md
    ├── .gitignore
    ├── .gitattributes
    ├── Directory.Build.props
    ├── Directory.Packages.props
    ├── docs/
    ├── build/
    ├── deploy/
    ├── scripts/
    ├── src/
    │   ├── backend/
    │   ├── frontend/
    │   └── shared/
    └── tests/

## 4. Root-level files

### 4.1. `Mavrynt.sln`

The main solution file for .NET projects. It should include all backend and test projects related to the .NET part of the system.

### 4.2. `README.md`

The repository entry file. It should contain:
- a short project description,
- repository purpose,
- basic startup information,
- links to documentation under `docs`.

### 4.3. `.gitignore`

Defines files and folders ignored by Git.

### 4.4. `.gitattributes`

Defines file handling standards, for example line endings and text rules.

### 4.5. `Directory.Build.props`

Shared build configuration for .NET projects, for example:
- shared language version,
- warning configuration,
- common compilation settings.

### 4.6. `Directory.Packages.props`

Central management of NuGet package versions.

## 5. `docs`

The architecture, project, and organizational documentation folder.

Example files:
- `architecture.md`
- `decisions.md`
- `repo-structure.md`

Later it may also include:
- domain documents,
- coding standards,
- environment descriptions,
- diagrams,
- release and deployment process documentation.

### Rules

- documentation should evolve together with the project,
- files under `docs` are the source of truth for high-level structure and decisions,
- temporary notes and random working dumps should not be stored here.

## 6. `build`

A folder intended for build-related assets.

It may contain:
- build configuration files,
- helper scripts for build processes,
- versioning definitions,
- pipeline support files.

### Rules

- only build-related elements belong here,
- build and deployment assets should not be mixed without a clear reason,
- no business logic belongs here.

## 7. `deploy`

A folder intended for deployment assets.

It may contain:
- Docker Compose files,
- environment configuration,
- deployment manifests,
- deployment templates,
- environment startup definitions.

### Rules

- `deploy` is for runtime and deployment concerns,
- files should be grouped clearly by environment or deployment type,
- development and production assets should not be mixed without an explicit structure.

## 8. `scripts`

A folder for operational and development helper scripts.

It may contain:
- bootstrap scripts,
- local startup scripts,
- data preparation scripts,
- CI/CD helper scripts.

### Rules

- scripts should be named clearly and explicitly,
- random one-off files should be avoided,
- every important script should have a short description in a README or inline comments.

## 9. `src`

The source folder of the whole product.

    src/
    ├── backend/
    ├── frontend/
    └── shared/

## 10. `src/backend`

This is where the backend solution and shared .NET server-side projects live.

Target structure:

    src/backend/
    ├── Mavrynt.AppHost/
    ├── Mavrynt.ServiceDefaults/
    ├── Mavrynt.Api/
    ├── Mavrynt.AdminApp/
    ├── Mavrynt.BuildingBlocks/
    └── Mavrynt.Modules/

## 11. `src/backend/Mavrynt.AppHost`

A runtime project intended for local orchestration and wiring environment dependencies together during development.

### Responsibility

- starting local dependencies,
- wiring application hosts together,
- simplifying development and debugging.

### Should not contain

- business logic,
- domain implementations,
- random helpers unrelated to hosting.

## 12. `src/backend/Mavrynt.ServiceDefaults`

A project containing shared technical settings for backend services.

### Responsibility

- observability,
- health checks,
- shared host extensions,
- common technical conventions.

### Should not contain

- module business logic,
- endpoints,
- frontend code.

## 13. `src/backend/Mavrynt.Api`

The main product API host.

### Responsibility

- HTTP pipeline,
- module registration,
- application endpoints,
- security configuration at the host level,
- integration with domain backend modules.

### Should not contain

- module domain logic,
- repository implementations,
- unrelated shared classes.

## 14. `src/backend/Mavrynt.AdminApp`

A backend host for the administrative area.

### Responsibility

- administrative functions,
- system operations,
- user management,
- access management,
- feature flag management.

### Should not contain

- generic user-facing logic unrelated to administration,
- domain logic placed directly in the host.

## 15. `src/backend/Mavrynt.BuildingBlocks`

A folder containing backend shared projects.

Target structure:

    src/backend/Mavrynt.BuildingBlocks/
    ├── Mavrynt.BuildingBlocks.Domain/
    ├── Mavrynt.BuildingBlocks.Application/
    ├── Mavrynt.BuildingBlocks.Infrastructure/
    └── Mavrynt.BuildingBlocks.Contracts/

### Purpose

To separate reusable shared elements from the code of specific modules.

## 16. `Mavrynt.BuildingBlocks.Domain`

### Responsibility

- domain abstractions,
- domain primitives,
- domain errors,
- foundational patterns for entities and value objects.

### Example folders

- `Abstractions`
- `Primitives`
- `Errors`

## 17. `Mavrynt.BuildingBlocks.Application`

### Responsibility

- application abstractions,
- shared use case patterns,
- pipeline behaviors,
- shared DI extensions for the application layer.

### Example folders

- `Abstractions`
- `Behaviors`
- `DependencyInjection`

## 18. `Mavrynt.BuildingBlocks.Infrastructure`

### Responsibility

- shared persistence elements,
- technical configuration,
- shared infrastructure extensions,
- technical cross-cutting components.

### Example folders

- `Persistence`
- `Configuration`
- `Extensions`

## 19. `Mavrynt.BuildingBlocks.Contracts`

### Responsibility

- request/response contracts,
- integration events,
- shared messages.

### Example folders

- `Events`
- `Requests`
- `Responses`

## 20. `src/backend/Mavrynt.Modules`

A folder containing backend domain modules.

Module structure should be predictable and repeatable. Each module should have its own layered projects.

Example:

    src/backend/Mavrynt.Modules/
    └── Users/
        ├── Mavrynt.Modules.Users.Domain/
        ├── Mavrynt.Modules.Users.Application/
        └── Mavrynt.Modules.Users.Infrastructure/

Future modules may include:
- Identity
- Authorization
- FeatureManagement
- Notifications
- Audit

## 21. `Mavrynt.Modules.Users.Domain`

### Responsibility

- user entities,
- value objects,
- business rules,
- domain events,
- repository abstractions.

### Example folders

- `Entities`
- `ValueObjects`
- `Enums`
- `Events`
- `Repositories`

## 22. `Mavrynt.Modules.Users.Application`

### Responsibility

- user module use cases,
- commands and queries,
- DTOs,
- module-level orchestration,
- integration between domain logic and the host.

### Example folders

- `Abstractions`
- `Commands`
- `Queries`
- `DTOs`
- `DependencyInjection`

## 23. `Mavrynt.Modules.Users.Infrastructure`

### Responsibility

- persistence implementation,
- EF or other data layer configuration,
- repository implementations,
- technical integration details of the module.

### Example folders

- `Persistence`
- `Configuration`
- `Repositories`
- `DependencyInjection`

## 24. `src/frontend`

A folder containing frontend applications.

Target structure:

    src/frontend/
    ├── mavrynt-web/
    ├── mavrynt-admin/
    └── mavrynt-landing/

## 25. `src/frontend/mavrynt-web`

The main end-user frontend.

### Responsibility

- end-user interface,
- integration with the main product API,
- user-facing functionality.

## 26. `src/frontend/mavrynt-admin`

The administrative frontend.

### Responsibility

- administration interface,
- system management,
- user, flag, and configuration management.

## 27. `src/frontend/mavrynt-landing`

The static marketing landing page.

### Responsibility

- marketing website,
- product value communication,
- promotional content,
- independent evolution from the application area.

### Rules

- the landing page should remain lightweight,
- it should not depend on backend business logic,
- any integration with the backend should be minimal and explicit.

## 28. `src/shared`

A folder intended for assets shared between different parts of the product, only if they do not more naturally belong to backend or frontend.

It may contain, for example:
- shared configuration artifacts,
- platform-independent definitions,
- assets shared between applications.

### Rules

- `shared` must not become a dumping ground,
- before adding anything here, check whether it belongs more naturally to a specific project.

## 29. `tests`

The solution test folder.

Target layout:

    tests/
    ├── backend/
    ├── frontend/
    ├── integration/
    └── architecture/

## 30. `tests/backend`

Backend unit and module tests.

### Responsibility

- domain layer tests,
- application layer tests,
- business logic tests.

## 31. `tests/frontend`

Frontend tests.

### Responsibility

- component tests,
- frontend logic tests,
- UI tests where needed.

## 32. `tests/integration`

Integration tests for the solution or selected parts of it.

### Responsibility

- host integration tests,
- database tests,
- cross-layer flow tests,
- tests using runtime dependencies.

## 33. `tests/architecture`

Architectural tests.

### Responsibility

- protecting dependency rules,
- detecting forbidden references,
- validating structure against architectural assumptions.

These tests are especially important in a modular monolith.

## 34. Naming conventions

### 34.1. Backend projects

Backend project names should use the prefix:
- `Mavrynt.*`

### 34.2. Modules

Module project names should use the pattern:
- `Mavrynt.Modules.{ModuleName}.Domain`
- `Mavrynt.Modules.{ModuleName}.Application`
- `Mavrynt.Modules.{ModuleName}.Infrastructure`

### 34.3. BuildingBlocks

Shared project names should use the pattern:
- `Mavrynt.BuildingBlocks.*`

### 34.4. Frontends

Frontend folders may use simpler technical names:
- `mavrynt-web`
- `mavrynt-admin`
- `mavrynt-landing`

## 35. Repository hygiene rules

The following rules should be enforced:
- do not place business logic in hosts,
- do not place random helpers in top-level folders,
- do not mix frontend and backend code,
- do not use `shared` as a generic storage area,
- every new module should keep the same structural pattern,
- every major structural change should be reflected in documentation.

## 36. Current implementation state

At the current stage, the repository is being built manually from an empty `.sln`. This means some folders and projects may already exist, while others are still planned. This document describes both the current state and the intended target direction.

## 37. Summary

The Mavrynt repository structure has been designed to:
- support a modular monolith,
- preserve order and predictability,
- allow the whole product to evolve in a single repository,
- work well with manual solution setup,
- support collaboration with multiple AI tools and agents.

Every new element added to the repository should follow the rule of clear responsibility and alignment with the overall architecture of the solution.