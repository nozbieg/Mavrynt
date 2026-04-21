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

Dokument stanowi rejestr decyzji architektonicznych dla Mavrynt. Ma pomagać utrzymać spójność rozwiązania podczas jego ręcznego budowania i dalszego rozwijania. Każda istotna decyzja zmieniająca kierunek architektury powinna zostać tutaj dopisana.