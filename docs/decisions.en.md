# Mavrynt — Architecture Decision Log

## Purpose of this document

This document collects the most important architectural and organizational decisions related to the Mavrynt solution. Each decision should be recorded in a concise but unambiguous way so that in the future it is clear:
- what was decided,
- why the decision was made,
- what consequences follow from that decision.

This is a living document and should be extended whenever important changes are made.
**This file is the canonical ADR log.** Do not edit accepted ADRs in place; mark them `Superseded` and add a new ADR.

---

## Index

| ID | Title | Status | Date | Relevance |
|---|---|---|---|---|
| ADR-001 | Solution model: modular monolith | Accepted | 2026-04-18 | Architectural style baseline |
| ADR-002 | Single repository for the whole product | Accepted | 2026-04-18 | Repo organization |
| ADR-003 | Separation of backend hosts | Accepted | 2026-04-18 | Hosts: Api / AdminApp / AppHost / ServiceDefaults |
| ADR-004 | Separate administrative area | Accepted | 2026-04-18 | Why AdminApp is its own host |
| ADR-005 | Layered module structure | Accepted | 2026-04-18 | Domain / Application / Infrastructure per module |
| ADR-006 | Shared BuildingBlocks projects | Accepted | 2026-04-18 | What lives in BuildingBlocks |
| ADR-007 | First foundational module: Users | Accepted | 2026-04-18 | Users is the template module |
| ADR-008 | Feature flags as a core architectural capability | Accepted | 2026-04-18 | Flags from day one, AdminApp-managed |
| ADR-009 | Full observability from the start | Accepted | 2026-04-18 | Observability lives in `ServiceDefaults` |
| ADR-010 | Frontends in the same repository, separated from the backend | Accepted | 2026-04-18 | Frontend ↔ backend boundary |
| ADR-011 | PostgreSQL as the primary database | Accepted | 2026-04-18 | Persistence default |
| ADR-012 | Redis, RabbitMQ, and Kafka reserved in the architecture | Accepted | 2026-04-18 | Reserved, not active |
| ADR-013 | Manual solution setup from a clean `.sln` | Accepted | 2026-04-18 | Why structure is hand-curated |
| ADR-014 | Architecture documentation stored inside the repository | Accepted | 2026-04-18 | Docs live with the code |
| ADR-015 | Marketing landing SPA: independent lifecycle with shared tooling | Accepted | 2026-04-19 | Landing decoupled from backend |
| ADR-016 | Cross-app URL resolution via `@mavrynt/config/app-urls` | Accepted | 2026-04-20 | Cross-SPA navigation rule |
| ADR-017 | `@mavrynt/auth-ui` as a separate shared package | Accepted | 2026-04-20 | Auth UI is not in `@mavrynt/ui` |
| ADR-018 | BuildingBlocks baseline without mediator lock-in | Accepted (superseded by ADR-020 for the mediator decision) | 2026-04-27 | Sets foundation; deferred mediator choice |
| ADR-019 | Users module Domain and Application baseline | Accepted | 2026-04-27 | Users domain + application contracts |
| ADR-020 | Internal mediator and application pipeline behaviors | Accepted | 2026-04-28 | Internal `MavryntMediator`; supersedes ADR-018's mediator deferral |
| ADR-021 | Backend test strategy: architecture, unit, Testcontainers integration | Accepted | 2026-04-28 | Test pyramid in place |
| ADR-022 | Phase 1 Admin Foundation Slice: roles, FeatureManagement, Audit | Accepted | 2026-04-29 | Admin vertical slice complete |
| ADR-023 | Notifications module: DB-backed SMTP, templates, `IEmailNotificationService` | Accepted | 2026-04-30 | Email module complete |
| ADR-024 | Default local SMTP seed and per-configuration test email | Accepted | 2026-05-04 | SMTP foundation ready for password reset by email |
| ADR-025 | Pipeline-owned commit: ITransactionalRequest + multi-UoW behavior | Accepted | 2026-05-04 | Mutating commands persist via TransactionBehavior; repositories no longer self-save |

### ADR-018 ↔ ADR-020 relationship

ADR-018 deliberately deferred the mediator choice and shipped library-neutral
command/query/handler interfaces. ADR-020 later **accepted the internal
`MavryntMediator`**, replacing only that deferred decision. The library-neutral
interfaces from ADR-018 remain valid; only the dispatch strategy moved from
"deferred" to "internal mediator".

### Note on `docs/adr/` files

A separate folder `docs/adr/` contains detailed implementation notes that were
written with their own ADR numbers (`ADR-020-jwt-bearer-authentication.md`,
`ADR-021-efcore-postgresql-persistence.md`, `ADR-022-shared-users-module-admin-policy.md`,
`ADR-023-audit-trail-design.md`). Those numbers **collide** with the ADRs in
this file but cover different topics. **This file (`docs/decisions.en.md`) is the
canonical ADR log.** The standalone files in `docs/adr/` should be treated as
detailed implementation notes for the corresponding subsystem (auth, persistence,
audit). Renumbering of `docs/adr/*.md` to remove the collision is tracked in
`docs/next-work.md`.

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

