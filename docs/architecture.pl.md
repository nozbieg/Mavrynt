# Mavrynt — Architektura (skrót PL)

> **Wersja kanoniczna:** [`architecture.en.md`](architecture.en.md). Pełna treść
> jest utrzymywana w języku angielskim. Ten plik jest krótkim skrótem dla osób
> preferujących polski.

## Skrót

- **Styl architektoniczny:** modularny monolit w jednym repozytorium (ADR-001).
- **Hosty backendu:** `Mavrynt.Api`, `Mavrynt.AdminApp`, `Mavrynt.AppHost`,
  `Mavrynt.ServiceDefaults`. Hosty komponują moduły i nie zawierają logiki biznesowej.
- **Każdy moduł:** `Domain` ← `Application` ← `Infrastructure`. Domena nie zna
  Infrastruktury ani hosta. Aplikacja nie zna hosta.
- **Mediator:** wewnętrzny `MavryntMediator` (ADR-020). MediatR jest zabronione.
- **Persistencja:** EF Core 9 + PostgreSQL. Każdy moduł ma własny schemat.
- **Auth:** JWT Bearer (HS256), polityka `AdminOnly` dla endpointów admina.
- **Frontendy:** trzy SPA (`mavrynt-web`, `mavrynt-admin`, `mavrynt-landing`),
  współdzielone pakiety `@mavrynt/*`. Landing jest świadomie odcięty od backendu (ADR-015).
- **Testy:** trzy warstwy — architektoniczne (NetArchTest), jednostkowe,
  integracyjne na realnym PostgreSQL przez Testcontainers (ADR-021).

Szczegóły, zasady zależności i opis każdego modułu: [`architecture.en.md`](architecture.en.md).
Aktualny stan i priorytety: [`status.md`](status.md), [`next-work.md`](next-work.md).
