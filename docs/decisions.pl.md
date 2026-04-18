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