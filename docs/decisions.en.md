# Mavrynt — Architecture Decision Log

## Purpose of this document

This document collects the most important architectural and organizational decisions related to the Mavrynt solution. Each decision should be recorded in a concise but unambiguous way so that in the future it is clear:
- what was decided,
- why the decision was made,
- what consequences follow from that decision.

This is a living document and should be extended whenever important changes are made.

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

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

---

## ADR-015 — Marketing landing SPA: independent lifecycle with shared tooling

**Status:** Accepted  
**Date:** 2026-04-19

### Decision
The marketing landing page (`mavrynt-landing`) ships as an independent React + Vite SPA that:
- is orchestrated by `Mavrynt.AppHost` during local development for a consistent launch experience,
- deploys to any static host (CDN / object storage) without a runtime dependency on `Mavrynt.Api`,
- consumes shared frontend tooling (`@mavrynt/design-tokens`, `@mavrynt/ui`, `@mavrynt/config`, `@mavrynt/eslint-config`, `@mavrynt/tsconfig-base`) from the internal `src/frontend/shared/*` workspace,
- uses ports-and-adapters for every cross-cutting concern (analytics, lead submission, feature flags), with console / no-op adapters as the default runtime.

### Rationale
The landing page is the product's public front door and has a different failure domain than the backend:
- it must stay live even when the API is in maintenance,
- its release cadence follows marketing needs, not backend deploys,
- it benefits from the cheapest possible hosting model (CDN / static) and aggressive caching.

At the same time, forking the landing away from the monorepo would dilute design consistency. Sharing tokens, UI primitives, ESLint, and tsconfig packages keeps visual and code-quality standards identical across all SPAs. Ports-and-adapters preserve the option to integrate with `Mavrynt.Api` later (an HTTP `LeadService` adapter) without rewriting any components.

### Consequences
- the landing must not import anything from `src/backend/*`; integration happens only through adapters,
- all lead-capture paths must go through the `LeadService` port — direct `fetch` calls inside components are not allowed,
- the landing test pyramid lives inside the app: Vitest (unit + integration, jsdom) and Playwright (Chromium smoke for key journeys),
- static assets are pre-compressed (gzip + brotli) at build time; the CDN / host is expected to serve them,
- new shared frontend concepts (e.g., a UI primitive) go into `src/frontend/shared/*`, not into any individual SPA,
- WCAG 2.1 AA is the accessibility baseline; regressions in labels, landmarks, or focus management should break the e2e smoke suite.

---

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

---

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

---

## Summary

This document is the architectural decision register for Mavrynt. It is intended to preserve consistency while the solution is being built manually and extended over time. Every significant decision that changes architectural direction should be added here.