# mavrynt-web

User-facing single-page application for the Mavrynt workspace. Login, registration, and the product surfaces live here. Pairs with `Mavrynt.Api` on the backend.

Runs on Vite dev port **5174** (see `ADR-016` / `docs/frontends.{en,pl}.md` for the full port + env matrix).

## Scripts

```bash
npm run dev            # Vite dev server on http://localhost:5174
npm run build          # tsc -b && vite build
npm run lint           # ESLint
npm run typecheck      # tsc -b --noEmit
npm run preview        # serve the production bundle

npm run test           # Vitest unit / integration (jsdom)
npm run test:watch     # Vitest watch mode
npm run test:cov       # Vitest with V8 coverage

npm run test:e2e       # Playwright smoke (Chromium, boots `npm run dev`)
npm run test:e2e:ui    # Playwright UI mode
npm run test:e2e:install # one-time Playwright browser install
```

## Architecture snapshot

```
src/
├── app/            Providers composition root + router
├── pages/          Route-level components (Home, Login, Register, NotFound)
├── layouts/        App shell (AppNav, AppFooter, AppLayout)
├── lib/            Per-app concerns (analytics, auth, i18n, router, seo, app-urls)
├── test/           Vitest harness (authHarness.tsx + setup.ts)
└── main.tsx        Bootstrap
```

Cross-cutting behaviour lives behind ports:

- `AuthService` — `@mavrynt/auth-ui` port; default adapter is `createConsoleAuthService()` in dev, swapped for `createHttpAuthService()` via `Providers.tsx` once the Users module lands.
- `AnalyticsClient` — per-app analytics port (console adapter in dev).
- `FeatureFlagClient` — `@mavrynt/config` port (static map today, remote adapter later per `ADR-008`).
- `resolveAppUrls()` — cross-app URL resolver from `@mavrynt/config/app-urls`; used by nav and footer to link to the landing site and admin console.

## Auth

Login and register pages compose `@mavrynt/auth-ui`:

- `LoginPage` renders `<AuthCard><LoginForm source="web:login" /></AuthCard>` and redirects to `/` on success.
- `RegisterPage` renders `<AuthCard><RegisterForm source="web:register" /></AuthCard>` and redirects to `/login` on success.

The auth i18n bundle is registered in `src/lib/i18n/i18n.ts` under namespace `"auth"`. Test triggers for the console adapter (`fail+invalid@…`, `fail+taken@…`) are documented in `docs/auth-ui.{en,pl}.md`.

## Tests

| Target | Path |
| --- | --- |
| `resolveAppUrls` env matrix | `src/lib/app-urls/resolveAppUrls.test.ts` |
| `useLoginForm` state machine | `src/test/useLoginForm.test.tsx` |
| `useRegisterForm` state machine | `src/test/useRegisterForm.test.tsx` |
| `consoleAuthService` triggers | `src/test/consoleAuthService.test.ts` |
| Playwright smoke (login + register) | `tests/e2e/auth.spec.ts` |

The Vitest config reuses `vite.config.ts` via `mergeConfig` so modules resolve identically to the runtime. The Playwright suite is excluded from the unit include glob and runs against the dev server with the default (console) auth adapter.

## Env

| Variable | Purpose | Default |
| --- | --- | --- |
| `VITE_APP_URL_LANDING` | Absolute URL of the marketing site | `http://localhost:5173` |
| `VITE_APP_URL_WEB` | Absolute URL of this SPA | `http://localhost:5174` |
| `VITE_APP_URL_ADMIN` | Absolute URL of the admin console | `http://localhost:5175` |
| `VITE_API_PROXY_TARGET` | Upstream for `/api/*` (Mavrynt.Api) | `http://localhost:5000` |

Legacy aliases (`VITE_MARKETING_URL`, `VITE_WEB_URL`, `VITE_ADMIN_URL`) are still honoured — see `ADR-016`.
