# Mavrynt — Struktura repozytorium (skrót PL)

> **Wersja kanoniczna:** [`repo-structure.en.md`](repo-structure.en.md). Pełna
> mapa repozytorium jest utrzymywana w języku angielskim. Ten plik to krótki
> skrót dla osób preferujących polski.

## Skrót

```
Mavrynt/
├── AGENTS.md, README.md, Mavrynt.sln, Directory.{Build,Packages}.props
├── docs/                       dokumentacja (architektura, decyzje, status, next-work)
├── build/, deploy/, scripts/   zarezerwowane (puste)
├── src/
│   ├── backend/                modularny monolit
│   │   ├── Mavrynt.Api / Mavrynt.AdminApp / Mavrynt.AppHost / Mavrynt.ServiceDefaults
│   │   ├── Mavrynt.BuildingBlocks.{Domain,Application,Infrastructure} + BuildingBlocksContracts
│   │   └── Mavrynt.Modules.{Users,FeatureManagement,Audit,Notifications}.{Domain,Application,Infrastructure}
│   └── frontend/
│       ├── Mavrynt.Web.App/      wrapper Aspire wokół mavrynt-web/
│       ├── Mavrynt.Web.Admin/    wrapper Aspire wokół mavrynt-admin/
│       ├── Mavrynt.Web.Landing/  wrapper Aspire wokół mavrynt-landing/
│       └── shared/               @mavrynt/{auth-ui,config,design-tokens,eslint-config,tsconfig-base,ui}
└── tests/                       architecture, unit, integration (Testcontainers)
```

## Zasady (krótko)

- Hosty nie zawierają logiki biznesowej.
- Domena nie zna Infrastruktury, frameworka, hosta.
- Każdy nowy moduł = trzy projekty (Domain / Application / Infrastructure) +
  trzy projekty testowe.
- Frontend nie referuje projektów backendowych — wyłącznie HTTP/porty.
- Nowe decyzje architektoniczne: dopisuj nowy ADR do
  [`decisions.en.md`](decisions.en.md). Nie edytuj historycznych ADR-ów.

Pełna treść (hosty, BuildingBlocks, moduły, testy, zasady dodawania nowych
elementów): [`repo-structure.en.md`](repo-structure.en.md).
