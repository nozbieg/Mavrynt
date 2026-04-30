# Mavrynt — Dziennik decyzji architektonicznych

## Cel dokumentu

Dokument gromadzi najważniejsze decyzje architektoniczne i organizacyjne dotyczące rozwiązania Mavrynt. Każda decyzja powinna być zapisana w sposób zwięzły, ale jednoznaczny, tak aby w przyszłości było jasne:
- co zostało ustalone,
- dlaczego podjęto taką decyzję,
- jakie są konsekwencje tej decyzji.

Dokument ma charakter żywy i powinien być uzupełniany przy kolejnych istotnych zmianach.

---

## ADR-001 — Model rozwiązania: modular monolith

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
System Mavrynt jest budowany jako modularny monolit.

### Uzasadnienie
Na etapie budowy fundamentu produktu ważniejsze są:
- prostsza organizacja kodu,
- niższy koszt operacyjny,
- krótszy czas wejścia,
- łatwiejsze ręczne tworzenie i kontrola rozwiązania,
- zachowanie logicznego podziału na moduły bez konieczności uruchamiania wielu usług.

### Konsekwencje
- system pozostaje jednym rozwiązaniem i jednym produktem backendowym,
- moduły są wydzielane w obrębie jednego repozytorium,
- konieczne jest pilnowanie granic zależności,
- ewentualne wydzielanie mikroserwisów może nastąpić później, ale nie jest celem początkowym.

---

## ADR-002 — Jedno repozytorium dla całego produktu

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Cały produkt jest utrzymywany w jednym repozytorium.

### Uzasadnienie
Jedno repozytorium upraszcza:
- zarządzanie architekturą,
- spójność wersjonowania,
- pracę agentów AI,
- powiązanie backendu, frontendów, dokumentacji i wdrożeń,
- utrzymanie wspólnych standardów.

### Konsekwencje
- repozytorium musi mieć czytelną strukturę,
- trzeba pilnować odpowiedzialności katalogów i projektów,
- wzrasta znaczenie spójnych zasad organizacji kodu,
- łatwiej budować wspólne pipeline i wspólne standardy techniczne.

---

## ADR-003 — Rozdzielenie hostów backendowych

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Backend zostaje podzielony na odrębne hosty:
- `Mavrynt.Api`
- `Mavrynt.AdminApp`
- `Mavrynt.AppHost`
- `Mavrynt.ServiceDefaults` jako projekt wspierający standardy techniczne

### Uzasadnienie
Część użytkowa i administracyjna mają różne odpowiedzialności i różne scenariusze użycia. Dodatkowo development lokalny wymaga projektu spinającego zależności środowiskowe.

### Konsekwencje
- `Mavrynt.Api` staje się głównym hostem API produktu,
- `Mavrynt.AdminApp` obsługuje obszar administracyjny,
- `Mavrynt.AppHost` odpowiada za lokalną orkiestrację,
- `Mavrynt.ServiceDefaults` przechowuje standardy współdzielone między hostami.

---

## ADR-004 — Osobna część administracyjna

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Admin jest traktowany jako osobny obszar produktu, a nie jedynie zestaw dodatkowych ekranów.

### Uzasadnienie
Panel administracyjny ma odmienny charakter niż część użytkowa. Wymaga:
- odrębnych uprawnień,
- większej kontroli bezpieczeństwa,
- zarządzania konfiguracją systemu,
- zarządzania feature flagami,
- zarządzania użytkownikami i dostępami.

### Konsekwencje
- admin może rozwijać się niezależnie od warstwy użytkowej,
- część administracyjna zachowuje wyraźne granice odpowiedzialności,
- architektura od początku wspiera wydzielone procesy administracyjne.

---

## ADR-005 — Warstwowy układ modułów

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Każdy moduł backendowy powinien dążyć do układu:
- `Domain`
- `Application`
- `Infrastructure`

### Uzasadnienie
Takie podejście wspiera:
- czytelny podział odpowiedzialności,
- testowalność,
- kontrolę zależności,
- stopniowe dokładanie logiki,
- lepszą współpracę z kodem generowanym lub rozwijanym przez agentów AI.

### Konsekwencje
- logika domenowa nie trafia do hosta,
- szczegóły persystencji nie trafiają do domeny,
- przypadki użycia są wydzielane do warstwy aplikacyjnej,
- infrastruktura implementuje potrzeby warstw wyższych.

---

## ADR-006 — Wspólne projekty BuildingBlocks

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Powstaje zestaw wspólnych projektów:
- `Mavrynt.BuildingBlocks.Domain`
- `Mavrynt.BuildingBlocks.Application`
- `Mavrynt.BuildingBlocks.Infrastructure`
- `Mavrynt.BuildingBlocks.Contracts`

### Uzasadnienie
Wspólne projekty pozwalają przechowywać:
- wspólne abstrakcje,
- wzorce implementacyjne,
- podstawowe typy i kontrakty,
- elementy techniczne powtarzalne między modułami.

### Konsekwencje
- należy pilnować, aby BuildingBlocks nie stały się chaotycznym magazynem wszystkiego,
- do projektów wspólnych trafiają wyłącznie elementy rzeczywiście wielokrotnego użytku,
- odpowiedzialność modułów domenowych pozostaje oddzielona.

---

## ADR-007 — Pierwszy moduł bazowy: Users

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Pierwszym modułem bazowym backendu jest `Users`.

### Uzasadnienie
Użytkownicy są naturalnym fundamentem dalszych obszarów:
- uwierzytelniania,
- autoryzacji,
- ról i dostępów,
- resetów haseł,
- działań administracyjnych.

### Konsekwencje
- dalsze moduły będą mogły opierać się na ustalonych granicach dla obszaru użytkowników,
- moduł `Users` stanie się jedną z osi fundamentu produktu.

---

