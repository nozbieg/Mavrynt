import { createContext, useContext } from "react";
import { createConsoleLeadService } from "./consoleLeadService.ts";
import type { LeadService } from "./types.ts";

export { createConsoleLeadService } from "./consoleLeadService.ts";
export { createHttpLeadService } from "./httpLeadService.ts";
export { LeadSubmissionError } from "./types.ts";
export type { LeadPayload, LeadService } from "./types.ts";

/**
 * React context for the lead service. Defaults to the console adapter
 * so components always have *some* working implementation (Liskov —
 * the default honours the same contract, just doesn't leave the browser).
 */
export const LeadServiceContext = createContext<LeadService>(
  createConsoleLeadService(),
);

export const useLeadService = (): LeadService => useContext(LeadServiceContext);
