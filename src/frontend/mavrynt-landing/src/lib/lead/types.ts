/**
 * LeadService port — the contact form depends on this interface only,
 * never on a concrete adapter. Backend choice (Mavrynt API, CRM SaaS,
 * simple mailer) is decided later and swapped in `Providers.tsx` without
 * touching form code (Dependency Inversion / ADR-010: landing stays
 * backend-independent by default).
 */
export interface LeadPayload {
  readonly name: string;
  readonly email: string;
  readonly company?: string;
  readonly message: string;
  /** Free-form source tag, e.g. "landing:contact" or "landing:pricing". */
  readonly source: string;
  /** IETF BCP 47 locale the form was submitted from. */
  readonly locale: string;
}

/**
 * Structured error the form UI can branch on. Concrete adapters throw
 * this (never bare `Error`) so callers don't need to inspect messages.
 */
export class LeadSubmissionError extends Error {
  public override readonly name = "LeadSubmissionError";
  public readonly code: "network" | "validation" | "rate_limited" | "server";

  public constructor(
    code: LeadSubmissionError["code"],
    message: string,
    options?: { readonly cause?: unknown },
  ) {
    super(message, options);
    this.code = code;
  }
}

export interface LeadService {
  /**
   * Submit a lead. Resolves on success; throws `LeadSubmissionError` on
   * any failure. No implicit retry — the caller decides UX (retry
   * button, fallback email link, etc).
   */
  readonly submit: (payload: LeadPayload) => Promise<void>;
}
