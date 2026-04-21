import type { ReactNode } from "react";
import { HelmetProvider } from "react-helmet-async";
import { I18nextProvider } from "react-i18next";
import type { i18n as I18nInstance } from "i18next";
import { ThemeProvider } from "@mavrynt/ui";
import {
  AuthServiceContext,
  AuthAnalyticsContext,
  noopAuthAnalytics,
  type AuthAnalyticsPort,
  type AuthService,
} from "@mavrynt/auth-ui";
import {
  AnalyticsContext,
  noopAnalytics,
  type AnalyticsClient,
} from "../lib/analytics/index.ts";
import { authService as defaultAuthService } from "../lib/auth/authService.ts";

/**
 * Providers — single composition root for cross-cutting concerns in
 * `mavrynt-web`. Mirrors the order used by `mavrynt-landing` so both
 * apps feel identical at runtime:
 *
 *  1. HelmetProvider       — head-tag side effects
 *  2. I18nextProvider      — translation below
 *  3. ThemeProvider        — `<html data-theme="...">`
 *  4. AnalyticsContext     — app-level analytics (pageview, generic events)
 *  5. AuthAnalyticsContext — auth-specific analytics port (`@mavrynt/auth-ui`)
 *  6. AuthServiceContext   — injected AuthService adapter (console by default)
 *
 * Every external dependency is a port with a safe default — the SPA
 * boots end-to-end in any environment (even offline tests) without
 * wiring real adapters. Hosts override any of these in their tests
 * simply by passing props.
 */
export interface ProvidersProps {
  readonly i18n: I18nInstance;
  readonly analytics?: AnalyticsClient;
  readonly authAnalytics?: AuthAnalyticsPort;
  readonly authService?: AuthService;
  readonly children: ReactNode;
}

export const Providers = ({
  i18n,
  analytics = noopAnalytics,
  authAnalytics = noopAuthAnalytics,
  authService = defaultAuthService,
  children,
}: ProvidersProps) => (
  <HelmetProvider>
    <I18nextProvider i18n={i18n}>
      <ThemeProvider defaultMode="system">
        <AnalyticsContext.Provider value={analytics}>
          <AuthAnalyticsContext.Provider value={authAnalytics}>
            <AuthServiceContext.Provider value={authService}>
              {children}
            </AuthServiceContext.Provider>
          </AuthAnalyticsContext.Provider>
        </AnalyticsContext.Provider>
      </ThemeProvider>
    </I18nextProvider>
  </HelmetProvider>
);
