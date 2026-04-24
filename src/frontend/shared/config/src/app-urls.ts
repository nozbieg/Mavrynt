/**
 * Cross-app URL resolver.
 *
 * Single source of truth for "where do the three Mavrynt SPAs live?".
 * Every frontend imports this helper to render absolute links to sibling
 * SPAs without duplicating host/port config anywhere in the codebase.
 *
 * Resolution order (first match wins):
 *   1. VITE_APP_URL_<APP>  — injected by Aspire AppHost (.WithEnvironment)
 *                            in dev, or set by deployment pipeline in prod.
 *   2. Same-origin fallback — window.location.origin + the SPA's canonical
 *                            base path. Works when all three SPAs are served
 *                            from one origin behind a reverse proxy (prod).
 *
 * No ports or hostnames are hardcoded. All URLs are either injected via env
 * or derived at runtime from the browser's current origin.
 *
 * SOLID:
 * - Single Responsibility : URL resolution only.
 * - Dependency Inversion  : env source is injected (default import.meta.env)
 *                           so the module is testable without Vite.
 */

export type AppId = "landing" | "web" | "admin";

export interface AppUrls {
  readonly landing: string;
  readonly web: string;
  readonly admin: string;
}

/**
 * Canonical base paths for each SPA.
 *
 * Must match the `base` option in each project's vite.config.ts:
 *   - mavrynt-landing : VITE_LANDING_BASE ?? "/"
 *   - mavrynt-web     : VITE_APP_BASE     ?? "/app/"
 *   - mavrynt-admin   : VITE_ADMIN_BASE   ?? "/admin/"
 *
 * These paths are used as the same-origin fallback when no
 * VITE_APP_URL_* env var is present. They are the only values that
 * legitimately belong in shared config — they are routing contracts,
 * not deployment details.
 */
export const SPA_BASE_PATHS: Readonly<Record<AppId, string>> = Object.freeze({
  landing: "/",
  web: "/app",
  admin: "/admin",
});

const PRIMARY_ENV_KEYS: Readonly<Record<AppId, string>> = Object.freeze({
  landing: "VITE_APP_URL_LANDING",
  web: "VITE_APP_URL_WEB",
  admin: "VITE_APP_URL_ADMIN",
});

const normalise = (raw: string): string => raw.replace(/\/+$/u, "");

const readEnvSource = (): Record<string, string | undefined> => {
  try {
    return (import.meta as ImportMeta).env as unknown as Record<
      string,
      string | undefined
    >;
  } catch {
    return {};
  }
};

/**
 * Returns the current browser origin (e.g. "https://mavrynt.com").
 * Falls back to an empty string in SSR / test environments where
 * `window` is not defined.
 */
const currentOrigin = (): string => {
  try {
    return window.location.origin;
  } catch {
    return "";
  }
};

export const resolveAppUrls = (
  source: Readonly<Record<string, string | undefined>> = readEnvSource(),
): AppUrls => {
  const origin = currentOrigin();

  const pick = (app: AppId): string => {
    const injected = source[PRIMARY_ENV_KEYS[app]];
    if (injected) return normalise(injected);
    // Same-origin fallback: correct for production reverse-proxy deployments.
    // In Aspire dev all VITE_APP_URL_* vars are always injected by AppHost,
    // so this branch is never reached during local development.
    return normalise(`${origin}${SPA_BASE_PATHS[app]}`);
  };

  return Object.freeze({
    landing: pick("landing"),
    web: pick("web"),
    admin: pick("admin"),
  });
};

export const resolveAppUrl = (
  app: AppId,
  source?: Readonly<Record<string, string | undefined>>,
): string => resolveAppUrls(source)[app];
