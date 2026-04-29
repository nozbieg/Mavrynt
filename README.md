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

Najważniejsze dokumenty znajdują się w katalogu `docs` (każdy w wersji PL i EN):
- `docs/architecture.pl.md` / `docs/architecture.en.md`,
- `docs/decisions.pl.md` / `docs/decisions.en.md`,
- `docs/repo-structure.pl.md` / `docs/repo-structure.en.md`.

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

Fundament backendowy (hosty, building blocks, moduł Users, podstawowa autoryzacja JWT, mediator, pierwsze testy backendowe) oraz wszystkie trzy aplikacje frontendowe (`mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`) są już na miejscu. Aplikacja landing przeszła pełny cykl fundament → treść → dostępność (WCAG 2.1 AA) → wydajność → testy end-to-end (Vitest + Playwright).

### Najbliższy krok Fazy 1

Najbliższym priorytetem jest domknięcie pierwszego administracyjnego vertical slice'a Fazy 1:
- role i uprawnienia użytkowników,
- moduł FeatureManagement zarządzany z poziomu AdminApp,
- trwały audyt operacji systemowych,
- endpointy administracyjne zabezpieczone polityką `AdminOnly`,
- testy architektoniczne, jednostkowe i integracyjne dla nowego zakresu.

Dopiero po tym kroku repozytorium powinno przejść do szerszej konfiguracji CI/CD, stagingu i kolejnych modułów domenowych.

### Zasady organizacyjne

W projekcie obowiązują następujące zasady:
- nie umieszczamy logiki biznesowej w hostach,
- nie mieszamy warstw domeny, aplikacji i infrastruktury,
- każdy moduł powinien zachowywać spójny układ,
- dokumentacja architektury jest utrzymywana razem z kodem,
- większe decyzje techniczne powinny być dopisywane do `docs/decisions.pl.md` (oraz `docs/decisions.en.md`),
- landing marketingowy jest niezależny od backendu — integracje idą wyłącznie przez adaptery (ADR-015).

### Uruchomienie

**Wymagania środowiskowe:**
- .NET 10 SDK,
- Node.js 22+ oraz pnpm 9+,
- Docker (opcjonalnie, pod przyszłe zależności infrastrukturalne).

**Cały stos przez Aspire AppHost (backend + wszystkie SPA):**

    cd src/backend/Mavrynt.AppHost
    dotnet run

`Mavrynt.AppHost` orkiestruje API, Admin API, `mavrynt-web`, `mavrynt-admin` oraz `mavrynt-landing`. Landing jest świadomie niezwiązany z API — działa samodzielnie również wtedy, gdy backend jest wyłączony.

**Tylko landing (szybka iteracja marketingu):**

    pnpm --filter mavrynt-landing dev        # serwer deweloperski (HMR)
    pnpm --filter mavrynt-landing test       # testy jednostkowe Vitest
    pnpm --filter mavrynt-landing test:e2e   # smoke Playwright (Chromium)
    pnpm --filter mavrynt-landing build      # build produkcyjny (gzip + brotli)

Szczegółowa instrukcja landingu: [`src/frontend/mavrynt-landing/README.md`](src/frontend/mavrynt-landing/README.md).

### Backend tests (architecture, unit, integration)

Backendowy fundament testów jest podzielony na trzy warstwy:
- **testy architektoniczne** chronią granice projektów i zależności w modularnym monolicie,
- **testy jednostkowe** weryfikują prymitywy domenowe i handlery komend/zapytań,
- **testy integracyjne** używają realnych kontenerów PostgreSQL przez Testcontainers dla infrastruktury i smoke testów hostów.

Uruchomienie sprawdzeń backendowych z katalogu głównego repozytorium:

```bash
dotnet restore Mavrynt.sln
dotnet build Mavrynt.sln --no-restore
dotnet test Mavrynt.sln --no-build
```

