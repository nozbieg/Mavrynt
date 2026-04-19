import type { ReactNode } from "react";
import { HelmetProvider } from "react-helmet-async";
import { I18nextProvider } from "react-i18next";
import type { i18n as I18nInstance } from "i18next";
import { ThemeProvider } from "@mavrynt/ui";
import {
  AnalyticsContext,
  noopAnalytics,
  type AnalyticsClient,
} from "../lib/analytics/index.ts";

/**
 * Providers — single composition root for cross-cutting app concerns.
 *
 * Order matters:
 *  1. HelmetProvider (head-tag side effects)
 *  2. I18nextProvider (everything below can translate)
 *  3. ThemeProvider (<html data-theme="...">)
 *  4. AnalyticsContext (routing-aware consumers)
 *
 * Analytics defaults to the noop adapter so nothing fires in dev/tests
 * unless an explicit client is injected by the host app.
 */
export interface ProvidersProps {
  readonly i18n: I18nInstance;
  readonly analytics?: AnalyticsClient;
  readonly children: ReactNode;
}

export const Providers = ({
  i18n,
  analytics = noopAnalytics,
  children,
}: ProvidersProps) => (
  <HelmetProvider>
    <I18nextProvider i18n={i18n}>
      <ThemeProvider defaultMode="system">
        <AnalyticsContext.Provider value={analytics}>
          {children}
        </AnalyticsContext.Provider>
      </ThemeProvider>
    </I18nextProvider>
  </HelmetProvider>
);
