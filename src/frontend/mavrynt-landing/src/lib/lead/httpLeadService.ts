import {
  LeadSubmissionError,
  type LeadPayload,
  type LeadService,
} from "./types.ts";

/**
 * HTTP adapter — POSTs the payload as JSON. Endpoint is injected, so
 * the same adapter works against:
 *  - the Mavrynt API (when we decide to accept direct writes),
 *  - a webhook (Formspark / Formspree / your own Lambda),
 *  - a local mock in tests.
 *
 * Maps HTTP status codes onto `LeadSubmissionError.code` so the form
 * can render precise error copy without parsing response bodies.
 */
export interface HttpLeadServiceOptions {
  readonly endpoint: string;
  /** Extra headers (e.g. CSRF token, API key). */
  readonly headers?: Readonly<Record<string, string>>;
  /** Abort after this many ms. Default 10s. */
  readonly timeoutMs?: number;
}

const DEFAULT_TIMEOUT = 10_000;

export const createHttpLeadService = (
  options: HttpLeadServiceOptions,
): LeadService => ({
  submit: async (payload: LeadPayload): Promise<void> => {
    const controller = new AbortController();
    const timer = setTimeout(
      () => {
        controller.abort();
      },
      options.timeoutMs ?? DEFAULT_TIMEOUT,
    );

    let response: Response;
    try {
      response = await fetch(options.endpoint, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
          ...options.headers,
        },
        body: JSON.stringify(payload),
        signal: controller.signal,
      });
    } catch (cause) {
      throw new LeadSubmissionError("network", "Network request failed.", {
        cause,
      });
    } finally {
      clearTimeout(timer);
    }

    if (response.ok) return;

    if (response.status === 422 || response.status === 400) {
      throw new LeadSubmissionError("validation", "Validation failed.");
    }
    if (response.status === 429) {
      throw new LeadSubmissionError("rate_limited", "Too many requests.");
    }
    throw new LeadSubmissionError(
      "server",
      `Server responded with ${String(response.status)}.`,
    );
  },
});
