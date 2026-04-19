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

Fundament backendowy (hosty, building blocks, moduł Users) oraz wszystkie trzy aplikacje frontendowe (`mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`) są już na miejscu. Aplikacja landing przeszła pełny cykl fundament → treść → dostępność (WCAG 2.1 AA) → wydajność → testy end-to-end (Vitest + Playwright).

### Cele najbliższych etapów

Najbliższe etapy rozwoju obejmują:
- implementację właściwej logiki modułu Users (autoryzacja, role, uprawnienia),
- dopięcie kolejnych modułów domenowych,
- rozbudowę piramidy testów na backendzie (unit, integracyjne, architektoniczne),
- produkcyjne adaptery portów frontendowych (HTTP `LeadService`, real-world `AnalyticsClient`),
- konfigurację CI/CD oraz kontenerizacji środowiska,
- pierwsze środowisko wdrożeniowe (staging).

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

### Status

Fundament architektoniczny gotowy. Pierwszy z frontendów (landing) jest w pełni funkcjonalny i gotowy do wdrożenia. Następny krok: rozwój modułów domenowych i środowiska CI/CD.

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

The backend foundation (hosts, building blocks, the Users module) and all three frontend applications (`mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`) are now in place. The landing app has completed a full lifecycle: foundation → content → accessibility (WCAG 2.1 AA) → performance → end-to-end testing (Vitest + Playwright).

### Near-term goals

The next stages of development include:
- implementing the real Users module logic (authn/authz, roles, permissions),
- wiring additional domain modules,
- expanding the backend test pyramid (unit, integration, architectural),
- production adapters for the frontend ports (HTTP `LeadService`, real-world `AnalyticsClient`),
- CI/CD configuration and environment containerisation,
- the first deployment environment (staging).

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

### Status

Architectural foundation complete. The first frontend (landing) is fully functional and deploy-ready. Next steps: domain-module build-out and CI/CD.