## ADR-016 — Cross-app URL resolution via `@mavrynt/config/app-urls`

**Status:** Accepted  
**Date:** 2026-04-20

### Decision
All cross-SPA navigation (e.g., `mavrynt-landing` → `/login` on `mavrynt-web`, `mavrynt-admin` → the user-facing SPA, any footer escape-hatches) resolves absolute URLs through a single helper: `resolveAppUrls()` exported from `@mavrynt/config/app-urls`. The helper:
- takes an optional env-source argument (defaults to `import.meta.env`) so it is testable in Node without Vite,
- reads canonical keys `VITE_APP_URL_LANDING`, `VITE_APP_URL_WEB`, `VITE_APP_URL_ADMIN` per deployment,
- accepts legacy aliases `VITE_MARKETING_URL`, `VITE_WEB_URL`, `VITE_ADMIN_URL` for backwards compatibility with Phase 2 / Phase 3 env wiring,
- falls back to per-app Vite dev ports (`:5173`, `:5174`, `:5175`) when nothing is set,
- normalises trailing slashes and freezes the result so callers can safely append paths.

### Rationale
Inline `VITE_*_URL` literals scattered across nav and footer components drift over time (per-app rename, per-env override, per-tier domain). Concentrating URL resolution in one module:
- gives one place to change deployment topology,
- makes a cross-cutting change (e.g., adding a fourth SPA) mechanical rather than a grep-everywhere exercise,
- isolates env-variable access so the rest of the code stays pure and unit-testable,
- mirrors the ports-and-adapters stance taken for analytics, lead capture, and auth (`ADR-015`).

### Consequences
- no frontend component may read `VITE_*_URL` env vars directly — navigation always goes through `resolveAppUrls()` / `resolveAppUrl(app)`,
- new deployments set `VITE_APP_URL_*` in their Vite env file; the legacy keys are tolerated for one release cycle, then retired,
- tests inject a plain `Record<string, string | undefined>` as the env source — no global state, no `vi.stubEnv` gymnastics,
- the helper lives in `@mavrynt/config` (not `@mavrynt/ui`) because it is runtime configuration, not presentation.

---

## ADR-017 — `@mavrynt/auth-ui` as a separate shared package

**Status:** Accepted  
**Date:** 2026-04-20

### Decision
Authentication UI (login + register forms, the `AuthService` port, the default console adapter, the HTTP adapter factory, and the bilingual auth i18n bundle) lives in its own workspace package, `@mavrynt/auth-ui`, rather than inside `@mavrynt/ui`.

### Rationale
`@mavrynt/ui` is a presentational library: buttons, sections, navbar, footer, no domain semantics. Auth, by contrast, deals with sessions, credentials, typed error codes (`invalid_credentials`, `email_taken`, `rate_limited`, …), and the abstraction over the backend auth endpoint. Putting that domain into `@mavrynt/ui` would:
- force every consumer of primitive buttons to pay the auth dependency cost,
- blur the boundary between presentation and domain — every future decision ("should this go into `ui` or somewhere else?") would get murky,
- conflict with WCAG-driven review scope: auth flows have a stricter review bar than marketing visuals.

Keeping the two packages separate lets each evolve on its own cadence. `@mavrynt/ui` stays small, cheap to depend on, and easy to reason about. `@mavrynt/auth-ui` owns its port, its adapters, and its i18n resources — swap in a different backend adapter by changing the `AuthServiceContext` provider, not by editing UI code.

### Consequences
- `@mavrynt/auth-ui` depends on `@mavrynt/ui` (not the reverse) and exposes forms, the service port, the analytics port, and the i18n resources,
- consuming apps (`mavrynt-web`, `mavrynt-admin`) register the auth i18n bundle under namespace `"auth"` in their i18n bootstrap,
- apps inject their own `AuthService` (`createConsoleAuthService` in dev, `createHttpAuthService` against `Mavrynt.Api` / `Mavrynt.AdminApp` in prod) via `AuthServiceContext.Provider`,
- route-level feature flags (`admin.register.enabled`) gate the admin registration surface without touching `@mavrynt/auth-ui`,
- tests for the forms, hooks, and adapters live inside the consuming SPA's Vitest suite (primarily `mavrynt-web`) — the package itself is source-shipped and has no independent runner.

---

## ADR-018 — BuildingBlocks baseline without mediator lock-in

**Status:** Accepted  
**Date:** 2026-04-27