## ADR-008 — Feature flags jako element architektury podstawowej

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Mechanizm feature flag ma być obecny od początku architektury i zarządzany z poziomu części administracyjnej.

### Uzasadnienie
Feature flags wspierają:
- bezpieczne wdrażanie zmian,
- kontrolowane włączanie funkcji,
- separację wdrożenia od publikacji funkcjonalności,
- Continuous Delivery.

### Konsekwencje
- architektura musi przewidywać miejsce dla modułu lub komponentu flag,
- panel administracyjny musi mieć możliwość zarządzania flagami,
- projekt musi być przygotowany na konfigurację flag zależnie od środowiska i kontekstu.

---

## ADR-009 — Pełna observability od początku

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Rozwiązanie ma od początku wspierać pełną observability.

### Uzasadnienie
Projekt ma rosnąć i być wdrażany w sposób kontrolowany. Wymaga to od początku:
- logów,
- metryk,
- śladów,
- health checks,
- przewidywalnej diagnostyki.

### Konsekwencje
- część ustawień wspólnych powinna trafić do `Mavrynt.ServiceDefaults`,
- hosty i moduły muszą być gotowe na spójne standardy telemetryczne,
- observability nie może być odkładane na koniec implementacji.

---

## ADR-010 — Frontendy w tym samym repozytorium, ale oddzielone od backendu

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Frontendy pozostają w tym samym repozytorium, ale są fizycznie wydzielone w `src/frontend`.

### Uzasadnienie
Takie podejście pozwala:
- zachować spójność produktu,
- łatwo rozwijać aplikacje klienckie i landing,
- unikać mieszania odpowiedzialności backendowych i frontendowych,
- budować wspólne pipeline w jednym repo.

### Konsekwencje
- frontend nie powinien zależeć od projektów backendowych,
- integracja odbywa się przez API i kontrakty, nie przez bezpośrednie referencje projektowe,
- landing marketingowy pozostaje możliwie niezależny.

---

## ADR-011 — PostgreSQL jako główna baza danych

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Główną relacyjną bazą danych projektu jest PostgreSQL.

### Uzasadnienie
PostgreSQL dobrze wspiera:
- systemy aplikacyjne o architekturze modułowej,
- rozwój na platformie .NET,
- konteneryzację,
- nowoczesne scenariusze wdrożeniowe.

### Konsekwencje
- warstwa infrastruktury będzie projektowana z myślą o PostgreSQL,
- konfiguracja developmentowa i wdrożeniowa będzie uwzględniała tę bazę jako standard domyślny.

---

## ADR-012 — Redis, RabbitMQ i Kafka jako elementy przewidziane architektonicznie

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Architektura od początku przewiduje miejsce na:
- Redis,
- RabbitMQ,
- Kafka.

### Uzasadnienie
Produkt ma być gotowy na:
- cache,
- komunikację asynchroniczną,
- zdarzeniowość,
- integracje i skalowanie wewnętrznych przepływów.

### Konsekwencje
- nie wszystkie komponenty muszą być aktywne od pierwszego etapu,
- repo i AppHost powinny być przygotowane na ich późniejsze dokładanie,
- architektura nie może blokować przyszłych scenariuszy integracyjnych.

---

## ADR-013 — Ręczna budowa rozwiązania od czystego `.sln`

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Rozwiązanie jest budowane ręcznie od pustego `.sln`, krok po kroku, z pełną kontrolą struktury.

### Uzasadnienie
Podejście ręczne daje:
- pełną kontrolę nad nazwami i strukturą,
- lepszą jakość architektury startowej,
- mniejsze ryzyko przypadkowych zależności,
- większą przewidywalność przy dalszym rozwoju.

### Konsekwencje
- rozwój początkowy jest wolniejszy, ale bardziej kontrolowany,
- dokumentacja struktury i decyzji ma kluczowe znaczenie,
- każdy kolejny krok powinien być świadomie zatwierdzany.

---

## ADR-014 — Dokumentacja architektury utrzymywana w repozytorium

**Status:** zaakceptowana  
**Data:** 2026-04-18

### Decyzja
Dokumenty architektoniczne są przechowywane bezpośrednio w repozytorium w katalogu `docs`.

### Uzasadnienie
Dokumentacja blisko kodu:
- łatwiej nadąża za zmianami,
- jest częścią procesu rozwoju,
- ułatwia pracę ludziom i agentom AI,
- zmniejsza ryzyko rozjazdu między implementacją a opisem.

### Konsekwencje
- dokumentacja powinna być aktualizowana przy istotnych zmianach,
- decyzje architektoniczne muszą być zapisywane na bieżąco,
- repozytorium staje się jednym źródłem prawdy dla produktu.

---

## ADR-015 — Landing marketingowy: niezależny cykl życia ze wspólnym toolingiem

**Status:** zaakceptowana  
**Data:** 2026-04-19

### Decyzja
Landing marketingowy (`mavrynt-landing`) dostarczany jest jako niezależna aplikacja SPA (React + Vite), która:
- jest orkiestrowana przez `Mavrynt.AppHost` w środowisku deweloperskim dla spójności uruchamiania,
- wdraża się na dowolny hosting statyczny (CDN / object storage) bez runtime-owej zależności od `Mavrynt.Api`,
- korzysta ze wspólnego frontendowego toolingu (`@mavrynt/design-tokens`, `@mavrynt/ui`, `@mavrynt/config`, `@mavrynt/eslint-config`, `@mavrynt/tsconfig-base`) z wewnętrznej przestrzeni roboczej `src/frontend/shared/*`,
- używa portów i adapterów dla wszystkich elementów przekrojowych (analityka, formularz kontaktu, feature flags), z konsolowymi / no-op adapterami jako domyślnym runtime.

