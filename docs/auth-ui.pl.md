# `@mavrynt/auth-ui` — dokumentacja pakietu

Wspólne UI autoryzacji dla SPA Mavrynt. Posiada formularze logowania i rejestracji, port `AuthService`, domyślny adapter deweloperski, fabrykę adaptera HTTP oraz dwujęzyczną (en/pl) paczkę i18n dla namespace'u auth. Uzasadnienie wydzielenia z `@mavrynt/ui` — patrz `ADR-017`.

## Co znajduje się w pakiecie

```
@mavrynt/auth-ui
├── forms/
│   ├── AuthCard.tsx         — karta (eyebrow + tytuł + podtytuł + slot stopki)
│   ├── LoginForm.tsx        — prezentacyjny formularz związany z useLoginForm
│   ├── RegisterForm.tsx     — prezentacyjny formularz związany z useRegisterForm
│   ├── PasswordField.tsx    — toggle pokaż/ukryj, aria-invalid
│   ├── TextField.tsx        — label + input + tekst błędu inline
│   ├── useLoginForm.ts      — maszyna stanów (idle → submitting → success/error)
│   └── useRegisterForm.ts   — maszyna stanów z walidacją confirm-password
├── service/
│   ├── types.ts             — AuthService, AuthSession, AuthError, AuthErrorCode
│   ├── consoleAuthService.ts — domyślny adapter dev (mock sesji + test triggery)
│   ├── httpAuthService.ts   — adapter fetch dla Mavrynt.Api / Mavrynt.AdminApp
│   └── context.ts           — AuthServiceContext + hook useAuthService
├── analytics/
│   ├── types.ts             — AuthAnalyticsPort (track(event, props))
│   └── context.ts           — AuthAnalyticsContext + hook useAuthAnalytics
└── i18n/
    ├── en.json              — namespace auth, copy angielski
    ├── pl.json              — namespace auth, copy polski
    └── index.ts             — AUTH_I18N_NAMESPACE + authI18nResources
```

## Korzystanie z pakietu

### 1. Rejestracja paczki i18n

W bootstrapie i18n konsumującej aplikacji:

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

Formularze wewnętrznie wołają `useTranslation("auth")`, więc paczka musi być zarejestrowana pod namespace'em `"auth"`.

### 2. Wstrzyknięcie `AuthService`

Owiń aplikację providerem `AuthServiceContext.Provider` w `Providers.tsx`. Typowy wzorzec:

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

Domyślna wartość (gdy provider nie jest podpięty) to adapter konsolowy — komponenty zawsze mają działającą implementację (Liskov), domyślna po prostu nie wychodzi z przeglądarki.

### 3. (Opcjonalnie) wstrzyknięcie analityki

```tsx
import { AuthAnalyticsContext } from "@mavrynt/auth-ui";

const authAnalytics = {
  track: (event, props) => myAnalytics.track(event, props),
};

<AuthAnalyticsContext.Provider value={authAnalytics}>…</AuthAnalyticsContext.Provider>;
```

Jeśli to pominiesz, używany jest domyślny `noopAuthAnalytics`, a formularze działają tak samo.

### 4. Renderowanie strony

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

Prop `source` jest dowolny i trafia zarówno do propsów analityki, jak i do `LoginCredentials.source` (audyt serwerowy). Sugerowana konwencja: `"<app>:<powierzchnia>"` (np. `"web:login"`, `"admin:login"`).

## Kontrakty

### `AuthService`

```ts
interface AuthService {
  login(credentials: LoginCredentials): Promise<AuthSession>;
  register(credentials: RegisterCredentials): Promise<AuthSession>;
  logout(): Promise<void>;
}
```

Wszystkie błędy są rzucane jako `AuthError` z typowanym `code`:

```ts
type AuthErrorCode =
  | "network"
  | "validation"
  | "invalid_credentials"
  | "email_taken"
  | "rate_limited"
  | "server";
```

Każdy kod mapuje się 1:1 na klucz i18n pod `auth.<form>.error.<code>`. Dodanie nowego kodu to trzy kroki: dopisanie do uniona, dopisanie copy w obu locale oraz upewnienie się, że adapter go surface'uje (adapter http parsuje body błędów z backendu do tego uniona).

### `AuthAnalyticsPort`

```ts
interface AuthAnalyticsPort {
  track(event: AuthAnalyticsEvent, props?: AuthAnalyticsProps): void;
}

type AuthAnalyticsEvent =
  | "auth_login_attempt"    | "auth_login_success"    | "auth_login_error"
  | "auth_register_attempt" | "auth_register_success" | "auth_register_error";
```

## Triggery testowe adaptera konsolowego

`createConsoleAuthService` to domyślny fallback dev / test. Dwa triggery na email pozwalają testom wywołać gałęzie błędów bez backendu:

| Prefiks emaila | Metoda | Rezultat |
| --- | --- | --- |
| `fail+invalid@…` | `login` | `AuthError("invalid_credentials")` |
| `fail+taken@…` | `register` | `AuthError("email_taken")` |

Cokolwiek innego rozwiązuje się mockową sesją po ~300 ms (konfigurowalne przez `latencyMs: 0` w testach). Instancje dla admina przekazują `roles: ["admin"]`, dzięki czemu route guardy zachowują się realistycznie wobec mocka.

## Czego pakiet NIE posiada

- Routingu — każdy link jest przekazywany jako `ReactNode` do `AuthCard.footer`, dzięki czemu formularze pozostają routing-agnostic. Aplikacje podstawiają własny `<RouterLink>`.
- Flag funkcyjnych — bramkowanie `admin.register.enabled` dzieje się w warstwie tras / navu `mavrynt-admin`, a nie wewnątrz formularza.
- Przechowywania sesji — formularze zwracają `AuthSession` w callbacku `onSuccess`; persystencja (cookie, localStorage, stan kontekstu) jest decyzją konsumenta.
- Realnych endpointów HTTP — `createHttpAuthService` przyjmuje `HttpAuthEndpoints`, więc dokładny kształt tras negocjujemy z `Mavrynt.Api` / `Mavrynt.AdminApp`, a nie wpiekamy do pakietu.

## Wskazówki testowe

Pakiet jest source-shipped; testy żyją w konsumujących aplikacjach, głównie w `mavrynt-web`:

- matryca fallbacków env `resolveAppUrls` — `src/lib/app-urls/resolveAppUrls.test.ts`
- maszyny stanów `useLoginForm` / `useRegisterForm` — `src/test/useLoginForm.test.tsx`, `src/test/useRegisterForm.test.tsx`
- triggery `consoleAuthService` — `src/test/consoleAuthService.test.ts`
- smoke Playwrighta przeciwko adapterowi konsolowemu — `tests/e2e/auth.spec.ts`

Użyj `buildAuthHarness()` z `src/test/authHarness.tsx`, aby zbudować drzewko providerów z realną instancją i18n oraz stubami `AuthService` / analytics na `vi.fn()`.
