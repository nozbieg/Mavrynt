import type { FC, ReactNode } from "react";
import i18next, { type i18n as I18nInstance } from "i18next";
import { I18nextProvider, initReactI18next } from "react-i18next";
import { vi } from "vitest";
import {
  AUTH_I18N_NAMESPACE,
  authI18nResources,
  AuthAnalyticsContext,
  type AuthAnalyticsPort,
  AuthServiceContext,
  type AuthService,
  type AuthSession,
} from "@mavrynt/auth-ui";

/**
 * Auth test harness.
 *
 * Wires up the bare minimum provider tree that `@mavrynt/auth-ui`
 * requires so we can exercise `useLoginForm` / `useRegisterForm` and
 * the rendered forms without spinning up the whole SPA `Providers`.
 *
 * Design notes:
 *  - We use the real auth i18n bundles (en.json / pl.json) so copy
 *    assertions match what users actually see. Synthetic fixtures
 *    would drift.
 *  - AuthService is a fully controllable `vi.fn()`-backed stub —
 *    tests choose when it resolves, when it rejects, and with what
 *    error code. This keeps each spec deterministic (no waits on the
 *    console adapter's 300 ms simulated latency).
 *  - AuthAnalytics is a vi.fn() so we can assert on the attempt /
 *    success / error events without dragging a real analytics client
 *    into the suite.
 */

export interface AuthTestHarness {
  readonly i18n: I18nInstance;
  readonly auth: AuthService;
  readonly analytics: AuthAnalyticsPort;
  readonly Wrapper: FC<{ readonly children: ReactNode }>;
}

export interface AuthHarnessOverrides {
  readonly auth?: AuthService;
  readonly analytics?: AuthAnalyticsPort;
  readonly language?: "en" | "pl";
}

export const createAuthAnalyticsMock = (): AuthAnalyticsPort => ({
  track: vi.fn(),
});

const resolveSession = (overrides?: Partial<AuthSession>): AuthSession => ({
  token: "test-token",
  user: {
    id: "test-user",
    email: "test@example.com",
    name: "Test User",
    roles: [],
  },
  ...overrides,
});

export const createAuthServiceMock = (
  impl?: Partial<AuthService>,
): AuthService => ({
  login: vi.fn(
    impl?.login ??
      (async (): Promise<AuthSession> => Promise.resolve(resolveSession())),
  ),
  register: vi.fn(
    impl?.register ??
      (async (): Promise<AuthSession> => Promise.resolve(resolveSession())),
  ),
  logout: vi.fn(impl?.logout ?? (async (): Promise<void> => Promise.resolve())),
});

const createTestI18n = async (
  language: "en" | "pl",
): Promise<I18nInstance> => {
  const instance = i18next.createInstance();
  await instance.use(initReactI18next).init({
    lng: language,
    fallbackLng: "en",
    defaultNS: AUTH_I18N_NAMESPACE,
    ns: [AUTH_I18N_NAMESPACE],
    resources: {
      en: { [AUTH_I18N_NAMESPACE]: authI18nResources.en },
      pl: { [AUTH_I18N_NAMESPACE]: authI18nResources.pl },
    },
    interpolation: { escapeValue: false },
    react: { useSuspense: false },
  });
  return instance;
};

export const buildAuthHarness = async (
  overrides: AuthHarnessOverrides = {},
): Promise<AuthTestHarness> => {
  const i18n = await createTestI18n(overrides.language ?? "en");
  const auth = overrides.auth ?? createAuthServiceMock();
  const analytics = overrides.analytics ?? createAuthAnalyticsMock();

  const Wrapper: FC<{ readonly children: ReactNode }> = ({ children }) => (
    <I18nextProvider i18n={i18n}>
      <AuthAnalyticsContext.Provider value={analytics}>
        <AuthServiceContext.Provider value={auth}>
          {children}
        </AuthServiceContext.Provider>
      </AuthAnalyticsContext.Provider>
    </I18nextProvider>
  );

  return { i18n, auth, analytics, Wrapper };
};
