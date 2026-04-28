# Mavrynt — Architektura rozwiązania

## 1. Cel dokumentu

Celem dokumentu jest opisanie docelowej architektury rozwiązania Mavrynt na poziomie repozytorium, głównych komponentów, odpowiedzialności projektów oraz zasad zależności pomiędzy warstwami. Dokument ma stanowić wspólny punkt odniesienia dla dalszej implementacji wykonywanej ręcznie oraz z użyciem agentów AI.

Architektura została zaprojektowana tak, aby:
- umożliwiać rozwój produktu etapami,
- zachować wysoką czytelność kodu i odpowiedzialności,
- wspierać modularny monolit jako model startowy,
- być gotową do rozbudowy o dodatkowe moduły domenowe,
- wspierać ciągłe dostarczanie, obserwowalność, testowalność oraz zarządzanie funkcjami.

---

## 2. Kontekst systemu

Mavrynt jest platformą budowaną jako jeden produkt w jednym repozytorium. Na obecnym etapie fundamentem jest część administracyjna i bazowa warstwa produktowa, obejmująca między innymi:
- użytkowników,
- uwierzytelnianie i autoryzację,
- role i uprawnienia,
- obsługę haseł oraz resetów,
- podstawowe procesy systemowe,
- funkcje administracyjne,
- mechanizm feature flag,
- przygotowanie pod komunikację mailową,
- bazową observability i zarządzanie środowiskami.

Rozwiązanie obejmuje:
- backend aplikacyjny,
- osobną aplikację administracyjną,
- frontend użytkownika,
- frontend administracyjny,
- statyczny landing marketingowy,
- elementy infrastrukturalne i wdrożeniowe.

---

## 3. Styl architektoniczny

### 3.1. Główny model

Przyjętym stylem architektonicznym jest **modular monolith**.

Oznacza to, że:
- system działa jako jeden logiczny produkt,
- moduły domenowe są wydzielone strukturalnie,
- każdy moduł posiada własne granice odpowiedzialności,
- zależności pomiędzy modułami są kontrolowane,
- komunikacja wewnętrzna ma być możliwie jawna i przewidywalna,
- ewentualne przyszłe wydzielenie usług nie jest wykluczone, ale nie jest celem początkowym.

### 3.2. Uzasadnienie wyboru

Modularny monolit został wybrany, ponieważ:
- zmniejsza koszt wejścia i złożoność operacyjną,
- ułatwia ręczne rozwijanie architektury od czystego `.sln`,
- dobrze wspiera pojedyncze repozytorium,
- umożliwia szybkie iterowanie nad fundamentem produktu,
- daje rozsądny kompromis pomiędzy separacją a prostotą utrzymania.

---

## 4. Główne założenia architektoniczne

### 4.1. Jedno repozytorium

Całość rozwiązania znajduje się w jednym repozytorium. Repozytorium zawiera:
- backend,
- frontendy,
- testy,
- dokumentację,
- skrypty,
- konfigurację build i deploy.

### 4.2. Jasny podział odpowiedzialności

Każdy projekt powinien mieć możliwie jednoznaczną odpowiedzialność. Nie należy mieszać:
- hostów aplikacyjnych,
- logiki domenowej,
- logiki aplikacyjnej,
- infrastruktury,
- kontraktów,
- kodu wspólnego niezwiązanego z odpowiedzialnością danego projektu.

### 4.3. Zależności skierowane do środka

Warstwy wyższego poziomu mogą zależeć od warstw niższych semantycznie, ale nie odwrotnie. W praktyce:
- `Domain` nie zna `Infrastructure`,
- `Domain` nie zna hostów,
- `Application` nie zależy od hosta,
- `Infrastructure` implementuje potrzeby warstw wyższych,
- host scala moduły i konfigurację uruchomieniową.

### 4.4. Backend jako rdzeń produktu

Backend jest centralnym elementem systemu. Frontendy są klientami backendu lub osobnymi warstwami prezentacji.

### 4.5. Admin jako osobny obszar odpowiedzialności

Część administracyjna jest przewidziana jako osobny obszar produktu. Ma własny host backendowy i własny frontend administracyjny, nawet jeśli w pierwszych iteracjach część logiki pozostaje wspólna.

---

## 5. Główne komponenty rozwiązania

## 5.1. `Mavrynt.AppHost`

Projekt uruchomieniowy przeznaczony do lokalnej orkiestracji i spójnego spinania zależności środowiskowych. Jego zadaniem jest:
- uruchamianie elementów backendu podczas developmentu,
- spinanie zależności infrastrukturalnych,
- przygotowanie środowiska developerskiego,
- uproszczenie startu rozwiązania lokalnie.

Na późniejszych etapach może integrować:
- PostgreSQL,
- Redis,
- RabbitMQ,
- Kafka,
- inne elementy wspierające development i testy integracyjne.

## 5.2. `Mavrynt.ServiceDefaults`

