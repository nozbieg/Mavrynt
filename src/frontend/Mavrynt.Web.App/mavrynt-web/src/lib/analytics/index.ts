import { createContext, useContext } from "react";
import type { AnalyticsClient } from "./types.ts";

/**
 * Default no-op analytics client. Safe to mount in every environment;
 * real adapters are injected via `<AnalyticsContext.Provider>` without
 * changing calling code.
 */
export const noopAnalytics: AnalyticsClient = {
  pageView: () => {
    /* noop */
  },
  track: () => {
    /* noop */
  },
};

export const AnalyticsContext = createContext<AnalyticsClient>(noopAnalytics);

export const useAnalytics = (): AnalyticsClient => useContext(AnalyticsContext);

export type { AnalyticsClient } from "./types.ts";
