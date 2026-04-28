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

## Mediator — zasady używania (ADR-020)

Mavrynt używa **wewnętrznego mediatora** (`MavryntMediator`). Nie dodawaj MediatR ani żadnej zewnętrznej biblioteki mediatora.

### Nowe komendy i zapytania

```csharp
// Komenda bez odpowiedzi
public sealed record MyCommand(...) : ICommand;

// Komenda z odpowiedzią
public sealed record MyCommand(...) : ICommand<MyDto>;

// Zapytanie
public sealed record MyQuery(...) : IQuery<MyDto>;
```

### Handlery

```csharp
// Handler komendy bez odpowiedzi
public sealed class MyCommandHandler : ICommandHandler<MyCommand>
{
    public Task<Result> HandleAsync(MyCommand command, CancellationToken ct) { ... }
}

// Handler komendy z odpowiedzią
public sealed class MyCommandHandler : ICommandHandler<MyCommand, MyDto>
{
    public Task<Result<MyDto>> HandleAsync(MyCommand command, CancellationToken ct) { ... }
}

// Handler zapytania
public sealed class MyQueryHandler : IQueryHandler<MyQuery, MyDto>
{
    public Task<Result<MyDto>> HandleAsync(MyQuery query, CancellationToken ct) { ... }
}
```

### Wywoływanie z endpointów

```csharp
// DOBRZE — wstrzyknij IMediator
private static async Task<IResult> HandleAsync(MyRequest req, IMediator mediator, CancellationToken ct)
{
    var result = await mediator.SendAsync(new MyCommand(req.Field), ct);
    return result.IsFailure ? MapToHttpError(result.Error) : Results.Ok(result.Value);
}

// ŹLE — nie wstrzykuj konkretnych handlerów w endpointach
private static async Task<IResult> HandleAsync(ICommandHandler<MyCommand, MyDto> handler, ...) { }
```

### Walidacja

Walidacja wejściowa (format, null-checks, długości) należy do `IValidator<TRequest>`, **nie** do handlerów:

```csharp
public sealed class MyCommandValidator : IValidator<MyCommand>
{
    public Task<Result> ValidateAsync(MyCommand request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Field))
            return Task.FromResult(Result.Failure(new Error("Validation.FieldRequired", "Field is required.")));
        return Task.FromResult(Result.Success());
    }
}
```

Walidatory są odkrywane automatycznie przez `AddMavryntMediator`. Logika biznesowa (invarianty domenowe) pozostaje w Domain.

### Markery opcjonalnych zachowań pipeline'u

| Interfejs | Efekt |
|---|---|
| `IAuditableRequest` | Zapisuje `AuditEntry` przez `IAuditService` po wykonaniu |
| `ITransactionalRequest` | Wywołuje `IUnitOfWork.SaveChangesAsync()` po sukcesie |
| `IResilientRequest` | Hook dla retry/timeout (nie stosuj na nieindepotentnych komendach) |

```csharp
// Przykład: komenda audytowalna i transakcyjna
public sealed record RegisterUserCommand(...) : ICommand<UserDto>, IAuditableRequest, ITransactionalRequest
{
    public string AuditEventType => "Users.UserRegistered";
}
```

### Rejestracja nowego modułu

W DI extensions modułu użyj:

```csharp
services.AddMavryntMediator(typeof(IMyModuleMarker).Assembly);
```

Rejestruje to mediator, wszystkie handlery, walidatory i zachowania pipeline'u dla podanego assembly.

### Czego NIE robić

- **Nie dodawaj MediatR** — Mavrynt ma własny mediator
- **Nie obchodź mediatora** z hostów — endpointy wywołują `IMediator.SendAsync`
- **Nie duplikuj logowania** w handlerach — `LoggingBehavior` już to robi
- **Nie duplikuj audytu** w handlerach — użyj `IAuditableRequest`
- **Nie otwieraj transakcji** w handlerach — użyj `ITransactionalRequest`
- **Nie serializes całego żądania** do logów (hasła, tokeny)

---

## Klucze do efektywności

1. **Czytaj dokumentację** — architektura ma konkretne powody
2. **Pilnuj granic** — Domain ← Application ← Infrastructure jest zasadą
3. **Moduł Users jako wzorzec** — powielaj jego strukturę dla nowych modułów
4. **Edytuj AppHost dla nowych serwisów** — tam spinacie nowe komponenty
5. **Test coverage** — każdy moduł powinien być testowany jednostkowo (folder `tests/`)
6. **Stosuj wzorce projektowe** — np. CQRS dla Commands/Queries, Repository dla dostępu do danych, DRY, SOLID

