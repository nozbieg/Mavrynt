/**
 * Web-app i18n bootstrap.
 *
 * Uses the shared factory from `@mavrynt/config/i18n` so all SPAs share
 * the same setup contract (DRY). Locale resource JSON lives next to this
 * file; add keys in `locales/{en,pl}/common.json`.
 *
 * The `auth` namespace is contributed by `@mavrynt/auth-ui` so the
 * login/register forms render translated strings out of the box.
 *
 * Exports a **promise** because `createI18n` is async. `main.tsx` awaits
 * it before rendering — this guarantees translations are ready on first
 * paint (no FOUC / key flashing).
 */
import { createI18n, type I18nResources } from "@mavrynt/config/i18n";
import { authI18nResources, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui/i18n";
import enCommon from "./locales/en/common.json";
import plCommon from "./locales/pl/common.json";

const resources: I18nResources = {
  pl: {
    common: plCommon,
    [AUTH_I18N_NAMESPACE]: authI18nResources.pl,
  },
  en: {
    common: enCommon,
    [AUTH_I18N_NAMESPACE]: authI18nResources.en,
  },
};

/**
 * Singleton promise — import and `await` in `main.tsx`. The resolved
 * value is the configured i18next instance, suitable for passing to
 * `<I18nextProvider i18n={...}>`.
 */
export const i18nPromise = createI18n({
  resources,
  defaultNamespace: "common",
  fallbackLocale: "pl",
  detect: true,
});
