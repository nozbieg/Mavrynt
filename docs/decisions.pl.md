# Mavrynt — Dziennik decyzji (skrót PL)

> **Wersja kanoniczna:** [`decisions.en.md`](decisions.en.md). Pełna treść każdej
> decyzji (ADR) — kontekst, decyzja, uzasadnienie, konsekwencje — jest utrzymywana
> w języku angielskim. Nowe ADR-y dopisuj **wyłącznie** do `decisions.en.md`.

## Indeks (skrót)

| ID | Tytuł | Status |
|---|---|---|
| ADR-001 | Modularny monolit | Zaakceptowana |
| ADR-002 | Jedno repozytorium dla całego produktu | Zaakceptowana |
| ADR-003 | Podział hostów backendu | Zaakceptowana |
| ADR-004 | Wydzielony obszar administracyjny | Zaakceptowana |
| ADR-005 | Warstwowa struktura modułu (Domain / Application / Infrastructure) | Zaakceptowana |
| ADR-006 | Wspólne projekty BuildingBlocks | Zaakceptowana |
| ADR-007 | Pierwszy moduł fundamentu: Users | Zaakceptowana |
| ADR-008 | Feature flags jako zdolność architektoniczna | Zaakceptowana |
| ADR-009 | Pełna obserwowalność od początku | Zaakceptowana |
| ADR-010 | Frontendy w jednym repo, oddzielone od backendu | Zaakceptowana |
| ADR-011 | PostgreSQL jako podstawowa baza | Zaakceptowana |
| ADR-012 | Redis / RabbitMQ / Kafka zarezerwowane w architekturze | Zaakceptowana |
| ADR-013 | Ręczne tworzenie solution z czystego `.sln` | Zaakceptowana |
| ADR-014 | Dokumentacja architektury w repo | Zaakceptowana |
| ADR-015 | Landing marketingowy: niezależny cykl, wspólne narzędzia | Zaakceptowana |
| ADR-016 | Resolver URL między aplikacjami: `@mavrynt/config/app-urls` | Zaakceptowana |
| ADR-017 | `@mavrynt/auth-ui` jako osobny pakiet | Zaakceptowana |
| ADR-018 | BuildingBlocks bez przywiązania do mediatora | Zaakceptowana (zastąpiona w zakresie wyboru mediatora przez ADR-020) |
| ADR-019 | Moduł Users — Domain + Application baseline | Zaakceptowana |
| ADR-020 | Wewnętrzny mediator i pipeline aplikacji | Zaakceptowana — `MavryntMediator`, zastępuje odroczenie z ADR-018 |
| ADR-021 | Strategia testów backendu (architecture / unit / Testcontainers) | Zaakceptowana |
| ADR-022 | Slice administracyjny Fazy 1: role, FeatureManagement, Audit | Zaakceptowana |
| ADR-023 | Moduł Notifications: SMTP w bazie, szablony, `IEmailNotificationService` | Zaakceptowana |

Relacja **ADR-018 ↔ ADR-020:** ADR-018 świadomie odroczył wybór mediatora i
udostępnił neutralne biblioteki interfejsy komend/zapytań. ADR-020 później
zaakceptował **wewnętrzny `MavryntMediator`** — zmieniła się tylko decyzja o
mechanizmie dispatchu, neutralne kontrakty z ADR-018 pozostają obowiązujące.

Folder `docs/adr/` zawiera szczegółowe notatki implementacyjne, których numeracja
**koliduje** z ADR-020..023 z tego pliku — kanonem jest ten plik (i `decisions.en.md`).
Renumeracja `docs/adr/*.md` jest wpisana do [`next-work.md`](next-work.md).

Pełna treść każdego ADR-a: [`decisions.en.md`](decisions.en.md).
