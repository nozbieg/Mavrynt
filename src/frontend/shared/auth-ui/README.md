# @mavrynt/auth-ui

Shared authentication UI for all Mavrynt SPAs (`mavrynt-web`, `mavrynt-admin`). Built on top of `@mavrynt/ui` + `@mavrynt/design-tokens`.

Kept **separate** from `@mavrynt/ui` because it owns domain semantics (sessions, credentials, error codes, i18n keys), while `@mavrynt/ui` stays purely presentational (Container, Button, Stack…). See `ADR-017`. For usage docs see `docs/auth-ui.{en,pl}.md`.

## Public surface

```ts
import {
  // forms (presentational)
  AuthCard,
  LoginForm,
  RegisterForm,
  TextField,
  PasswordField,
  // form hooks (state machines)
  useLoginForm,
  useRegisterForm,
  // service port + adapters
  AuthError,
  AuthServiceContext,
  useAuthService,
  createConsoleAuthService,
  createHttpAuthService,
  // analytics port (optional)
  AuthAnalyticsContext,
  useAuthAnalytics,
  noopAuthAnalytics,
  // i18n resources
  AUTH_I18N_NAMESPACE,
  authI18nResources,
  // types
  type AuthService,
  type AuthSession,
  type AuthUser,
  type LoginCredentials,
  type RegisterCredentials,
  type AuthErrorCode,
} from "@mavrynt/auth-ui";
```

## Architectural rules

1. **Backend-agnostic.** Forms only depend on the `AuthService` interface — never a concrete adapter. The default in context is `createConsoleAuthService()` so the package works end-to-end with no backend wired up. Apps swap in `createHttpAuthService({ endpoints })` in their `Providers`.
2. **Routing-agnostic.** Forms accept a `secondaryAction: ReactNode` slot for "Sign in" / "Register" / "Back to mavrynt.com" links. Each app passes its own router-aware components — the package never imports from `react-router`.
3. **i18n-namespace-isolated.** All keys live under namespace `"auth"` (constant `AUTH_I18N_NAMESPACE`). Apps register the bundle once at i18n bootstrap; components call `useTranslation("auth")` internally.
4. **Stateless first.** Only the form hooks own state (`values`, `errors`, `status`). All visual components are pure.
5. **Tokens over magic numbers.** Every spacing / color value resolves to a design token. If you need a new value, add it to `@mavrynt/design-tokens` first.

## Wiring it up

### 1. Register i18n resources

```ts
// src/lib/i18n/i18n.ts (consuming app)
import { authI18nResources, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui/i18n";

export const i18n = createI18n({
  resources: {
    en: { common: enCommon, [AUTH_I18N_NAMESPACE]: authI18nResources.en },
    pl: { common: plCommon, [AUTH_I18N_NAMESPACE]: authI18nResources.pl },
  },
  // …
});
```

### 2. Provide an AuthService

```tsx
// src/app/Providers.tsx
import {
  AuthServiceContext,
  createConsoleAuthService,
  createHttpAuthService,
} from "@mavrynt/auth-ui";

const authService =
  import.meta.env.VITE_AUTH === "http"
    ? createHttpAuthService({
        endpoints: {
          login: "/api/auth/login",
          register: "/api/auth/register",
          logout: "/api/auth/logout",
        },
      })
    : createConsoleAuthService();

export const Providers = ({ children }) => (
  <AuthServiceContext.Provider value={authService}>
    {children}
  </AuthServiceContext.Provider>
);
```

### 3. Render the forms

```tsx
// src/pages/LoginPage.tsx
import { AuthCard, LoginForm } from "@mavrynt/auth-ui";
import { useTranslation } from "react-i18next";
import { RouterLink } from "../lib/router/RouterLink";

export const LoginPage = () => {
  const { t } = useTranslation("auth");
  return (
    <AuthCard title={t("login.title")} subtitle={t("login.subtitle")}>
      <LoginForm
        source="web:login"
        secondaryAction={
          <>
            <RouterLink to="/register">{t("login.links.register")}</RouterLink>
            <a href={import.meta.env.VITE_LANDING_ORIGIN}>
              {t("login.links.backToSite")}
            </a>
          </>
        }
        onSuccess={() => {
          // navigate("/dashboard")
        }}
      />
    </AuthCard>
  );
};
```

### Mock failures (for demos / tests)

The console adapter triggers deterministic error paths based on email prefix:

| Email prefix         | Error code            |
| -------------------- | --------------------- |
| `fail+invalid@…`     | `invalid_credentials` |
| `fail+taken@…`       | `email_taken`         |

## Tailwind v4 — class scanning

Each consuming app must include `@source` for this package alongside `@mavrynt/ui`:

```css
@import "tailwindcss";
@import "@mavrynt/design-tokens/styles/tokens.css";
@import "@mavrynt/design-tokens/styles/reset.css";

@source "../../shared/ui/src/**/*.{ts,tsx}";
@source "../../shared/auth-ui/src/**/*.{ts,tsx}";
```

## Backend contract

Adapter `createHttpAuthService` expects:

| Method | Endpoint (default)       | Request body                               | Success status | Success body                   |
| ------ | ------------------------ | ------------------------------------------ | -------------- | ------------------------------ |
| POST   | `/api/auth/login`        | `LoginCredentials`                         | `200`          | `AuthSession` (JSON)           |
| POST   | `/api/auth/register`     | `RegisterCredentials`                      | `200` / `201`  | `AuthSession` (JSON)           |
| POST   | `/api/auth/logout`       | _empty_                                    | `200` / `204`  | _ignored_                      |

Failure → `AuthError` with `code`:

| HTTP status        | `AuthErrorCode`        |
| ------------------ | ---------------------- |
| `401`, `403`       | `invalid_credentials`  |
| `409`              | `email_taken`          |
| `400`, `422`       | `validation`           |
| `429`              | `rate_limited`         |
| network failure    | `network`              |
| any other          | `server`               |

The Mavrynt Users module endpoints aren't yet implemented — Phase 2/3 use the console adapter; Phase 6 (or later) wires the HTTP adapter once the backend contract is live.
