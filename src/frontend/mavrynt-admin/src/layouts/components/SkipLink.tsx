import { useTranslation } from "react-i18next";

/**
 * SkipLink — WCAG 2.4.1 Bypass Blocks. Mirrors the identical component
 * in `mavrynt-web` / `mavrynt-landing` so cross-app keyboard behaviour
 * stays uniform.
 */
export const SkipLink = () => {
  const { t } = useTranslation();
  return (
    <a
      href="#main"
      className="sr-only focus:not-sr-only focus:fixed focus:left-4 focus:top-4 focus:z-50 focus:rounded-md focus:bg-primary focus:px-4 focus:py-2 focus:text-primary-fg focus:shadow-lg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg"
    >
      {t("skip")}
    </a>
  );
};
