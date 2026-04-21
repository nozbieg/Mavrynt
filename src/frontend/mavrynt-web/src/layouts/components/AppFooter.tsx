import { useTranslation } from "react-i18next";
import { Footer } from "@mavrynt/ui";
import { resolveAppUrls } from "@mavrynt/config/app-urls";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";

/**
 * AppFooter — slim footer for the authenticated Web SPA.
 *
 * Intentionally leaner than the marketing footer: no sitemap columns,
 * just brand / legal pointers to the marketing site + language. Shares
 * the same `<Footer>` shell from `@mavrynt/ui` for visual parity.
 */
const appUrls = resolveAppUrls();

export const AppFooter = () => {
  const { t } = useTranslation();
  const year = new Date().getFullYear();

  return (
    <Footer
      ariaLabel={t("a11y.footer")}
      tagline={t("footer.tagline")}
      columns={[]}
      bottom={
        <>
          <span>{t("footer.copyright", { year })}</span>
          <div className="flex items-center gap-4">
            <a
              href={appUrls.landing}
              className="rounded-sm text-fg-muted hover:text-fg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg"
            >
              {t("nav.backToSite")}
            </a>
            <LanguageSwitcher />
          </div>
        </>
      }
    />
  );
};
