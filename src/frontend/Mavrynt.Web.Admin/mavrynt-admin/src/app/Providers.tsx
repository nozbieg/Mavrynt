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
import { AdminSessionProvider } from "../lib/auth/AdminSessionProvider.tsx";
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
              <AdminSessionProvider>
                {children}
              </AdminSessionProvider>
            </AuthServiceContext.Provider>
          </AuthAnalyticsContext.Provider>
        </AnalyticsContext.Provider>
      </ThemeProvider>
    </I18nextProvider>
  </HelmetProvider>
);
