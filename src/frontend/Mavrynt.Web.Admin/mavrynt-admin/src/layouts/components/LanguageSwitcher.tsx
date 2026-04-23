import { useId } from "react";
import { useTranslation } from "react-i18next";
import type { SupportedLocale } from "@mavrynt/config/i18n";
import { useLocale } from "../../lib/i18n/useLocale.ts";

/**
 * LanguageSwitcher — native `<select>`: cheap, a11y-correct, keyboard
 * friendly. Mirrors web / landing versions so the control looks and
 * behaves identically across all SPAs.
 */
export const LanguageSwitcher = () => {
  const { t } = useTranslation();
  const { locale, supported, setLocale } = useLocale();
  const selectId = useId();

  return (
    <div className="flex items-center gap-2 text-sm">
      <label htmlFor={selectId} className="sr-only">
        {t("language.label")}
      </label>
      <select
        id={selectId}
        value={locale}
        onChange={(event) => {
          void setLocale(event.target.value as SupportedLocale);
        }}
        className="rounded-md border border-border bg-bg px-2 py-1 text-sm text-fg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg"
      >
        {supported.map((code) => (
          <option key={code} value={code}>
            {t(`language.${code}`)}
          </option>
        ))}
      </select>
    </div>
  );
};