### Decision
The four BuildingBlocks projects have been given their foundational implementation:
- `Mavrynt.BuildingBlocks.Domain` — domain primitives: `IEntity<TId>`, `IAggregateRoot`, `IDomainEvent`, `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Error`, `Result`, `Result<T>`.
- `Mavrynt.BuildingBlocks.Application` — application abstractions: command/query marker interfaces, handler interfaces returning `Result`/`Result<T>`, behavior marker interfaces (validation, logging, transaction), `IDateTimeProvider`, `ICurrentUserContext`, and a skeleton DI extension point.
- `Mavrynt.BuildingBlocksContracts` — integration contracts: `IIntegrationEvent`, `IntegrationEvent`, `IRequestContract`, `IResponseContract`, `PagedResponse<T>`.
- `Mavrynt.BuildingBlocks.Infrastructure` — infrastructure abstractions: `IUnitOfWork`, `IRepository<TEntity, TId>`, `PostgreSqlOptions`, `ConfigurationExtensions`.

The implementation intentionally does not introduce MediatR, FluentValidation, EF Core, Npgsql, or any concrete persistence implementation.

### Rationale
The Users module (and any future module) requires stable base types to build on without those types being entangled with a specific mediator, validator, or ORM choice. Locking into MediatR or FluentValidation at this stage would force those dependencies on every module before there is enough context to make a well-informed choice. The command/query/handler interfaces defined here are library-neutral and can be adapted to any pipeline implementation later. Keeping the domain layer free of all infrastructure and framework dependencies ensures it remains portable and easily testable.

### Consequences
- all domain entities must extend `Entity<TId>` or `AggregateRoot<TId>` from BuildingBlocks.Domain,
- all application use-cases must implement `ICommandHandler` or `IQueryHandler` from BuildingBlocks.Application,
- integration events between modules must extend `IntegrationEvent` from BuildingBlocks.Contracts,
- concrete mediator selection (MediatR, Wolverine, or a custom dispatcher) remains an open decision,
- concrete validation library (FluentValidation or DataAnnotations) remains an open decision,
- concrete EF Core DbContext and migration strategy remain open decisions,
- `BuildingBlocks.Infrastructure` must not be used as a general utility dump — only genuinely cross-module infrastructure abstractions belong there.

---

## ADR-019 — Users module Domain and Application baseline

**Status:** Accepted  
**Date:** 2026-04-27

### Decision
`Mavrynt.Modules.Users` is the first concrete module built on top of the BuildingBlocks baseline.

This stage delivers two layers only:

**Domain (`Mavrynt.Modules.Users.Domain`)**
- Value objects: `UserId`, `Email`, `PasswordHash`, `UserDisplayName` — all immutable, all use `Result<T>` for creation, no exceptions on validation failure.
- Aggregate: `User` (`AggregateRoot<UserId>`) — static `Register` factory, behavior methods (`ChangeEmail`, `ChangePasswordHash`, `ChangeDisplayName`, `Activate`, `Deactivate`), ORM-compatible private parameterless constructor.
- Enum: `UserStatus` (`Active`, `Inactive`, `Locked`).
- Domain events: `UserRegisteredDomainEvent`, `UserEmailChangedDomainEvent`, `UserPasswordChangedDomainEvent` — immutable records implementing `IDomainEvent`.
- Repository abstraction: `IUserRepository` — four methods, no implementation.
- Errors: `UserErrors` — strongly named `Error` constants, no string literals scattered in handlers.

**Application (`Mavrynt.Modules.Users.Application`)**
- DTOs: `UserDto`, `AuthResultDto` — plain records exposing primitives only, no domain types in the public contract.
- Commands: `RegisterUserCommand`, `LoginUserCommand`, `ChangeUserEmailCommand`, `ChangeUserPasswordCommand` — immutable records.
- Command handlers: one class per command, depend only on `IUserRepository` and `IDateTimeProvider` abstractions.
- Queries: `GetUserByIdQuery`, `GetUserByEmailQuery` — immutable records.
- Query handlers: fetch from `IUserRepository`, map to `UserDto`, never expose domain entities.
- Mapping: `UserMappings` — `internal static`, extension method `ToDto()`, no AutoMapper or Mapster.
- DI: `AddUsersApplication(IServiceCollection)` — registers handlers by interface; does not register infrastructure services.

### Rationale
Defining the domain model and use-case contracts before Infrastructure allows the persistence and API layers to be added independently in a future stage. Keeping the domain persistence-ignorant and the application free of Infrastructure references enforces the dependency rule and makes both layers unit-testable in isolation with a repository stub.

Strongly named errors (`UserErrors`) eliminate magic strings from handlers and provide a single source of truth for error codes consumed by future API and frontend layers.

`LoginUserCommand` is intentionally minimal: it compares pre-hashed values only and returns `AuthResultDto` with `TokenType: "not_implemented"`. The token/session/cookie strategy is a separate architectural decision deferred to the Infrastructure + API stage.

