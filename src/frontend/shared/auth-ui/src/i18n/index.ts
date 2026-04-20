import enAuth from "./en.json" with { type: "json" };
import plAuth from "./pl.json" with { type: "json" };

/**
 * Auth namespace resources, ready to be merged into a consuming app's
 * i18next instance. The forms use `useTranslation("auth")` internally,
 * so apps must register the bundle under namespace `"auth"`.
 *
 * Usage from a consuming app's i18n bootstrap:
 *
 * ```ts
 * import { authI18nResources, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui/i18n";
 *
 * i18n.addResourceBundle("en", AUTH_I18N_NAMESPACE, authI18nResources.en, true, true);
 * i18n.addResourceBundle("pl", AUTH_I18N_NAMESPACE, authI18nResources.pl, true, true);
 * ```
 *
 * Or pass them directly into `createI18n({ resources: { en: { auth: authI18nResources.en }, ... } })`.
 */
export const AUTH_I18N_NAMESPACE = "auth" as const;

export const authI18nResources = {
  en: enAuth,
  pl: plAuth,
} as const;

export type AuthLocale = keyof typeof authI18nResources;
