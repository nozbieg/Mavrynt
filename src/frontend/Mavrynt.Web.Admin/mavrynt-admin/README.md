# mavrynt-admin

Internal operator console for Mavrynt. Paired with `Mavrynt.AdminApp` on the backend. Self-registration is disabled on this surface by default — operators are invited in through the Users module (see `ADR-007` / the `admin.register.enabled` feature flag).

Runs on Vite dev port **5175**.

## Scripts

```bash
npm run dev            # Vite dev server on http://localhost:5175
npm run build          # tsc -b && vite build
npm run lint           # ESLint
npm run typecheck      # tsc -b --noEmit
npm run preview        # serve the production bundle

npm run test           # Vitest unit / integration (jsdom)
npm run test:watch     # Vitest watch mode
npm run test:cov       # Vitest with V8 coverage
```

No Playwright suite yet — added when admin routes carry real state worth smoke-testing.

## Screens

All routes below are protected by `RequireAdminAuth` and redirect to `/login` when unauthenticated.

| Route | Screen | Description |
| --- | --- | --- |
| `/dashboard` | Dashboard | Profile overview, system status, quick action links |
| `/users` | Users | Role assignment by user ID (`PATCH /api/admin/users/{id}/role`). No list endpoint exists in the backend yet — the screen documents this limitation clearly. |
| `/feature-flags` | Feature Flags | List, create, edit, toggle feature flags |
| `/smtp-settings` | SMTP Settings | List, create, edit, activate SMTP configurations. Password is never displayed in the edit form. |
| `/settings` | Settings | Session info, links to other sections, environment details |

### Backend endpoints

| Screen | Endpoint | Notes |
| --- | --- | --- |
| Users | `PATCH /api/admin/users/{userId}/role` | Only operation available — no list endpoint |
| Feature Flags | `GET /api/admin/feature-flags/` | — |
| Feature Flags | `POST /api/admin/feature-flags/` | key, name, description?, isEnabled |
| Feature Flags | `PATCH /api/admin/feature-flags/{key}` | name, description? |
| Feature Flags | `PATCH /api/admin/feature-flags/{key}/toggle` | — |
| SMTP Settings | `GET /api/admin/notifications/smtp-settings/` | — |
| SMTP Settings | `POST /api/admin/notifications/smtp-settings/` | password required |
| SMTP Settings | `PATCH /api/admin/notifications/smtp-settings/{id}` | password optional (omit = keep current) |
| SMTP Settings | `PATCH /api/admin/notifications/smtp-settings/{id}/enable` | — |

### Form validation

- **Feature flag key**: required, lowercase letters/numbers/dots/hyphens/underscores, max 100 chars
- **Feature flag name**: required, max 200 chars
- **Feature flag description**: optional, max 500 chars
- **SMTP port**: required, integer 1–65535
- **SMTP senderEmail**: required, valid email format
- **SMTP password**: required on create, optional on edit (empty = unchanged)

### Out of scope (separate tasks)

- Test email send (`POST /api/admin/notifications/email/test-send`)
- First-login forced password change
- Password reset
- User listing (no backend endpoint)

## Architecture snapshot

```
src/
├── app/            Providers composition root + router
├── pages/          Route-level components
├── layouts/        Console shell (AdminNav, AdminFooter, AdminLayout)
├── components/     Shared admin components (AdminCard, AdminPageHeader, AdminState, AdminTable)
├── lib/            Per-app concerns (analytics, auth, feature-flags, i18n, router, seo)
├── test/           Vitest setup
└── main.tsx        Bootstrap
```

Cross-cutting ports are the same as `mavrynt-web`; the admin adapter for `AuthService` is configured with `roles: ["admin"]` so route guards behave realistically against the console mock.

## Auth

`LoginPage` composes `@mavrynt/auth-ui` identically to the web SPA, with `source="admin:login"`. The `/register` route exists but is feature-flagged off by default:

- `featureFlags.isEnabled("admin.register.enabled")` returns `false` in the shipped static adapter.
- `AdminNav` hides the Register link entirely when the flag is off so operators aren't led to a dead-end.
- The route still mounts (defensive) but renders the `register.disabled` copy from `@mavrynt/auth-ui`.

Toggle the flag from the feature-flag client once the Users module provides a real backend.

## Env

| Variable | Purpose | Default |
| --- | --- | --- |
| `VITE_APP_URL_LANDING` | Absolute URL of the marketing site | `http://localhost:5173` |
| `VITE_APP_URL_WEB` | Absolute URL of the user SPA | `http://localhost:5174` |
| `VITE_APP_URL_ADMIN` | Absolute URL of this SPA | `http://localhost:5175` |
| `VITE_ADMIN_API_PROXY_TARGET` | Upstream for `/api/*` (Mavrynt.AdminApp) | `http://localhost:5001` |

Legacy aliases (`VITE_MARKETING_URL`, `VITE_WEB_URL`, `VITE_ADMIN_URL`) are still honoured — see `ADR-016`.
