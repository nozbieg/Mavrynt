/**
 * Cross-app URL resolver.
 *
 * Single source of truth for "where do the three Mavrynt SPAs live?".
 * Each frontend imports this helper and renders absolute links to the
 * other two SPAs without hard-coding hosts or ports in nav/footer code.
 *
 * Dev defaults match the YARP proxy routing:
 *   - landing  → http://localhost:5000 (main entry point, catch-all)
 *   - web      → http://localhost:5000/app
 *   - admin    → http://localhost:5000/admin
 *
 * Production overrides come from `VITE_APP_URL_*` environment
 * variables (set per-deployment; see Phase 5 docs). `VITE_MARKETING_URL`
 * and `VITE_WEB_URL` are honoured as legacy aliases so we can roll this
 * helper out without a flag-day rename.
 *
 * When running in .NET Aspire, set `VITE_SERVICE_DISCOVERY_URL` to the
 * app host URL (e.g., http://localhost:15888) to dynamically resolve URLs
 * from Aspire's service discovery endpoint.
 *
 * SOLID:
 * - Single Responsibility: just URL resolution.
 * - Dependency Inversion: the raw env source is injected (defaults to
 *   `import.meta.env`) so this module is testable without Vite.
 */

/** Apps in the Mavrynt frontend. */
export type AppId = "landing" | "web" | "admin";

export interface AppUrls {
  readonly landing: string;
  readonly web: string;
  readonly admin: string;
}

/** Dev defaults — line up with YARP proxy routing. */
export const DEFAULT_APP_URLS: AppUrls = Object.freeze({
  landing: "http://localhost:5000",
  web: "http://localhost:5000/app",
  admin: "http://localhost:5000/admin",
});

/** Canonical env keys (new, preferred). */
const PRIMARY_ENV_KEYS: Readonly<Record<AppId, string>> = Object.freeze({
  landing: "VITE_APP_URL_LANDING",
  web: "VITE_APP_URL_WEB",
  admin: "VITE_APP_URL_ADMIN",
});

/** Legacy aliases kept for backwards compatibility with Phase 2/3 env. */
const LEGACY_ENV_KEYS: Readonly<Record<AppId, readonly string[]>> = Object.freeze({
  landing: ["VITE_MARKETING_URL"],
  web: ["VITE_WEB_URL"],
  admin: ["VITE_ADMIN_URL"],
});

const SERVICE_DISCOVERY_URL_KEY = "VITE_SERVICE_DISCOVERY_URL";

const SERVICE_NAMES: Readonly<Record<AppId, string>> = Object.freeze({
  landing: "landing",
  web: "web",
  admin: "adminWeb",
});

const firstNonEmpty = (
  values: ReadonlyArray<string | undefined>,
): string | undefined => {
  for (const value of values) {
    if (typeof value === "string" && value.length > 0) return value;
  }
  return undefined;
};

const normalise = (raw: string): string => raw.replace(/\/+$/u, "");

const readEnvSource = (): Record<string, string | undefined> => {
  try {
    return (import.meta as ImportMeta).env as unknown as Record<
      string,
      string | undefined
    >;
  } catch {
    // Non-Vite environment (e.g. Node tests) — return an empty bag so
    // callers fall back to `DEFAULT_APP_URLS`.
    return {};
  }
};

/**
 * Resolve the absolute URL of every Mavrynt SPA.
 *
 * @param source  optional raw env map (defaults to `import.meta.env`);
 *                inject one in tests to exercise different deployments
 *                without touching process state.
 */
export const resolveAppUrls = (
  source: Readonly<Record<string, string | undefined>> = readEnvSource(),
): AppUrls => {
  const pick = (app: AppId): string => {
    const primary = source[PRIMARY_ENV_KEYS[app]];
    const legacy = LEGACY_ENV_KEYS[app].map((key) => source[key]);
    const chosen = firstNonEmpty([primary, ...legacy]);
    return normalise(chosen ?? DEFAULT_APP_URLS[app]);
  };

  return Object.freeze({
    landing: pick("landing"),
    web: pick("web"),
    admin: pick("admin"),
  });
};

/** Shorthand for `resolveAppUrls()[app]`. */
export const resolveAppUrl = (
  app: AppId,
  source?: Readonly<Record<string, string | undefined>>,
): string => resolveAppUrls(source)[app];

/**
 * Asynchronously resolve the absolute URL of every Mavrynt SPA, optionally
 * using Aspire service discovery if VITE_SERVICE_DISCOVERY_URL is set.
 *
 * @param source  optional raw env map (defaults to `import.meta.env`);
 *                inject one in tests to exercise different deployments
 *                without touching process state.
 */
export const resolveAppUrlsAsync = async (
  source: Readonly<Record<string, string | undefined>> = readEnvSource(),
): Promise<AppUrls> => {
  const discoveryUrl = source[SERVICE_DISCOVERY_URL_KEY];
  if (!discoveryUrl) {
    return resolveAppUrls(source);
  }

  const fetchUrl = async (serviceName: string): Promise<string | null> => {
    try {
      const response = await fetch(`${discoveryUrl}/v1.0/resolver/resolve/${serviceName}`);
      if (!response.ok) throw new Error(`HTTP ${response.status}`);
      const data: { address: string } = await response.json();
      return normalise(data.address);
    } catch (error) {
      console.warn(`Failed to resolve ${serviceName} from service discovery:`, error);
      return null;
    }
  };

  const urlPromises = Object.entries(SERVICE_NAMES).map(async ([appId, serviceName]) => {
    const url = await fetchUrl(serviceName);
    return { appId: appId as AppId, url: url ?? resolveAppUrl(appId as AppId, source) };
  });

  const resolved = await Promise.all(urlPromises);

  const urls: AppUrls = {
    landing: resolved.find(r => r.appId === 'landing')!.url,
    web: resolved.find(r => r.appId === 'web')!.url,
    admin: resolved.find(r => r.appId === 'admin')!.url,
  };

  return Object.freeze(urls);
};

/** Async shorthand for `resolveAppUrlsAsync()[app]`. */
export const resolveAppUrlAsync = async (
  app: AppId,
  source?: Readonly<Record<string, string | undefined>>,
): Promise<string> => (await resolveAppUrlsAsync(source))[app];
