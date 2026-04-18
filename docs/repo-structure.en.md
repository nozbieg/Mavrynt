# Mavrynt — Repository Structure

## 1. Purpose of this document

The purpose of this document is to describe the target and current structure of the Mavrynt repository. It defines:
- the purpose of the main folders,
- responsibilities of the individual projects,
- rules for organizing files and code,
- the direction of future repository expansion.

The repository should be readable for humans, predictable for AI agents, and maintainable over the long term.

---

## 2. Main organizational assumptions

The repository:
- contains the whole product,
- includes backend, frontend, documentation, tests, and deployment assets,
- avoids mixing responsibilities,
- follows consistent naming,
- supports modular monolith development.

The basic rule is: **every folder should have a clear purpose and should contain only what belongs to that responsibility.**

---

## 3. Top-level repository structure

The current target repository structure is:

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