> Docker jest wymagany dla testów integracyjnych opartych o Testcontainers (`Mavrynt.Modules.Users.Infrastructure.Tests`, `Mavrynt.Api.IntegrationTests`, `Mavrynt.AdminApp.IntegrationTests`).

### Status

Fundament architektoniczny jest gotowy, landing jest funkcjonalny i gotowy do wdrożenia, a backend ma już bazowy Users/Auth oraz początek piramidy testów. Następny krok: administracyjny vertical slice Fazy 1 — role/uprawnienia, FeatureManagement i trwały audyt w AdminApp.

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

The most important documents are located in the `docs` folder (each is shipped in PL and EN):
- `docs/architecture.pl.md` / `docs/architecture.en.md`,
- `docs/decisions.pl.md` / `docs/decisions.en.md`,
- `docs/repo-structure.pl.md` / `docs/repo-structure.en.md`.

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

The backend foundation (hosts, building blocks, the Users module, basic JWT authentication, mediator, and the first backend tests) and all three frontend applications (`mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`) are now in place. The landing app has completed a full lifecycle: foundation → content → accessibility (WCAG 2.1 AA) → performance → end-to-end testing (Vitest + Playwright).

### Next Phase 1 step

The nearest priority is to close the first administrative Phase 1 vertical slice:
- user roles and permissions,
- a FeatureManagement module managed from AdminApp,
- persistent audit of system operations,
- administrative endpoints protected by the `AdminOnly` policy,
- architectural, unit, and integration tests for the new scope.

Only after this step should the repository move toward broader CI/CD configuration, staging, and additional domain modules.

### Organizational rules

The following rules apply in the project:
- business logic must not be placed in hosts,
- domain, application, and infrastructure layers must not be mixed,
- every module should follow a consistent structure,
- architecture documentation is maintained together with the code,
- major technical decisions should be added to `docs/decisions.en.md` (and `docs/decisions.pl.md`),
- the marketing landing page is independent of the backend — integrations go through adapters only (ADR-015).

### Running the solution

**Requirements:**
- .NET 10 SDK,
- Node.js 22+ and pnpm 9+,
- Docker (optional — for future infrastructure dependencies).

**Full stack via Aspire AppHost (backend + all SPAs):**

    cd src/backend/Mavrynt.AppHost
    dotnet run

`Mavrynt.AppHost` orchestrates the API, the Admin API, `mavrynt-web`, `mavrynt-admin`, and `mavrynt-landing`. The landing is deliberately not coupled to the API — it still runs when the backend is stopped.

**Landing only (fast marketing iteration):**

    pnpm --filter mavrynt-landing dev        # dev server (HMR)
    pnpm --filter mavrynt-landing test       # Vitest unit + integration tests
    pnpm --filter mavrynt-landing test:e2e   # Playwright smoke (Chromium)
    pnpm --filter mavrynt-landing build      # production build (gzip + brotli)

Detailed landing docs: [`src/frontend/mavrynt-landing/README.md`](src/frontend/mavrynt-landing/README.md).

### Backend tests (architecture, unit, integration)

Backend test foundation is split into three layers:
- **Architecture tests** protect project and dependency boundaries in the modular monolith.
- **Unit tests** verify domain primitives and command/query handlers.
- **Integration tests** use **real PostgreSQL containers** via Testcontainers for infrastructure and host-level smoke tests.

Run backend checks from repository root:

```bash
dotnet restore Mavrynt.sln
dotnet build Mavrynt.sln --no-restore
dotnet test Mavrynt.sln --no-build
```

> Docker is required for Testcontainers-based integration tests (`Mavrynt.Modules.Users.Infrastructure.Tests`, `Mavrynt.Api.IntegrationTests`, `Mavrynt.AdminApp.IntegrationTests`).

### Status

Architectural foundation is ready, the landing app is functional and deploy-ready, and the backend already has base Users/Auth plus the beginning of the backend test pyramid. Next step: the administrative Phase 1 vertical slice — roles/permissions, FeatureManagement, and persistent audit in AdminApp.