### Consequences
- `Mavrynt.Modules.Users.Domain` does not reference Application, Infrastructure, ASP.NET, or EF Core.
- `Mavrynt.Modules.Users.Application` does not reference Infrastructure or hosts.
- No concrete persistence is implemented.
- No API endpoint is exposed.
- No authentication session, JWT, cookie, or refresh token is implemented.
- No mediator library is introduced.
- No validation library is introduced.
- Infrastructure (EF Core entity configuration, DbContext, migrations, PostgreSQL wiring), API endpoints, password hashing library, JWT/cookie strategy, role model, and admin management are all deferred to future stages.

---

## ADR-020 — Internal mediator and application pipeline behaviors

**Status:** Accepted
**Date:** 2026-04-28

### Decision

Mavrynt uses a lightweight internal mediator (`MavryntMediator`) instead of MediatR or any external mediator library.

Commands and queries are dispatched through `IMediator`:

```csharp
Task<Result> SendAsync(ICommand command, CancellationToken ct = default);
Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
```

A generic pipeline behavior interface is introduced as the single extension point for cross-cutting concerns:

```csharp
public interface IMavryntBehavior<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken);
}
```

Five built-in behaviors are registered in this order:

1. **LoggingBehavior** — logs request type, category, elapsed time, success/failure, error code, and trace ID. Never serializes the full request.
2. **ValidationBehavior** — resolves all `IValidator<TRequest>` from DI and stops on first failure.
3. **ResilienceBehavior** — pass-through hook for retry/timeout; respects `IResilientRequest` marker. No Polly dependency at this stage.
4. **AuditBehavior** — writes an `AuditEntry` via `IAuditService` for requests implementing `IAuditableRequest`.
5. **TransactionBehavior** — calls `IUnitOfWork.SaveChangesAsync()` after handler success for requests implementing `ITransactionalRequest`.

Request markers control optional behavior activation:
- `IAuditableRequest` — enables audit entry emission
- `IResilientRequest` — declares retry-safe requests
- `ITransactionalRequest` — enables unit-of-work commit on success

The existing `ICommand`, `ICommand<TResponse>`, `IQuery<TResponse>`, and their handler interfaces are preserved unchanged. Handlers continue to return `Result` or `Result<TResponse>`.

`Result` and `Error` remain the standard return model throughout the application layer — behaviors are aware of this contract.

Handler discovery and DI registration are provided by a single extension method:

```csharp
services.AddMavryntMediator(params Assembly[] assemblies);
```

Endpoints inject `IMediator`, not concrete handler interfaces.

### Rationale

- **Full control over Result/Error handling.** No adapter layer between `Result<T>` and an external mediator's response model.
- **No external mediator dependency.** Reduces supply-chain risk and version coupling.
- **Easy AI-agent readability.** All behavior logic lives in Mavrynt's own namespace with explicit XML documentation.
- **Clean extension point.** Adding a new cross-cutting concern is one class implementing `IMavryntBehavior<TRequest, TResponse>` and one line in DI registration.
- **Future replacement is safe.** All call sites depend on the internal `IMediator` abstraction, not on any external type. Swapping the implementation requires no changes in handlers or endpoints.
- **Explicit pipeline order.** Order is determined by DI registration in `AddMavryntMediator`, visible in one file, not scattered across attributes or configuration.

### Consequences

- `IMediator` is the only entry point for command and query dispatch in endpoints and application services.
- MediatR is not added to the solution. Any future decision to adopt it requires an explicit ADR.
- FluentValidation is not introduced. The internal `IValidator<TRequest>` abstraction is used. A future ADR may replace it.
- Polly is not introduced for the resilience behavior. A future ADR may add it when retry policies are needed.
- The three legacy behavior marker interfaces (`ILoggingBehavior`, `IValidationBehavior`, `ITransactionBehavior`) are marked `[Obsolete]`. They will be removed in a future cleanup.
- `IUnitOfWork` is defined in both `Mavrynt.BuildingBlocks.Application.Persistence` (canonical abstraction) and `Mavrynt.BuildingBlocks.Infrastructure.Persistence` (Infrastructure marker extending the Application interface). Concrete EF Core implementations must register against the Application interface.
- Unhandled exceptions in the pipeline are caught by the mediator, logged, and returned as `Result.Failure` with a trace ID. They do not propagate as exceptions to the API boundary.

---

## ADR-021 — Backend test strategy: architecture, unit and Testcontainers integration tests

**Status:** Accepted  
**Date:** 2026-04-28

### Decision
The backend test foundation is standardized into three layers:
- architecture tests for modular dependency boundaries,
- unit tests for command/query handlers using fakes,
- integration tests using real PostgreSQL through Testcontainers.

### Rationale
This strategy protects architecture drift early, keeps application logic deterministic in unit scope, and validates persistence/runtime integration without external shared environments.

### Consequences
- architecture tests become a gate for modular dependency direction,
- command/query handlers are tested in isolation with in-memory doubles,
- infrastructure and API/Admin integration tests run against PostgreSQL containers,
- tests do not depend on Aspire AppHost or Docker Compose,
- package versions remain centrally managed in `Directory.Packages.props`,
- backend tests become part of the Continuous Delivery foundation.

