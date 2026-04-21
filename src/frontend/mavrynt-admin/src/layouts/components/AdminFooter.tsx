import { useTranslation } from "react-i18next";
import { Footer } from "@mavrynt/ui";
import { resolveAppUrls } from "@mavrynt/config/app-urls";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";

const appUrls = resolveAppUrls();

/**
 * AdminFooter — slim footer for the internal operator console.
 *
 * Deliberately minimal: no sitemap, just a copyright/internal-use
 * marker plus escape-hatch links back to the user web SPA and the
 * marketing site (resolved through the shared URL helper).
 */
export const AdminFooter = () => {
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
              href={appUrls.web}
              className="rounded-sm text-fg-muted hover:text-fg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg"
            >
              {t("nav.backToWeb")}
            </a>
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
