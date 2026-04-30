# Mavrynt — Struktura repozytorium

## 1. Cel dokumentu

Celem dokumentu jest opisanie aktualnej i docelowej struktury repozytorium Mavrynt. Dokument definiuje:
- przeznaczenie głównych katalogów,
- odpowiedzialność poszczególnych projektów,
- zasady organizacji plików i kodu,
- kierunek dalszej rozbudowy repozytorium.

Repozytorium ma być czytelne dla człowieka, przewidywalne dla agentów AI i wygodne w utrzymaniu w długim okresie.

---

## 2. Główne założenia organizacyjne

Repozytorium:
- obejmuje cały produkt,
- zawiera backend, frontendy, dokumentację, testy i zasoby wdrożeniowe,
- unika mieszania odpowiedzialności,
- utrzymuje spójne nazewnictwo,
- wspiera rozwój modularnego monolitu,
- wspiera lokalną orkiestrację przez Aspire AppHost,
- jest przygotowywane pod Continuous Delivery.

Zasada podstawowa: **każdy katalog ma czytelny cel i powinien zawierać tylko to, co jest zgodne z jego odpowiedzialnością.**

---

## 3. Struktura główna repozytorium

Aktualna struktura repozytorium:

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
│   │   ├── Mavrynt.Modules.Users.Infrastructure/
│   │   ├── Mavrynt.Modules.FeatureManagement.Domain/
│   │   ├── Mavrynt.Modules.FeatureManagement.Application/
│   │   ├── Mavrynt.Modules.FeatureManagement.Infrastructure/
│   │   ├── Mavrynt.Modules.Audit.Domain/
│   │   ├── Mavrynt.Modules.Audit.Application/
│   │   └── Mavrynt.Modules.Audit.Infrastructure/
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
    ├── Mavrynt.Modules.FeatureManagement.Domain.Tests/
    ├── Mavrynt.Modules.FeatureManagement.Application.Tests/
    ├── Mavrynt.Modules.FeatureManagement.Infrastructure.Tests/
    ├── Mavrynt.Api.IntegrationTests/
    └── Mavrynt.AdminApp.IntegrationTests/
