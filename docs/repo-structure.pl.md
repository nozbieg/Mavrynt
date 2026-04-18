# Mavrynt — Struktura repozytorium

## 1. Cel dokumentu

Celem dokumentu jest opisanie docelowej i aktualnej struktury repozytorium Mavrynt. Dokument definiuje:
- przeznaczenie głównych katalogów,
- odpowiedzialność poszczególnych projektów,
- zasady organizacji plików i kodu,
- kierunek dalszej rozbudowy repozytorium.

Repozytorium ma być czytelne dla człowieka, przewidywalne dla agentów AI i wygodne w utrzymaniu w długim okresie.

---

## 2. Główne założenia organizacyjne

Repozytorium:
- obejmuje cały produkt,
- zawiera backend, frontend, dokumentację, testy i wdrożenia,
- unika mieszania odpowiedzialności,
- utrzymuje spójne nazewnictwo,
- wspiera rozwój modularnego monolitu.

Zasada podstawowa: **każdy katalog ma czytelny cel i powinien zawierać tylko to, co jest zgodne z jego odpowiedzialnością.**

---

## 3. Struktura główna repozytorium

Aktualna docelowa struktura repozytorium:

```text
Mavrynt/
├── Mavrynt.sln
├── README.md
├── .gitignore
├── .gitattributes
├── Directory.Build.props
├── Directory.Packages.props
├── docs/
├── build/
├── deploy/
├── scripts/
├── src/
│   ├── backend/
│   ├── frontend/
│   └── shared/
└── tests/