### Uzasadnienie
Landing jest publicznym wejściem do produktu i ma inną domenę awarii niż backend:
- musi działać nawet, gdy API jest w trybie konserwacji,
- jego cykl wydań wyznaczają potrzeby marketingu, a nie deploye backendu,
- korzysta z najtańszego możliwego modelu hostingu (CDN / statyczny) i agresywnego cache'owania.

Jednocześnie wyjmowanie landingu z monorepo obniżyłoby spójność wizualną i jakościową. Współdzielenie tokenów, prymitywów UI, konfiguracji ESLint i tsconfig utrzymuje standardy identyczne z pozostałymi aplikacjami SPA. Porty i adaptery zachowują opcję integracji z `Mavrynt.Api` w przyszłości (adapter HTTP `LeadService`) bez przepisywania komponentów.

### Konsekwencje
- landing nie może importować niczego z `src/backend/*`; integracje odbywają się wyłącznie przez adaptery,
- wszystkie ścieżki zbierania leadów idą przez port `LeadService` — bezpośrednie `fetch` w komponentach są niedozwolone,
- piramida testów landingu żyje wewnątrz aplikacji: Vitest (unit + integracyjne, jsdom) i Playwright (Chromium smoke dla kluczowych ścieżek),
- zasoby statyczne są wstępnie kompresowane (gzip + brotli) na etapie builda; hosting / CDN odpowiada za serwowanie,
- nowe wspólne pojęcia frontendowe (np. prymityw UI) trafiają do `src/frontend/shared/*`, a nie do konkretnej aplikacji SPA,
- poziomem bazowym dostępności jest WCAG 2.1 AA; regresje w etykietach, landmarkach czy zarządzaniu fokusem powinny łamać smoke e2e.

---

## ADR-016 — Rozwiązywanie URL-i między aplikacjami przez `@mavrynt/config/app-urls`

**Status:** Zaakceptowana  
**Data:** 2026-04-20

### Decyzja
Cała nawigacja między SPA (np. `mavrynt-landing` → `/login` w `mavrynt-web`, `mavrynt-admin` → SPA użytkownika, dowolne linki awaryjne w stopce) rozwiązuje adresy absolutne przez jeden pomocnik: `resolveAppUrls()` eksportowany z `@mavrynt/config/app-urls`. Pomocnik:
- przyjmuje opcjonalne źródło env (domyślnie `import.meta.env`), dzięki czemu można go testować w Node bez Vite,
- czyta kanoniczne klucze `VITE_APP_URL_LANDING`, `VITE_APP_URL_WEB`, `VITE_APP_URL_ADMIN` dla każdego środowiska,
- akceptuje legacy aliasy `VITE_MARKETING_URL`, `VITE_WEB_URL`, `VITE_ADMIN_URL` dla wstecznej zgodności z okablowaniem env z Fazy 2 / 3,
- stosuje fallback do developerskich portów Vite (`:5173`, `:5174`, `:5175`), gdy nic nie jest ustawione,
- normalizuje końcowe slashe i zamraża wynik, by wywołujący mogli bezpiecznie doklejać ścieżki.

### Uzasadnienie
Wkomponowane w JSX literały `VITE_*_URL` rozchodzą się w czasie (zmiana nazwy dla aplikacji, override dla środowiska, inny TLD dla wdrożenia). Skoncentrowanie rozwiązywania URL-i w jednym module:
- daje jedno miejsce do zmiany topologii wdrożenia,
- sprawia, że cross-cutting zmiana (np. dodanie czwartej SPA) jest mechaniczna, a nie grep-everywhere,
- izoluje dostęp do zmiennych środowiskowych, dzięki czemu reszta kodu pozostaje czysta i testowalna jednostkowo,
- odzwierciedla podejście porty-adaptery przyjęte dla analityki, zbierania leadów i autoryzacji (`ADR-015`).

### Konsekwencje
- żaden komponent frontendowy nie czyta zmiennych `VITE_*_URL` bezpośrednio — nawigacja zawsze przechodzi przez `resolveAppUrls()` / `resolveAppUrl(app)`,
- nowe wdrożenia ustawiają `VITE_APP_URL_*` w swoim pliku env Vite; legacy klucze są tolerowane przez jeden cykl wydania, a następnie wycofywane,
- testy wstrzykują zwykły `Record<string, string | undefined>` jako źródło env — bez stanu globalnego, bez gimnastyki `vi.stubEnv`,
- pomocnik mieszka w `@mavrynt/config` (nie w `@mavrynt/ui`), bo to konfiguracja runtime, a nie prezentacja.

---

## ADR-017 — `@mavrynt/auth-ui` jako osobny pakiet współdzielony

**Status:** Zaakceptowana  
**Data:** 2026-04-20

### Decyzja
UI autoryzacji (formularze logowania i rejestracji, port `AuthService`, domyślny adapter konsolowy, fabryka adaptera HTTP oraz dwujęzyczne zasoby i18n dla auth) żyje w osobnym pakiecie workspace `@mavrynt/auth-ui`, a nie wewnątrz `@mavrynt/ui`.