---

## ADR-022 — Phase 1 Admin Foundation Slice: role management, FeatureManagement, and persistent audit

**Status:** Accepted  
**Date:** 2026-04-29

### Decision

The first administrative vertical slice of Phase 1 is implemented and wired into `Mavrynt.AdminApp`. The slice delivers three capabilities:

**1. User role assignment**
- `PATCH /api/admin/users/{userId}/role` — assigns `Admin` or `User` role to an existing user.
- Protected by the `AdminOnly` authorization policy (JWT Bearer + `Admin` role claim).
- Implemented as `AssignUserRoleCommand` in `Mavrynt.Modules.Users.Application`.
- Audit trail written via the existing `IAuditService` (BuildingBlocks) with event type `user_role_assigned`.

**2. FeatureManagement module**
- New module: `Mavrynt.Modules.FeatureManagement.Domain`, `…Application`, `…Infrastructure`.
- `FeatureFlag` aggregate: key (`^[a-z0-9][a-z0-9._-]*$`, max 256 chars), name, description, enabled/disabled state, created/updated timestamps.
- CRUD commands: `CreateFeatureFlagCommand`, `UpdateFeatureFlagCommand`, `ToggleFeatureFlagCommand`.
- Queries: `ListFeatureFlagsQuery`, `GetFeatureFlagByKeyQuery`.
- Endpoints in AdminApp: `GET/POST /api/admin/feature-flags`, `GET/PATCH /api/admin/feature-flags/{key}`, `PATCH /api/admin/feature-flags/{key}/toggle`.
- All endpoints are `AdminOnly` protected.
- Feature flags are managed exclusively through AdminApp — no user-facing read endpoints are exposed at this stage.
- Persistence: PostgreSQL schema `feature_management`, table `feature_flags`, EF Core migrations, unique index on `key`.

**3. Audit module**
- New module: `Mavrynt.Modules.Audit.Domain`, `…Application`, `…Infrastructure`.
- `AuditLogEntry` entity — append-only record: actor user id, action, resource type, resource id, occurred at, metadata JSON.
- `IAuditLogWriter` (Audit.Application) — new abstraction for admin/system audit writes, separate from `IAuditService` (BuildingBlocks) which handles auth-level events.
- `FeatureManagement.Application` depends on `Audit.Application` for `IAuditLogWriter`.
- Persistence: PostgreSQL schema `audit`, table `audit_log_entries`, EF Core migrations.

**Test coverage added**
- Architecture tests: 6 new tests for FM and Audit layer dependency rules.
- Domain unit tests: 24 new tests for `FeatureFlag` aggregate and value objects.
- Application unit tests: 12 new tests for FM command/query handlers (fakes, no I/O).
- Application unit tests: 5 new tests for `AssignUserRoleCommandHandler`.
- Infrastructure integration tests: 6 new tests for `FeatureFlagRepository` against a real PostgreSQL container.
- AdminApp integration tests: 5 new tests for user role endpoint; 11 new tests for feature flag endpoints.

### Rationale

The administrative slice closes the gap between the backend foundation (Users, JWT, mediator, tests) and a working admin backend. Delivering role assignment, feature flag management, and audit in the same slice ensures that every administrative mutation is observable from day one, without accumulating audit debt.

Separating `IAuditLogWriter` from the existing `IAuditService` keeps BuildingBlocks free of module-specific audit semantics. `IAuditLogWriter` belongs to `Audit.Application` — the one module that owns the audit concern.

FeatureManagement is placed under AdminApp only at this stage. Exposing flags to user-facing APIs or using them as runtime gate checks are separate future decisions.

### Consequences

- `AdminOnly` policy is the only authorization model for admin endpoints; full RBAC (fine-grained permissions per resource type) is not implemented.
- Actor `userId` is not yet propagated to `IAuditLogWriter.WriteAsync()` — all admin audit entries are written with `actorUserId: null`. Propagation via `ICurrentUserContext` is deferred to a future slice.
- `FeatureManagement.Application` has a deliberate cross-module dependency on `Audit.Application`. This is the only permitted cross-module application-layer dependency and must not be extended without a new ADR.
- `IDateTimeProvider` is registered with `TryAddSingleton` in FeatureManagement.Infrastructure so the first registration (Users.Infrastructure) wins when both modules are active in the same host.
- CI/CD pipeline configuration, staging environments, and deployment automation remain deferred to a later stage.

---

## ADR-023 — Notifications module: database-backed SMTP, template engine, and IEmailNotificationService

**Status:** Accepted  
**Date:** 2026-04-30

### Decision

A new `Notifications` module (`Domain`, `Application`, `Infrastructure`) is added to handle all outbound email communication. Key design points:

