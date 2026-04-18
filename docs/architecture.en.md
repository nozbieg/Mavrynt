# Mavrynt — Solution Architecture

## 1. Purpose of this document

The purpose of this document is to describe the target architecture of the Mavrynt solution at the repository, component, project responsibility, and dependency rule levels. This document is intended to serve as a shared reference point for further implementation performed manually and with the support of AI agents.

The architecture has been designed to:
- enable incremental product development,
- preserve high code readability and clear responsibilities,
- support a modular monolith as the initial operating model,
- remain ready for future expansion with additional domain modules,
- support continuous delivery, observability, testability, and feature management.

---

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

---

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

---

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

---

## 5. Main solution components

## 5.1. `Mavrynt.AppHost`

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

## 5.2. `Mavrynt.ServiceDefaults`

A project containing shared service settings and technical standards. Example responsibilities:
- observability configuration,
- shared hosting extensions,
- standard health checks,
- shared telemetry settings,
- common technical conventions for backend services.

## 5.3. `Mavrynt.Api`

The main product API host. It is responsible for:
- exposing application endpoints,
- composing modules,
- configuring the HTTP pipeline,
- authentication and authorization at the host level,
- integrating backend modules.

This is the main entry point for the user-facing client.

## 5.4. `Mavrynt.AdminApp`

A backend host dedicated to the administrative area. It is responsible for:
- administrative functions,
- user management,
- role and permission management,
- feature flag management,
- future administrative system areas.

This is a separate host because the admin area has distinct security, permission, and responsibility requirements.

## 5.5. `Mavrynt.BuildingBlocks.*`

A set of shared projects for the entire backend.

### `Mavrynt.BuildingBlocks.Domain`
Base code for the domain layer, for example:
- entity base classes,
- value object patterns,
- domain errors,
- domain abstractions.

### `Mavrynt.BuildingBlocks.Application`
Base code for the application layer, for example:
- command/query abstractions,
- shared use case interfaces,
- validation and pipeline behaviors,
- dependency registration patterns.

### `Mavrynt.BuildingBlocks.Infrastructure`
Shared infrastructure code, for example:
- persistence mechanisms,
- configuration extensions,
- integration building blocks,
- technical implementation patterns for data access.

### `Mavrynt.BuildingBlocks.Contracts`
Shared contracts, for example:
- integration events,
- request/response contracts,
- cross-module or cross-layer messages.

## 5.6. `Mavrynt.Modules.*`

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

---

## 6. Layer model inside a module

Each backend module should aim for a consistent layered model.

## 6.1. Domain layer

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

## 6.2. Application layer

The application layer contains:
- use cases,
- commands,
- queries,
- DTOs,
- input and output contracts,
- orchestration of module logic.

The application layer uses the domain, but should not take responsibility for persistence details or hosting concerns.

## 6.3. Infrastructure layer

The infrastructure layer contains:
- repository implementations,
- database configuration,
- mappings,
- external integrations,
- technical configuration of the module.

This is where technology-specific details belong.

## 6.4. Host

The host:
- registers modules,
- exposes endpoints,
- wires environment configuration,
- handles cross-cutting concerns at the application level,
- should not contain domain logic.

---

## 7. Dependency rules

## 7.1. Allowed dependencies

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

## 7.2. Forbidden dependencies

The following must not be allowed:
- `Domain` → `Infrastructure`
- `Domain` → host
- `Application` → host
- uncontrolled direct dependency from module A to implementation details of module B
- frontend depending directly on backend projects
- marketing landing page depending on backend domain logic

---

## 8. Frontend and presentation layer

The repository is expected to contain three main frontend applications:

### `mavrynt-web`
The main end-user frontend.

### `mavrynt-admin`
The administrative frontend.

### `mavrynt-landing`
A static marketing landing page without dependency on the backend domain model. It may use only minimal integration mechanisms or remain entirely static.

Frontends are maintained separately from the backend, but within the same repository.

---

## 9. Data and infrastructure

The target architecture assumes the use of:
- PostgreSQL as the primary database,
- Redis as cache and supporting infrastructure,
- RabbitMQ and Kafka as asynchronous communication components where needed,
- Docker and Compose as the basis for local environments and technical deployment flows.

At the current stage, this document describes the architectural direction rather than the full infrastructure implementation.

---

## 10. Feature flags

The feature flag mechanism is an important part of the architecture. The assumptions are:
- feature flags should be supported from the beginning,
- flag management should be available from the administrative area,
- feature flags should support controlled rollout of functionality,
- the architecture should allow conditional activation per environment, user segment, or other segmentation strategy.

Feature flags are treated as a foundational Continuous Delivery capability, not as an optional add-on.

---

## 11. Observability

The architecture assumes full observability from the beginning. This includes:
- logging,
- metrics,
- traces,
- health checks,
- environment diagnostics.

Shared observability standards should be maintained centrally, mainly through `Mavrynt.ServiceDefaults` and shared technical layers.

---

## 12. Testability

The solution should be designed to support:
- unit tests for modules,
- integration tests for hosts and infrastructure layers,
- architectural tests for dependency rules,
- frontend tests for client applications.

Tests are treated as a first-class part of the repository, not as an afterthought.

---

## 13. Continuous Delivery

The repository and project architecture should support full Continuous Delivery. This includes:
- predictable build structure,
- repeatable environment startup,
- the ability to validate applications independently,
- safe rollout using feature flags,
- a clear split of environment-specific configuration.

---

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

---

## 15. Summary

Mavrynt is being built as a modular monolith in a single repository, with a clear split between hosts, building blocks, domain modules, frontends, and infrastructure assets. The main goal of the architecture is to preserve order, organizational and technical scalability, and readiness for future growth without unnecessary initial complexity.

This document serves as a reference baseline for further implementation decisions and should be updated as the solution evolves.