### Uzasadnienie
`@mavrynt/ui` jest biblioteką prezentacyjną: przyciski, sekcje, navbar, stopka — bez semantyki domenowej. Autoryzacja natomiast operuje na sesjach, poświadczeniach, typowanych kodach błędów (`invalid_credentials`, `email_taken`, `rate_limited`, …) oraz na abstrakcji nad endpointem backendu. Wtłoczenie tej domeny do `@mavrynt/ui` wymusiłoby:
- płacenie kosztu zależności auth przez każdego konsumenta prymitywów UI,
- zatarcie granicy między prezentacją a domeną — każda przyszła decyzja („czy to powinno iść do `ui`, czy gdzie indziej?") robiłaby się niejasna,
- kolizję z zakresem review opartym o WCAG: ścieżki auth mają wyższą poprzeczkę review niż wizualizacje marketingowe.

Utrzymanie dwóch pakietów osobno pozwala im ewoluować w swoim tempie. `@mavrynt/ui` pozostaje mały, tani w zależnościach i łatwy do ogarnięcia. `@mavrynt/auth-ui` posiada swój port, swoje adaptery i swoje zasoby i18n — podmiana backendu to zmiana providera `AuthServiceContext`, a nie edycja komponentów UI.

### Konsekwencje
- `@mavrynt/auth-ui` zależy od `@mavrynt/ui` (nie odwrotnie) i udostępnia formularze, port serwisu, port analityki oraz zasoby i18n,
- konsumujące aplikacje (`mavrynt-web`, `mavrynt-admin`) rejestrują paczkę i18n pod namespace'em `"auth"` w swoim bootstrapie i18n,
- aplikacje wstrzykują własny `AuthService` (`createConsoleAuthService` w dev, `createHttpAuthService` w prod przeciwko `Mavrynt.Api` / `Mavrynt.AdminApp`) przez `AuthServiceContext.Provider`,
- flagi funkcyjne na poziomie tras (`admin.register.enabled`) bramkują rejestrację admina bez modyfikacji `@mavrynt/auth-ui`,
- testy formularzy, hooków i adapterów żyją w zestawie Vitest konsumującej SPA (głównie `mavrynt-web`) — sam pakiet jest wysyłany w źródłach i nie ma własnego runnera.

---

## Zasady dopisywania kolejnych decyzji

Każda nowa decyzja powinna zawierać:
- identyfikator,
- status,
- datę,
- decyzję,
- uzasadnienie,
- konsekwencje.

Zalecany status:
- proponowana
- zaakceptowana
- odrzucona
- zastąpiona

---

## ADR-018 — Fundament BuildingBlocks bez blokady na konkretnego mediatora

**Status:** zaakceptowana  
**Data:** 2026-04-27

### Decyzja
Cztery projekty BuildingBlocks otrzymały podstawową implementację:
- `Mavrynt.BuildingBlocks.Domain` — prymitywy domenowe: `IEntity<TId>`, `IAggregateRoot`, `IDomainEvent`, `Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, `Error`, `Result`, `Result<T>`.
- `Mavrynt.BuildingBlocks.Application` — abstrakcje warstwy aplikacji: interfejsy znacznikowe komend i zapytań, interfejsy handlerów zwracające `Result`/`Result<T>`, interfejsy znacznikowe zachowań pipeline'u (walidacja, logowanie, transakcja), `IDateTimeProvider`, `ICurrentUserContext` oraz szkieletowy punkt rozszerzenia DI.
- `Mavrynt.BuildingBlocksContracts` — kontrakty integracyjne: `IIntegrationEvent`, `IntegrationEvent`, `IRequestContract`, `IResponseContract`, `PagedResponse<T>`.
- `Mavrynt.BuildingBlocks.Infrastructure` — abstrakcje infrastrukturalne: `IUnitOfWork`, `IRepository<TEntity, TId>`, `PostgreSqlOptions`, `ConfigurationExtensions`.

Implementacja celowo nie wprowadza MediatR, FluentValidation, EF Core, Npgsql ani żadnej konkretnej implementacji persystencji.

### Uzasadnienie
Moduł Users (i każdy kolejny moduł) wymaga stabilnych typów bazowych, które nie są powiązane z konkretnym mediatorem, biblioteką walidacji ani ORM. Blokada na MediatR lub FluentValidation na tym etapie wymusiłaby te zależności na wszystkich modułach, zanim będzie wystarczający kontekst do podjęcia świadomej decyzji. Zdefiniowane interfejsy komend, zapytań i handlerów są neutralne bibliotecznie i mogą być zaadaptowane do dowolnej implementacji pipeline'u. Utrzymanie warstwy domenowej wolnej od zależności infrastrukturalnych i frameworkowych zapewnia jej przenośność i łatwość testowania.

### Konsekwencje
- wszystkie encje domenowe muszą dziedziczyć po `Entity<TId>` lub `AggregateRoot<TId>` z BuildingBlocks.Domain,
- wszystkie przypadki użycia aplikacji muszą implementować `ICommandHandler` lub `IQueryHandler` z BuildingBlocks.Application,
- zdarzenia integracyjne między modułami muszą dziedziczyć po `IntegrationEvent` z BuildingBlocks.Contracts,
- wybór konkretnego mediatora (MediatR, Wolverine lub własny dispatcher) pozostaje otwartą decyzją,
- wybór biblioteki walidacji (FluentValidation lub DataAnnotations) pozostaje otwartą decyzją,
- konkretny DbContext EF Core i strategia migracji pozostają otwartymi decyzjami,
- `BuildingBlocks.Infrastructure` nie może stać się ogólnym zrzutniem narzędzi — należą tam tylko naprawdę współdzielone abstrakcje infrastrukturalne.

---

## Otwarte obszary do przyszłych decyzji

Poniższe obszary wymagają późniejszych decyzji:
- dokładny model autoryzacji i ról,
- model modułu feature management,
- standard endpointów i wersjonowania API,
- wybór bibliotek i wzorców dla walidacji i mediatorów,
- model migracji danych,
- strategia testów architektonicznych,
- model komunikacji asynchronicznej,
- standard wdrożeń środowiskowych,
- standard zarządzania sekretami i konfiguracją,
- model CI/CD.

---

## Podsumowanie

Dokument stanowi rejestr decyzji architektonicznych dla Mavrynt. Ma pomagać utrzymać spójność rozwiązania podczas jego ręcznego budowania i dalszego rozwijania. Każda istotna decyzja zmieniająca kierunek architektury powinna zostać tutaj dopisana.- `Mavrynt.BuildingBlocksContracts` — kontrakty zdarzeń integracyjnych: `IIntegrationEvent`, `IntegrationEvent`, `IRequestContract`, `IResponseContract`, `PagedResponse`.
- `Mavrynt.BuildingBlocks.Infrastructure` — abstrakcje infrastruktury: `IRepository<TEntity, TId>`, `IUnitOfWork`, `PostgreSqlOptions` oraz helpery konfiguracyjne.

Interfejsy handlerów komend i zapytań są neutralne bibliotecznie — nie wprowadzono MediatR ani żadnej innej biblioteki mediatora. Moduły podłączają własne handlery bezpośrednio przez `ICommandHandler<TCommand, TResponse>` i `IQueryHandler<TQuery, TResponse>`.

### Uzasadnienie
Definiowanie interfejsów w BuildingBlocks bez wiązania się z konkretnym mediatorem pozostawia każdemu modułowi swobodę bezpośredniego wywoływania komend i zapytań, lub późniejszego przyjęcia mediatora bez zmiany kontraktów handlerów. Abstrakcje są testowalne bez kontenera IoC. Miejsca na zachowania pipeline'u (logowanie, walidacja, transakcja) zarezerwowano jako interfejsy znacznikowe, aby przyszły pipeline można było podpiąć bez modyfikacji sygnatur handlerów.

### Konsekwencje
- Wszystkie projekty Domain modułów referują wyłącznie `Mavrynt.BuildingBlocks.Domain`.
- Wszystkie projekty Application modułów referują `Mavrynt.BuildingBlocks.Application` i `Mavrynt.BuildingBlocks.Domain`.
- Implementacje Infrastructure referują `Mavrynt.BuildingBlocks.Infrastructure`.
- Na tym etapie nie wprowadzono zależności od MediatR, FluentValidation ani AutoMapper.
- Dispatch handlerów i zachowania pipeline'u są odroczone do czasu, gdy konkretny moduł udowodni taką potrzebę.

---

## ADR-019 — Fundament Domain i Application modułu Users

**Status:** zaakceptowana  
**Data:** 2026-04-27

### Decyzja
`Mavrynt.Modules.Users` jest pierwszym konkretnym modułem zbudowanym na bazie BuildingBlocks.

Na tym etapie dostarczono wyłącznie dwie warstwy:

**Domain (`Mavrynt.Modules.Users.Domain`)**
- Value objects: `UserId`, `Email`, `PasswordHash`, `UserDisplayName` — wszystkie niemutowalne, tworzenie przez `Result<T>`, brak wyjątków przy błędach walidacji.
- Agregat: `User` (`AggregateRoot<UserId>`) — statyczna fabryka `Register`, metody behawioralne (`ChangeEmail`, `ChangePasswordHash`, `ChangeDisplayName`, `Activate`, `Deactivate`), prywatny konstruktor bezparametrowy zgodny z ORM.
- Enum: `UserStatus` (`Active`, `Inactive`, `Locked`).
- Zdarzenia domenowe: `UserRegisteredDomainEvent`, `UserEmailChangedDomainEvent`, `UserPasswordChangedDomainEvent` — niemutowalne rekordy implementujące `IDomainEvent`.
- Abstrakcja repozytorium: `IUserRepository` — cztery metody, brak implementacji.
- Błędy: `UserErrors` — silnie nazwane stałe `Error`, bez rozproszonych literałów łańcuchowych w handlerach.

**Application (`Mavrynt.Modules.Users.Application`)**
- DTO: `UserDto`, `AuthResultDto` — czyste rekordy eksponujące wyłącznie prymitywy, bez typów domenowych w publicznym kontrakcie.
- Komendy: `RegisterUserCommand`, `LoginUserCommand`, `ChangeUserEmailCommand`, `ChangeUserPasswordCommand` — niemutowalne rekordy.
- Handlery komend: jedna klasa na komendę, zależą wyłącznie od abstrakcji `IUserRepository` i `IDateTimeProvider`.
- Zapytania: `GetUserByIdQuery`, `GetUserByEmailQuery` — niemutowalne rekordy.
- Handlery zapytań: pobierają dane z `IUserRepository`, mapują do `UserDto`, nigdy nie eksponują encji domenowych.
- Mapowanie: `UserMappings` — `internal static`, metoda rozszerzająca `ToDto()`, bez AutoMapper ani Mapster.
- DI: `AddUsersApplication(IServiceCollection)` — rejestruje handlery przez interfejs; nie rejestruje usług infrastrukturalnych.

### Uzasadnienie
Zdefiniowanie modelu domenowego i kontraktów przypadków użycia przed infrastrukturą pozwala niezależnie dodać warstwy persystencji i API w przyszłym etapie. Utrzymanie domeny jako niezależnej od persystencji oraz aplikacji wolnej od referencji do Infrastructure egzekwuje regułę zależności i sprawia, że obie warstwy są testowalnie izolowane przy użyciu stubów repozytorium.

Silnie nazwane błędy (`UserErrors`) eliminują magiczne łańcuchy z handlerów i stanowią jedno źródło prawdy dla kodów błędów konsumowanych przez przyszłe warstwy API i frontendu.

`LoginUserCommand` jest celowo minimalna: porównuje wyłącznie wartości wcześniej zahashowanych haseł i zwraca `AuthResultDto` z `TokenType: "not_implemented"`. Strategia tokenów/sesji/ciasteczek jest oddzielną decyzją architektoniczną odroczoną do etapu Infrastructure + API.

### Konsekwencje
- `Mavrynt.Modules.Users.Domain` nie referuje Application, Infrastructure, ASP.NET ani EF Core.
- `Mavrynt.Modules.Users.Application` nie referuje Infrastructure ani hostów.
- Brak konkretnej persystencji.
- Brak wystawionych endpointów API.
- Brak sesji uwierzytelniania, JWT, ciasteczek ani tokenów odświeżania.
- Nie wprowadzono biblioteki mediatora.
- Nie wprowadzono biblioteki walidacji.
- Infrastructure (konfiguracja encji EF Core, DbContext, migracje, podłączenie PostgreSQL), endpointy API, biblioteka hashowania haseł, strategia JWT/ciasteczek, model ról oraz zarządzanie administratorami są odroczone do przyszłych etapów.

---

## ADR-020 — Wewnętrzny mediator i zachowania pipeline'u aplikacji

**Status:** Zaakceptowano
**Data:** 2026-04-28

### Decyzja

Mavrynt używa lekkiego wewnętrznego mediatora (`MavryntMediator`) zamiast MediatR lub jakiejkolwiek zewnętrznej biblioteki mediatora.

Komendy i zapytania są przekazywane przez `IMediator`:

```csharp
Task<Result> SendAsync(ICommand command, CancellationToken ct = default);
Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken ct = default);
Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query, CancellationToken ct = default);
```

Wprowadzony jest generyczny interfejs zachowania pipeline'u jako jedyny punkt rozszerzenia dla przekrojowych zagadnień:

```csharp
public interface IMavryntBehavior<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        Func<CancellationToken, Task<TResponse>> next,
        CancellationToken cancellationToken);
}
```

Pięć wbudowanych zachowań rejestrowanych w tej kolejności:

1. **LoggingBehavior** — loguje typ żądania, kategorię, czas wykonania, sukces/porażkę, kod błędu i trace ID. Nigdy nie serializuje całego żądania.
2. **ValidationBehavior** — rozwiązuje wszystkie `IValidator<TRequest>` z DI i zatrzymuje się na pierwszym błędzie.
3. **ResilienceBehavior** — punkt wejścia dla retry/timeout; respektuje marker `IResilientRequest`. Brak zależności od Polly na tym etapie.
4. **AuditBehavior** — zapisuje `AuditEntry` przez `IAuditService` dla żądań implementujących `IAuditableRequest`.
5. **TransactionBehavior** — wywołuje `IUnitOfWork.SaveChangesAsync()` po sukcesie handlera dla żądań implementujących `ITransactionalRequest`.

Markery żądań kontrolują opcjonalną aktywację zachowań:
- `IAuditableRequest` — włącza zapis audytu
- `IResilientRequest` — oznacza żądania bezpieczne do powtórzenia
- `ITransactionalRequest` — włącza commit jednostki pracy po sukcesie

Istniejące `ICommand`, `ICommand<TResponse>`, `IQuery<TResponse>` i interfejsy handlerów pozostają bez zmian. Handlery nadal zwracają `Result` lub `Result<TResponse>`.

`Result` i `Error` pozostają standardowym modelem zwrotnym w całej warstwie aplikacji.

Rejestracja DI i odkrywanie handlerów przez jeden method extension:

```csharp
services.AddMavryntMediator(params Assembly[] assemblies);
```

Endpointy wstrzykują `IMediator`, nie konkretne interfejsy handlerów.

### Uzasadnienie

- **Pełna kontrola nad obsługą Result/Error.** Brak warstwy adaptera między `Result<T>` a modelem odpowiedzi zewnętrznego mediatora.
- **Brak zewnętrznej zależności od mediatora.** Zmniejsza ryzyko łańcucha dostaw i sprzężenia wersji.
- **Czytelność dla agentów AI.** Cała logika zachowań żyje we własnej przestrzeni nazw Mavrynt z jawną dokumentacją XML.
- **Czysty punkt rozszerzenia.** Dodanie nowego przekrojowego zagadnienia to jedna klasa implementująca `IMavryntBehavior<TRequest, TResponse>` i jedna linia w rejestracji DI.
- **Przyszła wymiana jest bezpieczna.** Wszystkie miejsca wywołań zależą od wewnętrznej abstrakcji `IMediator`. Wymiana implementacji nie wymaga zmian w handlerach ani endpointach.
- **Jawna kolejność pipeline'u.** Kolejność jest określona przez rejestrację DI w `AddMavryntMediator`, widoczna w jednym pliku.

### Konsekwencje

- `IMediator` jest jedynym punktem wejścia dla dyspozycji komend i zapytań w endpointach i serwisach aplikacji.
- MediatR nie jest dodawany do rozwiązania. Każda przyszła decyzja o jego adopcji wymaga jawnego ADR.
- FluentValidation nie jest wprowadzana. Używana jest wewnętrzna abstrakcja `IValidator<TRequest>`. Przyszły ADR może ją zastąpić.
- Polly nie jest wprowadzane dla zachowania resilience. Przyszły ADR może je dodać.
- Trzy starsze marker interfejsy (`ILoggingBehavior`, `IValidationBehavior`, `ITransactionBehavior`) są oznaczone `[Obsolete]` i zostaną usunięte w przyszłym sprzątaniu.
- `IUnitOfWork` jest zdefiniowane zarówno w `Mavrynt.BuildingBlocks.Application.Persistence` (kanonична abstrakcja), jak i w `Mavrynt.BuildingBlocks.Infrastructure.Persistence` (marker Infrastructure rozszerzający interfejs Application). Konkretne implementacje EF Core muszą być rejestrowane względem interfejsu Application.
- Nieobsłużone wyjątki w pipeline'ie są przechwytywane przez mediator, logowane i zwracane jako `Result.Failure` z trace ID. Nie propagują się jako wyjątki do granicy API.

---

## ADR-021 — Backend test strategy: architecture, unit and Testcontainers integration tests

**Status:** zaakceptowana  
**Data:** 2026-04-28

### Decyzja
Fundament testów backendu zostaje ustandaryzowany do trzech warstw:
- testy architektoniczne dla granic modułów,
- testy jednostkowe handlerów command/query z fake,
- testy integracyjne na realnym PostgreSQL przez Testcontainers.

### Uzasadnienie
Takie podejście ogranicza dryf architektury, pozwala testować logikę aplikacyjną deterministycznie oraz weryfikuje integrację persystencji i hostów bez współdzielonych środowisk.

### Konsekwencje
- testy architektoniczne stają się bramką dla kierunku zależności,
- handlery command/query są testowane izolowanie przez fake in-memory,
- testy infrastruktury i hostów API/Admin używają kontenerów PostgreSQL,
- testy nie zależą od Aspire AppHost ani Docker Compose,
- wersje pakietów pozostają centralnie zarządzane w `Directory.Packages.props`,
- testy backendowe są elementem fundamentu Continuous Delivery.

---

## ADR-022 — Administracyjny vertical slice Fazy 1: role użytkowników, FeatureManagement i trwały audyt

**Status:** zaakceptowana  
**Data:** 2026-04-29

### Decyzja

Pierwszy administracyjny vertical slice Fazy 1 jest zaimplementowany i podłączony do `Mavrynt.AdminApp`. Slice dostarcza trzy możliwości:

**1. Przypisywanie ról użytkownikom**
- `PATCH /api/admin/users/{userId}/role` — przypisuje rolę `Admin` lub `User` istniejącemu użytkownikowi.
- Zabezpieczony polityką `AdminOnly` (JWT Bearer + claim roli `Admin`).
- Zaimplementowany jako `AssignUserRoleCommand` w `Mavrynt.Modules.Users.Application`.
- Ślad audytu zapisywany przez istniejące `IAuditService` (BuildingBlocks) z typem zdarzenia `user_role_assigned`.

**2. Moduł FeatureManagement**
- Nowy moduł: `Mavrynt.Modules.FeatureManagement.Domain`, `…Application`, `…Infrastructure`.
- Agregat `FeatureFlag`: klucz (`^[a-z0-9][a-z0-9._-]*$`, maks. 256 znaków), nazwa, opis, stan włączona/wyłączona, znaczniki czasowe.
- Komendy CRUD: `CreateFeatureFlagCommand`, `UpdateFeatureFlagCommand`, `ToggleFeatureFlagCommand`.
- Zapytania: `ListFeatureFlagsQuery`, `GetFeatureFlagByKeyQuery`.
- Endpointy w AdminApp: `GET/POST /api/admin/feature-flags`, `GET/PATCH /api/admin/feature-flags/{key}`, `PATCH /api/admin/feature-flags/{key}/toggle`.
- Wszystkie endpointy są chronione polityką `AdminOnly`.
- Feature flagi są zarządzane wyłącznie przez AdminApp — brak endpointów odczytu po stronie użytkownika na tym etapie.
- Persystencja: schemat PostgreSQL `feature_management`, tabela `feature_flags`, migracje EF Core, unikalny indeks na `key`.

**3. Moduł Audit**
- Nowy moduł: `Mavrynt.Modules.Audit.Domain`, `…Application`, `…Infrastructure`.
- Encja `AuditLogEntry` — rekord tylko do dopisywania: id aktora, akcja, typ zasobu, id zasobu, czas wystąpienia, metadane JSON.
- `IAuditLogWriter` (Audit.Application) — nowa abstrakcja do zapisu audytu administratorskiego/systemowego, oddzielna od `IAuditService` (BuildingBlocks) obsługującego zdarzenia uwierzytelniania.
- `FeatureManagement.Application` zależy od `Audit.Application` poprzez `IAuditLogWriter`.
- Persystencja: schemat PostgreSQL `audit`, tabela `audit_log_entries`, migracje EF Core.

**Dodana pokrycie testami**
- Testy architektoniczne: 6 nowych testów dla reguł zależności warstw FM i Audit.
- Testy jednostkowe domeny: 24 nowe testy dla agregatu `FeatureFlag` i value objects.
- Testy jednostkowe aplikacji: 12 nowych testów dla handlerów FM (fake, bez I/O).
- Testy jednostkowe aplikacji: 5 nowych testów dla `AssignUserRoleCommandHandler`.
- Testy integracyjne infrastruktury: 6 nowych testów dla `FeatureFlagRepository` na realnym kontenerze PostgreSQL.
- Testy integracyjne AdminApp: 5 testów dla endpointu ról użytkowników; 11 testów dla endpointów feature flag.

### Uzasadnienie

Administracyjny slice zamyka lukę między fundament backendu (Users, JWT, mediator, testy) a działającym backendem administracyjnym. Dostarczenie przypisywania ról, zarządzania flagami i audytu w jednym slicie zapewnia, że każda administracyjna mutacja jest obserwowalana od pierwszego dnia.

Oddzielenie `IAuditLogWriter` od istniejącego `IAuditService` utrzymuje BuildingBlocks wolne od semantyki audytu specyficznej dla modułów. `IAuditLogWriter` należy do `Audit.Application` — jedynego modułu odpowiedzialnego za zagadnienie audytu.

FeatureManagement jest na tym etapie dostępny wyłącznie przez AdminApp. Udostępnianie flag API użytkownika lub ich użycie jako bramek runtime to oddzielne przyszłe decyzje.

### Konsekwencje

- Polityka `AdminOnly` jest jedynym modelem autoryzacji dla endpointów administracyjnych; pełne RBAC (uprawnienia granularne per typ zasobu) nie jest zaimplementowane.
- `userId` aktora nie jest jeszcze propagowany do `IAuditLogWriter.WriteAsync()` — wszystkie wpisy audytu administracyjnego są zapisywane z `actorUserId: null`. Propagacja przez `ICurrentUserContext` jest odroczona.
- `FeatureManagement.Application` ma świadomą zależność cross-module od `Audit.Application`. Jest to jedyna dozwolona zależność cross-module na poziomie warstwy aplikacji i nie może być rozszerzana bez nowego ADR.
- `IDateTimeProvider` jest rejestrowane przez `TryAddSingleton` w FeatureManagement.Infrastructure, aby pierwsza rejestracja (Users.Infrastructure) wygrała gdy oba moduły są aktywne w tym samym hoście.
- Konfiguracja pipeline CI/CD, środowiska stagingowe i automatyzacja wdrożeń pozostają odroczone do późniejszego etapu.

---

## ADR-023 — Moduł Notifications: SMTP zarządzany w bazie, silnik szablonów i IEmailNotificationService

**Status:** zaakceptowana  
**Data:** 2026-04-30

### Decyzja

Dodano nowy moduł `Notifications` (`Domain`, `Application`, `Infrastructure`) do obsługi całej wychodzacej komunikacji e-mail. Kluczowe decyzje projektowe:

**Konfiguracja SMTP**
- Agregat `SmtpSettings` przechowuje nazwę dostawcy, host, port, dane uwierzytelniające, tożsamość nadawcy, flagę SSL i stan włączenia w tabeli PostgreSQL `notifications.smtp_settings`.
- Dokładnie jeden dostawca jest aktywny w danej chwili; włączenie nowego wyłącza wszystkich pozostałych (`DisableAllAsync` przed włączeniem docelowego).
- Hasła są przechowywane przez `ISecretProtector`. Domyślna implementacja (`PassThroughSecretProtector`) to przeźroczyste przejście przeznaczone wyłącznie do developmentu. Przed produkcją należy ją zastąpić DPAPI, Azure Key Vault lub podobnym rozwiązaniem.
- Hasła nigdy nie są zwracane w DTO, logowane ani umieszczane w wpisach audytu.

**Szablony e-mail**
- Trzy predefiniowane szablony są siane przy starcie: `auth.login_confirmation`, `auth.password_reset`, `auth.two_factor_code`.
- Szablony przechowywane w `notifications.email_templates` z unikalnym indeksem na `template_key`.
- Treść szablonów może być aktualizowana przez administratorów, ale klucze są niemutowalne. Szablony mogą być indywidualnie wyłączane.
- Seeder jest idempotentny — nie nadpisuje istniejących szablonów.

**Renderowanie szablonów**
- `EmailTemplateRenderer` (warstwa Application, bez zależności infrastrukturalnych) rozwiązuje tokeny `{{Placeholder}}` za pomocą wyrażenia regularnego.
- Wartości dla treści HTML są kodowane przez `WebUtility.HtmlEncode`; wartości tematu i treści tekstowej używane są bez zmian.
- Nieznane placeholdery zwracają `NotificationsErrors.EmailUnknownPlaceholder(key)` — brak cichego podstawiania.

**IEmailNotificationService**
- Generyczna metoda `SendAsync<TModel>(key, recipient, model, ct)` jest jedynym publicznym kontraktem dla wysyłania e-maili cross-module.
- `TModel : IEmailModel` dostarcza słownik `ToPlaceholders()`, chroniąc kod wywołujący przed manipulacją ciągami znaków.
- Gotowe typy modeli obejmują wszystkie trzy predefiniowane szablony; przyszłe szablony wymagają nowego typu modelu.

**Infrastruktura**
- `SmtpEmailSender` używa `System.Net.Mail.SmtpClient` (wyłącznie BCL, bez zewnętrznych bibliotek SMTP).
- Migracje są pisane ręcznie zgodnie z wzorcem FeatureManagement.
- `NotificationsStartupService` (IHostedService) uruchamia migrację bazy danych, a następnie sianie szablonów sekwencyjnie przy starcie.

**Endpointy administracyjne**
- Ustawienia SMTP: lista, get-by-id, tworzenie, aktualizacja, włączanie (`/api/admin/notifications/smtp-settings`).
- Szablony e-mail: lista, get-by-key, aktualizacja, lista-definicji, testowe wysyłanie (`/api/admin/notifications/email`).
- Wszystkie endpointy są chronione polityką `AdminOnly`; brak endpointów odczytu po stronie użytkownika.

### Uzasadnienie

Powiadomienia e-mail to kluczowa zdolność przekrojowa potrzebna modułowi Users (potwierdzenie logowania, reset hasła, 2FA). Zdefiniowanie `IEmailNotificationService` w Notifications.Application i konsumowanie go z innych modułów izoluje zagadnienie wysyłania. Użycie BCL `SmtpClient` unika zależności od bibliotek zewnętrznych, zachowując łatwość zamiany. Umieszczenie `EmailTemplateRenderer` w Application (zamiast Infrastructure) pozwala testować logikę renderowania bez połączenia z bazą danych ani SMTP. `ISecretProtector` dostarcza bezpieczny punkt rozszerzenia dla produkcyjnego szyfrowania bez wiązania domeny z konkretnym magazynem sekretów.

### Konsekwencje

- `Notifications.Application` ma cross-modułową zależność od `Audit.Application` (dla `IAuditLogWriter`) — ten sam wzorzec co w ADR-022. Żadne inne cross-modułowe zależności warstwy Application nie są dozwolone bez nowego ADR.
- `IDateTimeProvider` jest rejestrowane przez `TryAddSingleton` w `NotificationsInfrastructure`, aby pierwsza rejestracja wygrała gdy wiele modułów jest aktywnych.
- `PassThroughSecretProtector` musi zostać zastąpiony prawdziwą implementacją szyfrowania przed obsługą rzeczywistych poświadczeń SMTP w środowisku niedevlopmentowym.
- Klucze szablonów są niemutowalne po zasianiu. Dodanie czwartego szablonu wymaga nowej migracji, nowego wpisu w seederze, nowej implementacji `IEmailModel` i nowej stałej `EmailTemplateKey`.
- `SmtpClient` (BCL) nie obsługuje OAuth ani nowoczesnych przepływów uwierzytelniania. Zastąpienie go MailKit lub adapterem transakcyjnego API e-mail wymaga tylko nowej implementacji `IEmailSender` — bez innych zmian w kodzie.