```

Uwaga: katalogi `build/`, `deploy/` i `scripts/` pozostają przewidzianym miejscem dla automatyzacji build/deploy oraz skryptów operacyjnych, nawet jeśli w danym momencie są puste albo jeszcze nie zostały rozbudowane.

---

## 4. Backend

Backend znajduje się w `src/backend` i jest organizowany jako modularny monolit.

### 4.1. Hosty

- `Mavrynt.Api` — główny host API dla części użytkowej produktu.
- `Mavrynt.AdminApp` — host API dla części administracyjnej.
- `Mavrynt.AppHost` — lokalna orkiestracja przez Aspire; uruchamia backend i aplikacje frontendowe.
- `Mavrynt.ServiceDefaults` — wspólne ustawienia techniczne: observability, health checks, service discovery i standardy hostingu.

Hosty nie powinny zawierać logiki biznesowej. Ich zadaniem jest kompozycja modułów, konfiguracja pipeline HTTP i wystawianie endpointów.

### 4.2. Building Blocks

- `Mavrynt.BuildingBlocks.Domain` — bazowe typy domenowe, encje, agregaty, value objects, błędy i `Result`.
- `Mavrynt.BuildingBlocks.Application` — abstrakcje command/query, mediator, walidacja, pipeline behaviors, konteksty aplikacyjne.
- `Mavrynt.BuildingBlocks.Infrastructure` — wspólne abstrakcje infrastruktury, np. `IRepository`, `IUnitOfWork`, opcje PostgreSQL.
- `Mavrynt.BuildingBlocksContracts` — kontrakty integracyjne i współdzielone DTO kontraktowe.

Building Blocks nie mogą stać się zbiorem przypadkowych helperów. Trafiają tam wyłącznie elementy naprawdę współdzielone między modułami.

### 4.3. Moduły domenowe

Zaimplementowane moduły Fazy 1:

**Users** — rejestracja użytkowników, uwierzytelnianie, przypisywanie ról.
- `Mavrynt.Modules.Users.Domain`
- `Mavrynt.Modules.Users.Application`
- `Mavrynt.Modules.Users.Infrastructure`

**FeatureManagement** — CRUD flag funkcjonalności, zarządzany przez AdminApp.
- `Mavrynt.Modules.FeatureManagement.Domain`
- `Mavrynt.Modules.FeatureManagement.Application`
- `Mavrynt.Modules.FeatureManagement.Infrastructure`

**Audit** — append-only dziennik audytu administracyjnego i systemowego.
- `Mavrynt.Modules.Audit.Domain`
- `Mavrynt.Modules.Audit.Application`
- `Mavrynt.Modules.Audit.Infrastructure`

Każdy moduł zachowuje ten sam podział na Domain / Application / Infrastructure. Przewidziane przyszłe moduły:
- `Notifications` / `Email`,
- ewentualnie wydzielone `Authorization`, jeśli role i uprawnienia urosną poza prosty model Users.

---

## 5. Frontend

Frontend znajduje się w `src/frontend`.

Główne aplikacje:
- `mavrynt-web` — aplikacja użytkownika,
- `mavrynt-admin` — aplikacja administracyjna,
- `mavrynt-landing` — statyczny landing marketingowy.

Projekty `.NET` typu host SPA:
- `Mavrynt.Web.App`,
- `Mavrynt.Web.Admin`,
- `Mavrynt.Web.Landing`.

Pakiety współdzielone znajdują się w `src/frontend/shared/*` i mogą obejmować między innymi:
- konfigurację TypeScript,
- konfigurację ESLint,
- design tokens,
- prymitywy UI,
- konfigurację adresów aplikacji,
- współdzielone komponenty auth UI.

Frontend nie może referencjonować projektów backendowych bezpośrednio. Integracja odbywa się przez API, adaptery i jawne kontrakty.

---

## 6. Testy

Testy znajdują się w `tests`.

Aktualny podział:
- `tests/backend/Mavrynt.Architecture.Tests` — testy architektoniczne pilnujące granic zależności wszystkich modułów.
- `tests/backend/Mavrynt.BuildingBlocks.Domain.Tests` — testy prymitywów domenowych.
- `tests/Mavrynt.Modules.Users.Domain.Tests` — testy domeny Users.
- `tests/Mavrynt.Modules.Users.Application.Tests` — testy command/query handlerów Users.
- `tests/Mavrynt.Modules.Users.Infrastructure.Tests` — testy infrastruktury Users z PostgreSQL przez Testcontainers.
- `tests/Mavrynt.Modules.FeatureManagement.Domain.Tests` — testy domeny FeatureManagement.
- `tests/Mavrynt.Modules.FeatureManagement.Application.Tests` — testy handlerów command/query FeatureManagement.
- `tests/Mavrynt.Modules.FeatureManagement.Infrastructure.Tests` — testy infrastruktury FeatureManagement z PostgreSQL przez Testcontainers.
- `tests/Mavrynt.Api.IntegrationTests` — testy integracyjne głównego API.
- `tests/Mavrynt.AdminApp.IntegrationTests` — testy integracyjne AdminApp (role użytkowników + endpointy feature flag).

Docelowo testy powinny tworzyć trzy warstwy walidacji backendu:
1. testy architektoniczne,
2. testy jednostkowe modułów,
3. testy integracyjne z realną bazą danych przez Testcontainers.

---

## 7. Dokumentacja

Dokumentacja znajduje się w `docs`.

Najważniejsze pliki:
- `architecture.pl.md` / `architecture.en.md` — opis architektury rozwiązania,
- `decisions.pl.md` / `decisions.en.md` — dziennik decyzji architektonicznych,
- `repo-structure.pl.md` / `repo-structure.en.md` — struktura repozytorium.

`AGENTS.md` zawiera skrócone instrukcje operacyjne dla agentów AI i powinien być spójny z dokumentami w `docs`.

---

## 8. Zasady dodawania nowych elementów

Nowy backendowy moduł domenowy powinien trafić do `src/backend` jako zestaw projektów:

```text
Mavrynt.Modules.{Name}.Domain/
Mavrynt.Modules.{Name}.Application/
Mavrynt.Modules.{Name}.Infrastructure/
```

Nowe testy modułu powinny trafić do `tests` jako osobne projekty testowe:

```text
Mavrynt.Modules.{Name}.Domain.Tests/
Mavrynt.Modules.{Name}.Application.Tests/
Mavrynt.Modules.{Name}.Infrastructure.Tests/
```

Nowa aplikacja frontendowa powinna trafić do `src/frontend` i korzystać ze współdzielonych pakietów wyłącznie wtedy, gdy są one rzeczywiście wspólne.

Nowa decyzja architektoniczna powinna zostać dopisana do `docs/decisions.pl.md` oraz `docs/decisions.en.md`.

---

## 9. Aktualny kierunek Fazy 1

Faza 1 koncentruje się na fundamencie platformy:
- użytkownicy,
- logowanie,
- role i uprawnienia,
- feature flagi zarządzane z AdminApp,
- audyt działań systemowych,
- podstawowa komunikacja mailowa,
- observability,
- testowalność,
- przygotowanie pod CI/CD.

Administracyjny vertical slice (role/uprawnienia + FeatureManagement + trwały audyt, spięty z AdminApp, chroniony przez `AdminOnly`, pokryty testami architektonicznymi/jednostkowymi/integracyjnymi) jest **ukończony** od 2026-04-29.

Pozostałe elementy Fazy 1: email/powiadomienia, konfiguracja pipeline CI/CD, konfiguracja środowiska stagingowego.

---

## 10. Podsumowanie

Repozytorium Mavrynt ma fundament modularnego monolitu, hosty backendowe, moduły Users / FeatureManagement / Audit, aplikacje frontendowe oraz wielowarstwową piramidę testów (architektoniczne, jednostkowe i integracyjne Testcontainers). Struktura repozytorium powinna pozostać stabilna, a kolejne prace powinny rozwijać pozostałe elementy Fazy 1 (email, CI/CD) bez łamania granic warstw i bez przenoszenia logiki biznesowej do hostów.