**SMTP configuration**
- `SmtpSettings` aggregate stores provider name, host, port, credentials, sender identity, SSL flag, and enabled state in the `notifications.smtp_settings` PostgreSQL table.
- Exactly one provider is active at any time; enabling a new one disables all others (`DisableAllAsync` before enabling the target).
- Passwords are stored via `ISecretProtector`. The default implementation (`PassThroughSecretProtector`) is a dev-only pass-through. It must be replaced with DPAPI, Azure Key Vault, or similar before production.
- Passwords are never returned in DTOs, never logged, and never included in audit entries.

**Email templates**
- Three predefined templates are seeded on startup: `auth.login_confirmation`, `auth.password_reset`, `auth.two_factor_code`.
- Templates are stored in `notifications.email_templates` with a unique constraint on `template_key`.
- Template content can be updated by administrators but keys are immutable. Templates can be disabled individually.
- The seeder is idempotent — it never overwrites existing templates.

**Template rendering**
- `EmailTemplateRenderer` (Application layer, no infrastructure dependencies) resolves `{{Placeholder}}` tokens using a regex.
- HTML body values are HTML-encoded via `WebUtility.HtmlEncode`; subject and text body values are used as-is.
- Unknown placeholders return `NotificationsErrors.EmailUnknownPlaceholder(key)` — no silent substitution.

**IEmailNotificationService**
- The generic `SendAsync<TModel>(key, recipient, model, ct)` method is the single public contract for cross-module email dispatch.
- `TModel : IEmailModel` provides a `ToPlaceholders()` dictionary, keeping caller code free of string-manipulation details.
- Pre-built model types cover all three predefined templates; future templates require a new model type.

**Infrastructure**
- `SmtpEmailSender` uses `System.Net.Mail.SmtpClient` (BCL only, no third-party SMTP library).
- Migrations are written manually following the FeatureManagement pattern.
- `NotificationsStartupService` (IHostedService) runs database migration then template seeding in sequence on startup.

**Admin endpoints**
- SMTP settings: list, get-by-id, create, update, enable (`/api/admin/notifications/smtp-settings`).
- Email templates: list, get-by-key, update, list-definitions, send-test (`/api/admin/notifications/email`).
- All endpoints are `AdminOnly` protected; no user-facing read endpoints are exposed.

### Rationale

Email notification is a core cross-cutting capability needed by the Users module (login confirmation, password reset, 2FA). Defining `IEmailNotificationService` in Notifications.Application and consuming it from other modules keeps the sending concern isolated. Using BCL `SmtpClient` avoids a third-party dependency at this stage while remaining easy to replace. Placing `EmailTemplateRenderer` in Application (rather than Infrastructure) allows unit-testing the rendering logic without any database or SMTP connection. `ISecretProtector` provides a safe extension point for production-grade encryption without coupling the domain to any specific secret store.

### Consequences

- `Notifications.Application` has a cross-module dependency on `Audit.Application` (for `IAuditLogWriter`) — the same pattern established in ADR-022. No other cross-module application-layer dependencies are permitted without a new ADR.
- `IDateTimeProvider` is registered with `TryAddSingleton` in `NotificationsInfrastructure` so the first registration wins when multiple modules are active.
- `PassThroughSecretProtector` must be replaced with a real encryption implementation before the application handles real SMTP credentials in a non-development environment.
- Template keys are immutable once seeded. Adding a fourth template requires a new migration, a new seeder entry, a new `IEmailModel` implementation, and a new `EmailTemplateKey` constant.
- `SmtpClient` (BCL) does not support OAuth or modern authentication flows. Replacing it with MailKit or a transactional email API adapter requires only a new `IEmailSender` implementation — no other code changes.

---

## ADR-024 — Default local SMTP seed and per-configuration test email

**Status:** Accepted  
**Date:** 2026-05-04

### Decision

Two small additions to the Notifications module to prepare the SMTP foundation
required by the upcoming password-reset-by-email flow:

**Default local SMTP seed**
- A new `DefaultSmtpSettingsSeeder` in `Notifications.Infrastructure` runs once on startup
  (after migrations and template seeding) and creates exactly one default SMTP record
  pointing at a local Mailpit/MailHog-style capture server when no SMTP settings exist:
  `Local Dev SMTP` / `localhost:1025` / `dev` / `noreply@mavrynt.local` / SSL off / enabled.
- The seeder is idempotent: it skips when any SMTP setting already exists, so re-running
  the host never produces duplicates.
- Password is stored via `ISecretProtector.Protect` exactly like user-created records.
- `ISmtpSettingsRepository` gained a single new method, `AnyAsync(ct)`, used by the seeder
  for an efficient existence check.

**Per-configuration test email**
- A new `ISmtpTestEmailService` (Application abstraction) and `SmtpTestEmailService`
  (Infrastructure implementation) send a fixed `Mavrynt SMTP test email` message
  through a *specifically selected* SMTP configuration, regardless of whether it is
  active. This is intentionally separate from `IEmailSender.SendAsync(...)`, which
  continues to use only the active provider.
