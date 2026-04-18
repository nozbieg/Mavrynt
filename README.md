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