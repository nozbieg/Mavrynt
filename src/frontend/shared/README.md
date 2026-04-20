# Mavrynt — Shared Frontend Workspace

This folder hosts the shared frontend layer consumed by all three SPAs in `src/frontend/`:

- `mavrynt-web` (user-facing SPA)
- `mavrynt-admin` (admin SPA)
- `mavrynt-landing` (marketing landing SPA)

## Packages

| Package | Purpose |
| --- | --- |
| [`@mavrynt/design-tokens`](./design-tokens) | Design tokens (colors, spacing, typography, radii, breakpoints). Exposes both TypeScript exports and a CSS file with Tailwind v4 `@theme` and semantic theme variables (light / dark). |
| [`@mavrynt/ui`](./ui) | Shared headless / styled UI primitives: `Container`, `Section`, `Stack`, `Button`, `Link`, `Logo`, plus layout (`Navbar`, `Footer`) and `ThemeProvider`. |
| [`@mavrynt/auth-ui`](./auth-ui) | Shared authentication UI: `LoginForm`, `RegisterForm`, `AuthCard`, `AuthService` port (console + http adapters), bilingual i18n resources. Used by `mavrynt-web` and `mavrynt-admin`; deliberately kept separate from `@mavrynt/ui` because it owns domain semantics (sessions, credentials). |
| [`@mavrynt/config`](./config) | Cross-app runtime concerns: typed env loader, feature-flag client interface, i18n bootstrap factory. |
| [`@mavrynt/eslint-config`](./eslint-config) | Shared ESLint flat config for all SPAs. |
| [`@mavrynt/tsconfig-base`](./tsconfig-base) | Base `tsconfig` files (`base.json`, `react.json`) extended by every SPA. |

## Architectural rules (SOLID / DRY)

1. **Apps depend on `shared`, never the other way around.** Shared packages must not import from any `mavrynt-*` app.
2. **Single Responsibility per package.** Tokens hold values, `ui` holds components, `config` holds runtime infrastructure. Do not mix.
3. **Open/Closed via composition.** Primitives in `ui` accept `className`/`asChild`/render-prop patterns where useful, so apps extend without forking.
4. **Dependency Inversion for runtime concerns.** `feature-flags` and `i18n` expose interfaces; apps inject implementations.
5. **No backend coupling.** `mavrynt-landing` consumes shared packages only — never API clients tied to the backend domain (ADR-010).

## How packages are consumed

All packages are workspace-linked (npm workspaces). Apps import them by name:

```ts
import { Button, Navbar } from "@mavrynt/ui";
import { tokens } from "@mavrynt/design-tokens";
import { loadEnv, createI18n } from "@mavrynt/config";
```

CSS is imported by path:

```css
@import "tailwindcss";
@import "@mavrynt/design-tokens/styles/reset.css";
@import "@mavrynt/design-tokens/styles/tokens.css";
```

## Build model

Packages are **source-shipped** — `exports` point to `.ts` / `.tsx` files. Vite (with `@vitejs/plugin-react`) compiles them on demand in each consuming app, so there is no separate build step in `shared/*`. This keeps the dev loop fast and avoids stale `dist/` artifacts.

## Local commands

Run from the repository root (where the workspace `package.json` lives):

```bash
npm install          # installs all workspaces
npm run dev:landing  # start the marketing SPA
npm run dev:web      # start the user SPA
npm run dev:admin    # start the admin SPA
npm run lint         # lint every workspace
npm run typecheck    # typecheck every workspace
npm run build        # production build of every SPA
```
