import { useCallback, useEffect } from "react";
import { useTranslation } from "react-i18next";
import {
  SUPPORTED_LOCALES,
  persistLocale,
  type SupportedLocale,
} from "@mavrynt/config/i18n";

/**
 * Typed locale hook. Wraps `react-i18next` so callers never touch the
 * loose `string` locale type leaking from i18next itself.
 *
 * SOLID — Dependency Inversion: UI depends on this narrow contract,
 * not on i18next directly. Swapping the provider later only changes
 * this file.
 */
export interface LocaleController {
  readonly locale: SupportedLocale;
  readonly supported: ReadonlyArray<SupportedLocale>;
  readonly setLocale: (locale: SupportedLocale) => Promise<void>;
}

const isSupported = (value: string): value is SupportedLocale =>
  (SUPPORTED_LOCALES as ReadonlyArray<string>).includes(value);

export const useLocale = (): LocaleController => {
  const { i18n } = useTranslation();

  const current = (
    isSupported(i18n.language) ? i18n.language : "pl"
  ) satisfies SupportedLocale;

  // Keep `<html lang>` synced with the active locale — covers the
  // initial mount (when the detector picked a non-default locale) and
  // any subsequent change. Screen readers use `<html lang>` to choose
  // the correct pronunciation dictionary (WCAG 3.1.1 / 3.1.2).
  useEffect(() => {
    if (typeof document !== "undefined") {
      document.documentElement.setAttribute("lang", current);
    }
  }, [current]);

  const setLocale = useCallback(
    async (next: SupportedLocale): Promise<void> => {
      await i18n.changeLanguage(next);
      persistLocale(next);
      // The useEffect above handles the DOM sync; persist + i18n swap
      // are the setter's only responsibilities.
    },
    [i18n],
  );

  return {
    locale: current,
    supported: SUPPORTED_LOCALES,
    setLocale,
  };
};