- A new `SendSmtpTestEmailCommand` + handler validates the recipient, calls the test
  service, and writes a `SmtpTestEmailSent` audit entry on success. SMTP password
  is never exposed in DTOs, audit metadata, logs, or HTTP responses.
- A new `Notifications.Email.RecipientInvalid` error code is introduced for the
  validation failure path.
- AdminApp exposes `POST /api/admin/notifications/smtp-settings/{id:guid}/test`
  (AdminOnly), returning `204` on success, `404` when the configuration is not
  found, and `400` for validation or send failures.
- The Admin SMTP Settings page gains a per-row `Send test` button with an inline
  recipient form, surfacing success and failure messages without exposing the
  stored password.

### Rationale

- Local-first development: a freshly cloned and started AdminApp must be able to
  inspect, render, and (with Mailpit/MailHog running locally) actually send emails
  without manual SMTP setup.
- Verifying SMTP settings before activation: administrators need a safe way to
  validate a newly entered configuration without touching the active provider used
  by the rest of the application.
- Foundation for password reset by email: the upcoming reset flow requires a known-
  working SMTP configuration; default seed + manual verification removes that
  dependency from the reset feature itself.

### Consequences

- `ISmtpSettingsRepository` is one method larger; in-memory test doubles and EF
  Core implementation are updated together.
- A second SMTP-using path now exists (`SmtpTestEmailService` next to `SmtpEmailSender`).
  Both go through the same `System.Net.Mail.SmtpClient` style chosen in ADR-023;
  if/when that is replaced (MailKit, transactional email API), both must move
  together.
- The default seed assumes Mailpit/MailHog on `localhost:1025` for local dev.
  Operators that already have an external SMTP provider configured experience no
  change because the seeder is a no-op when any SMTP record exists.
- This ADR explicitly does **not** add password reset tokens, the
  forgot/reset-password endpoints, or public reset pages — those remain
  the next step.

---

## ADR-025 — Pipeline-owned commit: `ITransactionalRequest` + multi-`IUnitOfWork` behavior

**Status:** Accepted  
**Date:** 2026-05-04

### Decision

The mediator pipeline now owns persistence commits for every mutating command,
through three coordinated changes:

1. **`TransactionBehavior` commits all registered units of work.**
   The behavior resolves `IEnumerable<IUnitOfWork>` from DI (instead of a single
   `IUnitOfWork`) and calls `SaveChangesAsync` on each on handler success. This
   removes the previous DI collision where the last module's
   `services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<{Module}DbContext>())`
   silently overrode prior registrations in a multi-module host such as
   `Mavrynt.AdminApp`. EF Core only emits SQL when there are pending changes, so
   contexts unrelated to the current command are essentially free.

2. **All mutating commands implement `ITransactionalRequest`.**
   `Users`: `RegisterUserCommand`, `AssignUserRoleCommand`, `ChangeOwnPasswordCommand`,
   `ChangeUserPasswordCommand`, `ChangeUserEmailCommand`.
   `FeatureManagement`: `CreateFeatureFlagCommand`, `UpdateFeatureFlagCommand`,
   `ToggleFeatureFlagCommand`.
   `Notifications`: `CreateSmtpSettingsCommand`, `UpdateSmtpSettingsCommand`,
   `EnableSmtpSettingsCommand`, `UpdateEmailTemplateCommand`.
   `LoginUserCommand` and the SMTP test-email commands stay non-transactional —
   they do not mutate persisted entities directly. Audit-only writes continue
   to be flushed by the audit writer (see point 3).

3. **Repository `AddAsync` methods no longer self-save.**
   `UserRepository.AddAsync`, `FeatureFlagRepository.AddAsync`, and
   `SmtpSettingsRepository.AddAsync` previously called `SaveChangesAsync`
   internally. They now only stage the entity in the change tracker; commit
   happens via `TransactionBehavior` at end of pipeline. This removes the bug
   where Update/Enable/Toggle handlers (which never go through `AddAsync`)
   silently failed to persist their mutations.

`AuditDbContext` is intentionally **not** registered as `IUnitOfWork`, and
`EfAuditService` / `EfAuditLogWriter` continue to call `SaveChangesAsync`
themselves. Audit rows are append-only and self-flushing keeps them outside
the per-command transaction. Hardening audit so that an orphan audit entry
cannot precede a failed command commit is deferred (it would require
suppressing the audit save, registering `AuditDbContext` as a unit of work,
and accepting that one DbContext write may succeed while another fails — the
cross-context atomicity problem already noted in `TransactionBehavior`).

### Rationale

Before this ADR, three persistence mechanisms could in principle commit a
mutation, but none of them fired for a wide class of handlers:

- `TransactionBehavior` only activates for commands marked
  `ITransactionalRequest`, and **no command in any module had that marker**.
- `RepositoryX.AddAsync` self-saved, which made Create commands work. Update,
  Enable, and Toggle handlers — which mutate an already-tracked aggregate —
  had no save path.
