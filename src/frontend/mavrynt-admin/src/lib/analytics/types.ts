/**
 * Analytics port — minimal interface shared with the marketing + web
 * SPAs so adapters (Plausible, PostHog, GA, etc.) can be bolted on
 * later without changing callers. Fire-and-forget by design.
 */
export interface AnalyticsClient {
  /** Record a page view. Called once per route change. */
  readonly pageView: (path: string) => void;
  /** Record a named event with optional props (kept loose intentionally). */
  readonly track: (
    event: string,
    props?: Readonly<Record<string, unknown>>,
  ) => void;
}