Projekt zawierający wspólne ustawienia usług i standardy techniczne. Przykładowe odpowiedzialności:
- konfiguracja observability,
- wspólne rozszerzenia hostingu,
- standardy health checks,
- wspólne ustawienia telemetryczne,
- elementy wspólne dla uruchamianych usług backendowych.

## 5.3. `Mavrynt.Api`

Główny host API produktu. Odpowiada za:
- ekspozycję endpointów aplikacyjnych,
- kompozycję modułów,
- konfigurację pipeline HTTP,
- autoryzację i uwierzytelnianie na poziomie hosta,
- integrację z modułami backendowymi.

To główny punkt wejścia dla klienta użytkownika.

## 5.4. `Mavrynt.AdminApp`

Host backendowy przeznaczony dla obszaru administracyjnego. Odpowiada za:
- funkcje administracyjne,
- zarządzanie użytkownikami,
- zarządzanie rolami i dostępami,
- zarządzanie feature flagami,
- późniejsze obszary administracyjne systemu.

To osobny host, ponieważ admin jest wydzieloną częścią produktu o odmiennych wymaganiach bezpieczeństwa, uprawnień i odpowiedzialności.

## 5.5. `Mavrynt.BuildingBlocks.*`

Zestaw projektów wspólnych dla całego backendu.

### `Mavrynt.BuildingBlocks.Domain`
Kod bazowy dla warstwy domenowej, na przykład:
- klasy bazowe encji,
- value object patterns,
- błędy domenowe,
- abstrakcje domenowe.

### `Mavrynt.BuildingBlocks.Application`
Kod bazowy dla warstwy aplikacyjnej, na przykład:
- abstrakcje command/query,
- wspólne interfejsy use case,
- walidacja i pipeline behaviors,
- wzorce dla rejestracji zależności.

### `Mavrynt.BuildingBlocks.Infrastructure`
Kod wspólny infrastrukturalnie, na przykład:
- mechanizmy persystencji,
- rozszerzenia konfiguracyjne,
- elementy integracyjne,
- wzorce implementacyjne dla dostępu do danych.

### `Mavrynt.BuildingBlocks.Contracts`
Kontrakty współdzielone, na przykład:
- zdarzenia integracyjne,
- request/response contracts,
- komunikaty między modułami lub warstwami.

## 5.6. `Mavrynt.Modules.*`

Moduły domenowe systemu. Każdy moduł powinien posiadać własne granice odpowiedzialności.

Na obecnym etapie pierwszym modułem bazowym jest `Users`.

Przykładowy układ:
- `Mavrynt.Modules.Users.Domain`
- `Mavrynt.Modules.Users.Application`
- `Mavrynt.Modules.Users.Infrastructure`

W przyszłości analogicznie mogą powstać moduły:
- Identity
- Authorization
- FeatureManagement
- Notifications
- Audit
- Billing
- Signals
- MarketData
- NewsAnalysis

Ich dokładna lista będzie zależeć od kolejnych etapów rozwoju produktu.

---

## 6. Model warstw w module

Każdy moduł backendowy powinien dążyć do spójnego modelu warstwowego.

## 6.1. Warstwa domenowa

Warstwa domenowa zawiera:
- encje,
- value objects,
- enumy domenowe,
- zdarzenia domenowe,
- repozytoria jako abstrakcje,
- reguły biznesowe.

Ta warstwa nie powinna znać:
- EF Core,
- HTTP,
- hostingu,
- implementacji infrastrukturalnych,
- frameworków prezentacyjnych.

## 6.2. Warstwa aplikacyjna

Warstwa aplikacyjna zawiera:
- przypadki użycia,
- komendy,
- zapytania,
- DTO,
- kontrakty wejścia i wyjścia,
- orkiestrację logiki modułu.

Warstwa aplikacyjna korzysta z domeny, ale nie powinna przejmować odpowiedzialności za szczegóły persystencji czy hostingu.

## 6.3. Warstwa infrastruktury

Warstwa infrastruktury zawiera:
- implementacje repozytoriów,
- konfigurację bazy danych,
- mapowania,
- integracje zewnętrzne,
- konfigurację techniczną modułu.

To tutaj znajdują się szczegóły technologiczne.

## 6.4. Host

Host:
- rejestruje moduły,
- wystawia endpointy,
- spina konfigurację środowiska,
- obsługuje cross-cutting concerns na poziomie aplikacji,
- nie powinien zawierać logiki domenowej.

---

## 7. Zasady zależności

## 7.1. Dozwolone zależności

Przykładowo dozwolone są:
- `BuildingBlocks.Application` → `BuildingBlocks.Domain`
- `BuildingBlocks.Infrastructure` → `BuildingBlocks.Domain`
- `BuildingBlocks.Infrastructure` → `BuildingBlocks.Application`
- `Modules.Users.Domain` → `BuildingBlocks.Domain`
- `Modules.Users.Application` → `BuildingBlocks.Application`
- `Modules.Users.Application` → `Modules.Users.Domain`
- `Modules.Users.Infrastructure` → `Modules.Users.Application`
- `Modules.Users.Infrastructure` → `Modules.Users.Domain`
- `Mavrynt.Api` → moduły i building blocks
- `Mavrynt.AdminApp` → moduły i building blocks

