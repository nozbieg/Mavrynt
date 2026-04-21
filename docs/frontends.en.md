# Mavrynt — Frontend topology

This document describes how the three Mavrynt single-page applications relate to each other, how they are hosted in development, how they talk to the backend, and how environment variables are resolved. It is the operational companion to `ADR-010` (frontends separated from backend), `ADR-015` (landing independence), and `ADR-016` (cross-app URL resolution).

## The three SPAs

| SPA | Package | Role | Backend host | Default dev URL |
| --- | --- | --- | --- | --- |
| Marketing landing | `mavrynt-landing` | Public front door; marketing, contact, legal pages | None (backend-independent — see `ADR-010`) | `http://localhost:5173` |
| Web application | `mavrynt-web` | User-facing workspace — login, register, product surfaces | `Mavrynt.Api` | `http://localhost:5174` |
| Admin console | `mavrynt-admin` | Internal operator console | `Mavrynt.AdminApp` | `http://localhost:5175` |

The landing deliberately has no auth surface. Both `/login` and `/register` CTAs on the marketing site are external links into `mavrynt-web`, resolved through `@mavrynt/config/app-urls`. Reverse direction: `mavrynt-web` and `mavrynt-admin` link back to the landing via the same resolver (footer "back to mavrynt.com" + nav escape-hatches).

## Cross-app URL resolution

All cross-SPA links resolve through a single helper exported from `@mavrynt/config`:

```ts
import { resolveAppUrls } from "@mavrynt/config/app-urls";

const appUrls = resolveAppUrls();
// appUrls.landing, appUrls.web, appUrls.admin
```

### Env matrix

Canonical keys (preferred, set per deployment):

| Variable | Applies to | Example |
| --- | --- | --- |
| `VITE_APP_URL_LANDING` | Marketing site | `https://mavrynt.com` |
| `VITE_APP_URL_WEB` | User SPA | `https://app.mavrynt.com` |
| `VITE_APP_URL_ADMIN` | Admin console | `https://admin.mavrynt.com` |

Legacy aliases (still honoured; retire when possible):

| Legacy variable | Maps to |
| --- | --- |
| `VITE_MARKETING_URL` | `landing` |
| `VITE_WEB_URL` | `web` |
| `VITE_ADMIN_URL` | `admin` |

When no variable is set, `resolveAppUrls()` falls back to the per-app dev ports listed above. Trailing slashes are normalised so callers can always do `` `${appUrls.web}/login` `` safely. The returned object is frozen — do not mutate it.

## Vite dev proxies

`mavrynt-web` and `mavrynt-admin` proxy `/api/*` to their respective backend hosts during development so the SPAs can call relative URLs (`fetch("/api/auth/login")`) without CORS or env wiring. The proxy is defined once per app in `vite.config.ts` and reused between `server` and `preview`.

| SPA | Upstream | Default target | Env override |
| --- | --- | --- | --- |
| `mavrynt-web` | `/api/*` | `http://localhost:5000` | `VITE_API_PROXY_TARGET` |
| `mavrynt-admin` | `/api/*` | `http://localhost:5001` | `VITE_ADMIN_API_PROXY_TARGET` |

`mavrynt-landing` has no backend proxy — it stays backend-independent (`ADR-010` / `ADR-015`). Any data it needs (e.g., lead submission) goes through a port (`LeadService`) whose default adapter is the console logger in dev, and is swapped for an HTTP adapter in prod via `Providers.tsx`.

## Test pyramid per SPA

| SPA | Unit / integration | E2E smoke |
| --- | --- | --- |
| `mavrynt-landing` | Vitest + jsdom + Testing Library | Playwright (Chromium) — home/contact, pricing/FAQ, language switch |
| `mavrynt-web` | Vitest + jsdom + Testing Library | Playwright (Chromium) — login + register (console adapter) |
| `mavrynt-admin` | Vitest + jsdom + Testing Library | Not yet — add when routes carry real state |

Every SPA runs its own unit tests via `npm run test`. Playwright suites live under `tests/e2e/` and are excluded from the Vitest include glob so unit runs stay hermetic.

## Adding a fourth SPA (checklist)

1. Scaffold it under `src/frontend/<name>/`, mirroring `mavrynt-web` for structure.
2. Add it to the root `npm` workspaces array.
3. Register a new entry in `AppId` inside `@mavrynt/config/src/app-urls.ts`, extend `DEFAULT_APP_URLS`, `PRIMARY_ENV_KEYS`, and `LEGACY_ENV_KEYS`.
4. Update this document and `ADR-016`.
5. Add its dev port to the table above.
6. Register it with `Mavrynt.AppHost` for coherent local orchestration.

Keeping this checklist satisfied is cheaper than discovering later that a URL is hard-coded somewhere.
