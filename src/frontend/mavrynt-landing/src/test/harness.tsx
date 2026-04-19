import type { FC, ReactNode } from "react";
import i18next, { type i18n as I18nInstance } from "i18next";
import { I18nextProvider, initReactI18next } from "react-i18next";
import { vi } from "vitest";
import {
  AnalyticsContext,
  type AnalyticsClient,
} from "../lib/analytics/index.ts";
import {
  LeadServiceContext,
  type LeadPayload,
  type LeadService,
} from "../lib/lead/index.ts";
import enCommon from "../lib/i18n/locales/en/common.json";

/**
 * Test harness — builds an in-memory i18n instance and a provider tree
 * that mirrors the production `Providers.tsx` shape (Analytics + Lead).
 *
 * Design:
 *  - We reuse the real `en/common.json` so tests assert against the
 *    actual copy the user will see (no synthetic translation fixtures
 *    that can drift from production strings).
 *  - Analytics and Lead are exposed as `vi.fn()`-backed mocks so tests
 *    can assert on call shape without a production adapter running.
 */

export interface TestHarness {
  readonly i18n: I18nInstance;
  readonly analytics: AnalyticsClient;
  readonly leadService: LeadService;
  readonly Wrapper: FC<{ readonly children: ReactNode }>;
}

export interface HarnessOverrides {
  readonly leadService?: LeadService;
  readonly analytics?: AnalyticsClient;
  readonly language?: "en" | "pl";
}

export const createAnalyticsMock = (): AnalyticsClient => ({
  pageView: vi.fn(),
  track: vi.fn(),
});

/**
 * Default no-op submit — accepts a payload so the inferred function
 * type matches `LeadService["submit"]` exactly (required under
 * `strictFunctionTypes`).
 */
const defaultSubmit = async (_payload: LeadPayload): Promise<void> =>
  Promise.resolve();

export const createLeadServiceMock = (
  impl: (payload: LeadPayload) => Promise<void> = defaultSubmit,
): LeadService => ({
  submit: vi.fn(impl),
});

export const createTestI18n = async (
  language: "en" | "pl" = "en",
): Promise<I18nInstance> => {
  const instance = i18next.createInstance();
  await instance.use(initReactI18next).init({
    lng: language,
    fallbackLng: "en",
    defaultNS: "common",
    ns: ["common"],
    resources: { en: { common: enCommon } },
    interpolation: { escapeValue: false },
    react: { useSuspense: false },
  });
  return instance;
};

export const buildHarness = async (
  overrides: HarnessOverrides = {},
): Promise<TestHarness> => {
  const i18n = await createTestI18n(overrides.language ?? "en");
  const analytics = overrides.analytics ?? createAnalyticsMock();
  const leadService = overrides.leadService ?? createLeadServiceMock();

  const Wrapper: FC<{ readonly children: ReactNode }> = ({ children }) => (
    <I18nextProvider i18n={i18n}>
      <AnalyticsContext.Provider value={analytics}>
        <LeadServiceContext.Provider value={leadService}>
          {children}
        </LeadServiceContext.Provider>
      </AnalyticsContext.Provider>
    </I18nextProvider>
  );

  return { i18n, analytics, leadService, Wrapper };
};
