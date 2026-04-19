import { useTranslation } from "react-i18next";
import { Footer, type FooterColumn } from "@mavrynt/ui";
import { footerColumns } from "../../content/footer.ts";
import { siteConfig } from "../../content/site.ts";
import { RouterLink } from "../../lib/router/RouterLink.tsx";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";

/**
 * MarketingFooter — composes the shared `<Footer>` shell with localized
 * column content and a bottom bar carrying copyright + language switcher.
 */
export const MarketingFooter = () => {
  const { t } = useTranslation();
  const year = new Date().getFullYear();

  const columns: ReadonlyArray<FooterColumn> = footerColumns.map((column) => ({
    id: column.id,
    title: t(column.titleKey),
    items: column.items.map((item) => (
      <li key={item.id}>
        <RouterLink to={item.to} variant="muted">
          {t(item.labelKey)}
        </RouterLink>
      </li>
    )),
  }));

  return (
    <Footer
      tagline={t("footer.tagline")}
      columns={columns}
      bottom={
        <>
          <span>{t("footer.copyright", { year })}</span>
          <div className="flex items-center gap-4">
            <a
              href={siteConfig.social.github}
              className="text-fg-muted hover:text-fg"
              rel="noopener noreferrer"
              target="_blank"
            >
              GitHub
            </a>
            <a
              href={siteConfig.social.linkedin}
              className="text-fg-muted hover:text-fg"
              rel="noopener noreferrer"
              target="_blank"
            >
              LinkedIn
            </a>
            <LanguageSwitcher />
          </div>
        </>
      }
    />
  );
};