## 7.2. Niedozwolone zależności

Nie wolno dopuszczać sytuacji:
- `Domain` → `Infrastructure`
- `Domain` → host
- `Application` → host
- moduł A zależy bez kontroli od szczegółów modułu B
- frontend zależy bezpośrednio od projektów backendowych
- landing marketingowy zależy od logiki aplikacyjnej backendu

---

## 8. Frontend i warstwa prezentacji

Repozytorium przewiduje trzy główne aplikacje frontendowe:

### `mavrynt-web`
Główny frontend użytkownika.

### `mavrynt-admin`
Frontend administracyjny.

### `mavrynt-landing`
Statyczny landing marketingowy, bez zależności od backendowego modelu domenowego. Może korzystać jedynie z prostych mechanizmów integracyjnych lub całkowicie statycznej zawartości.

Frontendy są utrzymywane oddzielnie od backendu, ale w tym samym repozytorium.

---

## 9. Dane i infrastruktura

Docelowo architektura zakłada wykorzystanie:
- PostgreSQL jako głównej bazy danych,
- Redis jako cache i warstwy pomocniczej,
- RabbitMQ oraz Kafka jako elementów komunikacji asynchronicznej zależnie od potrzeb,
- Docker i Compose jako podstawy lokalnego środowiska oraz wdrożeń technicznych.

Na obecnym etapie dokument opisuje kierunek architektoniczny, a nie kompletną implementację infrastrukturalną.

---

## 10. Feature flags

Mechanizm feature flag jest istotnym elementem architektury. Założenia:
- flagi mają być wspierane od początku,
- zarządzanie flagami ma być dostępne z poziomu części administracyjnej,
- flagi mają wspierać kontrolowane wdrażanie funkcjonalności,
- architektura ma pozwalać na uruchamianie funkcji warunkowo per środowisko, grupa użytkowników lub inny mechanizm segmentacji.

Feature flags są traktowane jako element fundamentalny architektury Continuous Delivery, a nie jako dodatni komponent poboczny.

---

## 11. Observability

Architektura od początku zakłada pełną obserwowalność. Obejmuje to:
- logowanie,
- metryki,
- ślady,
- health checks,
- diagnostykę środowiskową.

Wspólne standardy observability powinny być utrzymywane centralnie, głównie poprzez `Mavrynt.ServiceDefaults` oraz warstwy wspólne.

---

## 12. Testowalność

Rozwiązanie ma być projektowane w sposób ułatwiający:
- testy jednostkowe modułów,
- testy integracyjne hostów i warstw infrastrukturalnych,
- testy architektoniczne zależności,
- testy frontendowe dla aplikacji klienckich.

Testy są traktowane jako równorzędna część repozytorium, a nie jako dodatek wtórny.

---

## 13. Continuous Delivery

Architektura repozytorium i projektów ma wspierać pełne Continuous Delivery. Oznacza to między innymi:
- przewidywalną strukturę buildów,
- powtarzalne uruchamianie środowisk,
- możliwość niezależnej walidacji poszczególnych aplikacji,
- bezpieczne wdrażanie funkcji z użyciem feature flag,
- czytelny podział konfiguracji środowiskowej.

---

## 14. Kierunki dalszego rozwoju

W kolejnych etapach architektura będzie rozszerzana o:
- kolejne moduły domenowe,
- testy architektoniczne,
- wspólną konfigurację pakietów,
- konteneryzację,
- pipeline CI/CD,
- konfigurację danych i migracji,
- bezpieczeństwo i polityki dostępowe,
- mechanizmy powiadomień i komunikacji asynchronicznej.

---

## 15. Podsumowanie

Mavrynt jest budowany jako modularny monolit w jednym repozytorium, z wyraźnym podziałem na hosty, building blocks, moduły domenowe, frontendy i warstwę infrastrukturalną. Głównym celem architektury jest zachowanie porządku, skalowalności organizacyjnej i technicznej oraz gotowości do dalszego rozwoju bez nadmiernego kosztu początkowego.

Dokument ten stanowi bazę referencyjną dla dalszych decyzji implementacyjnych i powinien być aktualizowany wraz z rozwojem rozwiązania.
---

## 12. Strategia testów (fundament backendu)

Strategia testów backendowych opiera się na trzech warstwach:

1. **Testy architektoniczne** (NetArchTest + kontrola referencji projektów) chronią granice modułów i kierunek zależności.
2. **Testy jednostkowe** pokrywają prymitywy domenowe i handlery komend/zapytań Users z użyciem fake/in-memory.
3. **Testy integracyjne** działają na realnym PostgreSQL przez Testcontainers dla repozytoriów infrastruktury oraz smoke testów hostów API/Admin.

Całość uruchamia się przez `dotnet test` i nie wymaga Aspire AppHost ani Docker Compose.
