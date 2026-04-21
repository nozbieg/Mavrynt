# `@mavrynt/auth-ui` — package reference

Shared authentication UI for the Mavrynt SPAs. Owns the login and register forms, the `AuthService` port, the default development adapter, the HTTP adapter factory, and the bilingual (en/pl) auth i18n bundle. See `ADR-017` for the rationale behind keeping this separate from `@mavrynt/ui`.

## What lives here

```
@mavrynt/auth-ui
├── forms/
│   ├── AuthCard.tsx         — card shell (eyebrow + title + subtitle + footer slot)
│   ├── LoginForm.tsx        — presentational form bound to useLoginForm
│   ├── RegisterForm.tsx     — presentational form bound to useRegisterForm
│   ├── PasswordField.tsx    — show/hide toggle, aria-invalid wiring
│   ├── TextField.tsx        — label + input + inline error text
│   ├── useLoginForm.ts      — state machine (idle → submitting → success/error)
│   └── useRegisterForm.ts   — state machine with confirm-password validation
├── service/
│   ├── types.ts             — AuthService, AuthSession, AuthError, AuthErrorCode
│   ├── consoleAuthService.ts — default dev adapter (mock session + test triggers)
│   ├── httpAuthService.ts   — fetch-based adapter for Mavrynt.Api / Mavrynt.AdminApp
│   └── context.ts           — AuthServiceContext + useAuthService hook
├── analytics/
│   ├── types.ts             — AuthAnalyticsPort (track(event, props))
│   └── context.ts           — AuthAnalyticsContext + useAuthAnalytics hook
└── i18n/
    ├── en.json              — auth namespace, English copy
    ├── pl.json              — auth namespace, Polish copy
    └── index.ts             — AUTH_I18N_NAMESPACE + authI18nResources
```

## Consuming the package

### 1. Register the i18n bundle

In the consuming app's i18n bootstrap:

```ts
import { AUTH_I18N_NAMESPACE, authI18nResources } from "@mavrynt/auth-ui/i18n";

await createI18n({
  resources: {
    en: { common: enCommon, [AUTH_I18N_NAMESPACE]: authI18nResources.en },
    pl: { common: plCommon, [AUTH_I18N_NAMESPACE]: authI18nResources.pl },
  },
  ns: ["common", AUTH_I18N_NAMESPACE],
});
```

The forms internally call `useTranslation("auth")`, so the bundle must be registered under namespace `"auth"`.

### 2. Inject the `AuthService`

Wrap the app in `AuthServiceContext.Provider` in `Providers.tsx`. Typical pattern:

```tsx
import {
  AuthServiceContext,
  createConsoleAuthService,
  createHttpAuthService,
} from "@mavrynt/auth-ui";

const authService =
  import.meta.env.VITE_AUTH === "http"
    ? createHttpAuthService({ baseUrl: "/api" })
    : createConsoleAuthService();

<AuthServiceContext.Provider value={authService}>
  {children}
</AuthServiceContext.Provider>;
```

The default (if no provider is mounted) is the console adapter — components always have a working implementation (Liskov), the default just doesn't leave the browser.

### 3. (Optional) inject analytics

```tsx
import { AuthAnalyticsContext } from "@mavrynt/auth-ui";

const authAnalytics = {
  track: (event, props) => myAnalytics.track(event, props),
};

<AuthAnalyticsContext.Provider value={authAnalytics}>…</AuthAnalyticsContext.Provider>;
```

If you skip this, the default `noopAuthAnalytics` is used and the forms still work.

### 4. Render a page

```tsx
import { AuthCard, LoginForm, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui";

const LoginPage = () => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const navigate = useNavigate();

  return (
    <AuthCard
      title={t("login.title")}
      subtitle={t("login.subtitle")}
      footer={<RouterLink to="/register">{t("login.links.register")}</RouterLink>}
    >
      <LoginForm
        source="web:login"
        onSuccess={() => navigate("/", { replace: true })}
      />
    </AuthCard>
  );
};
```

The `source` prop is free-form and flows into analytics props + `LoginCredentials.source` for server-side audit. Suggested convention: `"<app>:<surface>"` (e.g., `"web:login"`, `"admin:login"`).

## Contracts

### `AuthService`

```ts
interface AuthService {
  login(credentials: LoginCredentials): Promise<AuthSession>;
  register(credentials: RegisterCredentials): Promise<AuthSession>;
  logout(): Promise<void>;
}
```

All failures are thrown as `AuthError` with a typed `code`:

```ts
type AuthErrorCode =
  | "network"
  | "validation"
  | "invalid_credentials"
  | "email_taken"
  | "rate_limited"
  | "server";
```

Each code maps 1:1 to an i18n key under `auth.<form>.error.<code>`. Adding a new code is a three-step change: add it to the union, add the copy to both locales, and make sure the adapter surfaces it (the http adapter parses backend error bodies into this union).

### `AuthAnalyticsPort`

```ts
interface AuthAnalyticsPort {
  track(event: AuthAnalyticsEvent, props?: AuthAnalyticsProps): void;
}

type AuthAnalyticsEvent =
  | "auth_login_attempt"    | "auth_login_success"    | "auth_login_error"
  | "auth_register_attempt" | "auth_register_success" | "auth_register_error";
```

## Console adapter test triggers

`createConsoleAuthService` is the default dev / test fallback. Two email triggers let tests exercise the error branches without a backend:

| Email prefix | Method | Result |
| --- | --- | --- |
| `fail+invalid@…` | `login` | `AuthError("invalid_credentials")` |
| `fail+taken@…` | `register` | `AuthError("email_taken")` |

Anything else resolves with a mock session after ~300 ms (configurable via `latencyMs: 0` in tests). Admin instances pass `roles: ["admin"]` so route guards behave realistically against the mock.

## What the package does NOT own

- Routing — every link is rendered as `ReactNode` passed into `AuthCard.footer`, so the forms stay routing-agnostic. Apps pass their own `<RouterLink>`.
- Feature flags — `admin.register.enabled` gating happens in `mavrynt-admin`'s route / nav layer, not inside the form.
- Session storage — the forms return an `AuthSession` to the `onSuccess` callback; persistence (cookie, localStorage, context state) is the consumer app's decision.
- Real HTTP endpoints — `createHttpAuthService` takes `HttpAuthEndpoints` so the exact route shape is negotiated with `Mavrynt.Api` / `Mavrynt.AdminApp`, not baked into the package.

## Testing guidance

The package is source-shipped; tests live inside consuming apps, primarily `mavrynt-web`:

- `resolveAppUrls` env fallback matrix — `src/lib/app-urls/resolveAppUrls.test.ts`
- `useLoginForm` / `useRegisterForm` state machines — `src/test/useLoginForm.test.tsx`, `src/test/useRegisterForm.test.tsx`
- `consoleAuthService` test triggers — `src/test/consoleAuthService.test.ts`
- Playwright smoke against the console adapter — `tests/e2e/auth.spec.ts`

Use `buildAuthHarness()` from `src/test/authHarness.tsx` to wire a provider tree with a real i18n instance and `vi.fn()`-backed `AuthService` / analytics stubs.