- The audit writer saves its own context. In Users, that context is shared
  with `User` entities, so Users mutations got flushed *by accident* alongside
  the audit row. In FeatureManagement and Notifications, the audit writer
  uses a separate `AuditDbContext`, so the entity mutation was never persisted.

Concretely, before this ADR every PATCH on `/api/admin/notifications/smtp-settings/{id}`,
`/api/admin/notifications/smtp-settings/{id}/enable`,
`/api/admin/notifications/email/templates/{key}`,
`/api/admin/feature-flags/{key}`, and `/api/admin/feature-flags/{key}/toggle`
returned a successful response with the updated DTO computed from the in-memory
entity — but the database row was unchanged. `ChangeUserPasswordCommand` and
`ChangeUserEmailCommand` were broken too (no audit write means no accidental
save). Existing integration tests asserted only against the HTTP response body,
not against a fresh re-read, so the bug was not detected.

A pipeline-owned commit is the correct architectural fix:
- Handlers stay focused on domain mutation; they no longer need to know whether
  to call `_context.SaveChangesAsync()`, `_unitOfWork.SaveChangesAsync()`, or
  rely on a side-effect from another component.
- Adding a new mutating command becomes a one-line decision (add
  `ITransactionalRequest`), preventing the "I forgot to save" footgun.
- The collision between per-module `IUnitOfWork` registrations is resolved
  structurally rather than by ordering DI calls carefully.

### Consequences

- New mutating commands MUST implement `ITransactionalRequest` to be persisted.
  This is enforced by code review, not by the type system; an integration test
  per endpoint that re-fetches state via a fresh GET catches the omission.
- Repository unit tests and seeders that previously relied on
  `repository.AddAsync` to save now must call `SaveChangesAsync` (or
  `IUnitOfWork.SaveChangesAsync`) themselves. Updated:
  `UsersInfrastructureIntegrationTests`, `FeatureManagementInfrastructureTests`,
  `DefaultSmtpSettingsSeeder`, `DefaultEmailTemplateSeeder` (already did).
- Cross-context atomicity is still not guaranteed. If a handler mutates two
  module DbContexts and one save fails, the other has already committed. This
  is unchanged from the pre-ADR state and is documented in
  `TransactionBehavior`. Introducing an explicit `TransactionScope` or EF
  Core `IDbContextTransaction` across contexts requires its own ADR.
- Validation of the audit-write ordering is unchanged: the audit row may still
  be written before the entity mutation is flushed (the audit writer
  self-saves mid-handler). The pipeline commit guarantees the mutation lands
  *after* the audit row in the same successful execution; on handler failure
  we now correctly skip the entity commit but still leave a stale audit row.
  Tightening this is deferred.
- Persistence regression integration tests added to
  `AdminNotificationsIntegrationTests` and `AdminFeatureFlagIntegrationTests`
  re-fetch via GET after every PATCH to assert disk state matches the response.

---

## Rules for adding future decisions

Each new decision should include:
- an identifier (next free `ADR-NNN`),
- a status (`Proposed`, `Accepted`, `Rejected`, `Superseded`),
- a date,
- the decision itself,
- rationale,
- consequences.

Add the new ADR at the bottom of this file and add a row to the index table at
the top. Do not edit accepted ADRs in place — if a later ADR replaces an earlier
one, mark the older entry `Superseded` in the index and reference the new ADR.

---

## Open areas for future decisions

The following areas remain open at this stage:
- production-grade `ISecretProtector` selection and key rotation,
- CI/CD pipeline design and runner choice,
- staging and production deployment topology,
- user-facing feature-flag read endpoints (flags are AdminApp-only today),
- Polly-backed resilience policies for `IResilientRequest`,
- asynchronous communication model (RabbitMQ / Kafka activation),
- API versioning standards,
- full RBAC beyond `Admin` / `User` roles.

---

## Summary

This document is the canonical architectural decision register for Mavrynt. It
preserves the chain of decisions made while the solution is built and extended.
Every significant decision that changes architectural direction must be added
here.


## ADR-026 — Redis-backed query cache and object-level invalidation

**Status:** Accepted  
**Date:** 2026-05-04

### Decision
Enable Redis as an active cross-cutting cache through `MavryntMediator` behaviors. Queries declare explicit cache policy in Application (`ICachedQuery<TResponse>`). Mutating commands declare invalidation metadata (`IInvalidatesCache`), and invalidation is executed by pipeline behavior after successful persistence.

### Rationale
Keeps caching and invalidation out of handlers/endpoints, preserves module boundaries, and standardizes deterministic keys, TTLs, and tag invalidation. Redis has no native tags, so tag-to-key sets are implemented in Infrastructure.

### Consequences
- Redis is active (no longer only reserved from ADR-012).
- Application owns cache policy declarations.
- Infrastructure owns Redis implementation details.
- Handlers do not call Redis directly.
- Failed queries are not cached; failed commands do not invalidate cache.
- Invalidation executes post-commit through pipeline ordering.
