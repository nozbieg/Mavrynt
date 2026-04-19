/**
 * Analytics port — Phase 0 decision was "default" (hook stub only).
 *
 * The interface is deliberately minimal so adapters (Plausible, PostHog,
 * GA, etc.) can be bolted on without changing callers. Keep it purely
 * fire-and-forget — no returns, no promises leaking into callers.
 */
export interface AnalyticsClient {
  /** Record a page view. Called once per route change. */
  readonly pageView: (path: string) => void;
  /** Record a named event with optional props (kept loose intentionally). */
  readonly track: (event: string, props?: Readonly<Record<string, unknown>>) => void;
}
