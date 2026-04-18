# AGENTS.md - Wytyczne dla agentów AI

Dokument zawiera istotną wiedzę dla efektywnej pracy z kodem bazą Mavrynt.

## Architektura — "Big Picture"

**Model:** Modularny monolit w jednym repozytorium. Backend to jeden skalowany produkt, frontendy są klientami API.

**Struktura backendowa:**
- **Hosty:** `Mavrynt.Api` (główny), `Mavrynt.AdminApp` (administracyjny), `Mavrynt.AppHost` (orkiestracja lokalnie)
- **Building Blocks:** Wspólne projekty (Domain, Application, Infrastructure, Contracts) dla całego backendu
- **Moduły:** `Mavrynt.Modules.{DomainName}.*` — każdy moduł ma własne Domain/Application/Infrastructure

**Kluczowe granice zależności:**
- Domain nie zna Infrastructure ani hostów
- Application nie zależy od hosta
- Infrastructure implementuje kontrakt Application
- Host scala moduły, nie zawiera logiki biznesowej
- Między modułami tylko przez wyraźne interfejsy, bez bezpośrednich referencji domenowych

Czytaj: `docs/architecture.pl.md`, `docs/decisions.pl.md`

## Konwencje warstw w module

Każdy nowy moduł domenowy musi dążyć do tego struktury:

```
Mavrynt.Modules.{Domain}/
  ├── Entities/          (agregaty, entity roots)
  ├── ValueObjects/      (value objects)
  ├── Enums/            (typy domenowe)
  ├── Events/           (zdarzenia domenowe)
  └── Repositories/     (abstrakcje interfejsów)

Mavrynt.Modules.{Domain}.Application/
  ├── Commands/         (zmiana stanu — write)
  ├── Queries/          (czytanie stanu — read)
  ├── DTOs/             (transfer objects)
  ├── Abstractions/     (interfejsy, porty)
  └── DependencyInjection/ (rejestracja w kontenerze)

Mavrynt.Modules.{Domain}.Infrastructure/
  ├── Persistence/      (konfiguracja EF Core, DbContext, mapowania)
  ├── Repositories/     (implementacje interfejsów z Domain)
  └── Configuration/    (setup modułu do DI)
```

Przykład: `Mavrynt.Modules.Users.*` — śledź ten moduł jako szablon.

## Stack techniczny i polecenia

**.NET 10.0**, C# nullable enabled, implicit usings enabled.

**Zależności wspólne** (w `Directory.Packages.props`):
- OpenTelemetry (obserwability)
- Microsoft.Extensions.Http.Resilience (odporność)
- ServiceDiscovery (sieciowość)

**Lokalne uruchomienie:**
```powershell
cd C:\Repo\Mavrynt
dotnet run --project src/backend/Mavrynt.AppHost/Mavrynt.AppHost.csproj
```

Zapewnia orkiestrację: `Mavrynt.Api`, `Mavrynt.AdminApp`, `mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`.

**Budowanie:**
```powershell
dotnet build Mavrynt.sln
```

Centralne zarządzanie pakietami (edytuj `Directory.Packages.props`).

## Praktyczne warianty dla agentów

**Dodaj nowy endpoint do Mavrynt.Api:**
1. Endpoint w `Program.cs` lub kontrolerze
2. Logika w `Mavrynt.Modules.Users.Application` (Commands/Queries)
3. Zarejestruj w `DependencyInjection` modułu
4. Hosta nie edytuj dla logiki — host to tylko "router"

**Dodaj nową kolumnę/pole w Domain Users:**
1. Edytuj `Mavrynt.Modules.Users.Domain/Entities` lub `ValueObjects`
2. Jeśli potrzebne w DB: zaktualizuj mapowanie w `Infrastructure/Persistence`
3. Zwróć pole w DTO z `Application/DTOs` (nie zwracaj całej encji)

**Unikaj:**
- Dodawania Business Logic do `Program.cs` czy hosta
- Referencjonowania Infrastructure w Domain
- Bezpośrednich zależności między modułami — zawsze przez kontrakt

## Frontend — osobny ekosystem

Frontendy (`src/frontend/mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`) to oddzielne projekty Node.js/Vite.
- Nie referencjonują projektów backendowych
- Integrują się tylko przez HTTP API
- Każdy ma własny `package.json`

## Dokumentacja i decyzje

- Architektura: `docs/architecture.pl.md` (przeczytaj przed zmianami strukturalnymi)
- Decyzje: `docs/decisions.pl.md` (dlaczego coś wygląda tak, a nie inaczej)

Nowe decyzje architektoniczne: dodaj do `docs/decisions.pl.md` w formacie ADR (Architecture Decision Record).

## Feature flags i Observability

- Flagi mają być obsługiwane od razu (zarządzanie w AdminApp)
- Observability: logowanie, metryki, ślady — wspólny setup w `Mavrynt.ServiceDefaults`
- Nie odkładaj observability na koniec

## Klucze do efektywności

1. **Czytaj dokumentację** — architektura ma konkretne powody
2. **Pilnuj granic** — Domain ← Application ← Infrastructure jest zasadą
3. **Moduł Users jako wzorzec** — powielaj jego strukturę dla nowych modułów
4. **Edytuj AppHost dla nowych serwisów** — tam spinacie nowe komponenty
5. **Test coverage** — każdy moduł powinien być testowany jednostkowo (folder `tests/`)
6. **Stosuj wzorce projektowe** — np. CQRS dla Commands/Queries, Repository dla dostępu do danych, DRY, SOLID

