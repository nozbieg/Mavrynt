/**
 * i18n bootstrap factory.
 *
 * Wraps `i18next` + `react-i18next` so apps share a consistent setup
 * (PL/EN, browser language detection, lazy resource loading).
 *
 * The factory is the only place that touches `i18next` directly — apps
 * receive the typed instance and the `<I18nextProvider>` from `react-i18next`.
 *
 * Note: peer-imported. `i18next` and `react-i18next` are listed as
 * optional peer dependencies in this package — apps that need i18n must
 * install them and pass them in. This keeps `@mavrynt/config` free of
 * heavy runtime deps for apps that don't need translations.
 */

export type SupportedLocale = "pl" | "en";

export const DEFAULT_LOCALE: SupportedLocale = "pl";

export const SUPPORTED_LOCALES: ReadonlyArray<SupportedLocale> = ["pl", "en"];

/**
 * Resource bundle keyed by locale, then namespace.
 * Example: `{ en: { common: { hello: "Hi" } } }`
 */
export type I18nResources = Readonly<
  Record<SupportedLocale, Readonly<Record<string, Readonly<Record<string, unknown>>>>>
>;

export interface CreateI18nOptions {
  readonly resources: I18nResources;
  readonly defaultNamespace?: string;
  readonly fallbackLocale?: SupportedLocale;
  /** Detect locale from `navigator.language` and `localStorage`. Default: true. */
  readonly detect?: boolean;
}

/**
 * Returns a configured i18next instance ready to be passed to
 * `<I18nextProvider i18n={...}>`. Imports of `i18next` are dynamic so
 * the dependency is only loaded when this factory is called.
 */
export const createI18n = async (options: CreateI18nOptions): Promise<unknown> => {
  const i18nextModule = (await import("i18next")) as {
    default: { createInstance: () => unknown };
  };
  const reactI18nextModule = (await import("react-i18next")) as {
    initReactI18next: unknown;
  };

  const instance = i18nextModule.default.createInstance() as {
    use: (plugin: unknown) => typeof instance;
    init: (config: unknown) => Promise<unknown>;
  };

  const detected = options.detect !== false ? detectLocale() : DEFAULT_LOCALE;

  await instance
    .use(reactI18nextModule.initReactI18next)
    .init({
      resources: options.resources,
      lng: detected,
      fallbackLng: options.fallbackLocale ?? DEFAULT_LOCALE,
      defaultNS: options.defaultNamespace ?? "common",
      supportedLngs: SUPPORTED_LOCALES,
      interpolation: { escapeValue: false },
      returnNull: false,
    });

  return instance;
};

const STORAGE_KEY = "mavrynt:locale";

const detectLocale = (): SupportedLocale => {
  if (typeof window === "undefined") return DEFAULT_LOCALE;

  try {
    const stored = window.localStorage.getItem(STORAGE_KEY);
    if (stored && (SUPPORTED_LOCALES as ReadonlyArray<string>).includes(stored)) {
      return stored as SupportedLocale;
    }
  } catch {
    // localStorage may be blocked — ignore and fall back to navigator.
  }

  const nav = window.navigator.language.slice(0, 2).toLowerCase();
  if ((SUPPORTED_LOCALES as ReadonlyArray<string>).includes(nav)) {
    return nav as SupportedLocale;
  }
  return DEFAULT_LOCALE;
};

export const persistLocale = (locale: SupportedLocale): void => {
  if (typeof window === "undefined") return;
  try {
    window.localStorage.setItem(STORAGE_KEY, locale);
  } catch {
    // ignore — non-fatal
  }
};
