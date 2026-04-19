import type { LeadPayload, LeadService } from "./types.ts";

/**
 * Default development adapter — logs the payload and resolves. Keeps
 * the form functional end-to-end without any backend commitment
 * (Phase 0 decision: "decide later" for form backend).
 *
 * Never ship this to production; `Providers.tsx` swaps in a real
 * adapter when `VITE_LEAD_ENDPOINT` (or equivalent) is configured.
 */
export const createConsoleLeadService = (): LeadService => ({
  submit: async (payload: LeadPayload): Promise<void> => {
    // `console.warn` is allowed by the shared ESLint config; this adapter
    // is a development/test fallback, not a production lead pipeline.
    console.warn("[LeadService:console] would submit", payload);
    await new Promise<void>((resolve) => {
      setTimeout(resolve, 250);
    });
  },
});
