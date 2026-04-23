/**
 * Cross-app URL resolver.
 *
 * Single source of truth for "where do the three Mavrynt SPAs live?".
 * Każdy frontend importuje ten helper i renderuje absolutne linki do pozostałych SPA,
 * bez hardcodowania hostów/portów w kodzie nawigacji czy stopki.
 *
 * Domyślne porty dev:
 *   - landing  → http://localhost:5173
 *   - web      → http://localhost:5174
 *   - admin    → http://localhost:5175
 *
 * Produkcyjne nadpisania pochodzą z `VITE_APP_URL_*` (ustawiane przez AppHost).
 *
 * SOLID:
 * - Single Responsibility: tylko rozwiązywanie URL.
 * - Dependency Inversion: źródło env jest wstrzykiwane (domyślnie import.meta.env), więc testowalne bez Vite.
 */

export type AppId = "landing" | "web" | "admin";

export interface AppUrls {
  readonly landing: string;
  readonly web: string;
  readonly admin: string;
}

export const DEFAULT_APP_URLS: AppUrls = Object.freeze({
  landing: "http://localhost:5173",
  web: "http://localhost:5174",
  admin: "http://localhost:5175",
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

export const resolveAppUrls = (
  source: Readonly<Record<string, string | undefined>> = readEnvSource(),
): AppUrls => {
  const pick = (app: AppId): string => {
    const primary = source[PRIMARY_ENV_KEYS[app]];
    return normalise(primary ?? DEFAULT_APP_URLS[app]);
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
