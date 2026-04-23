import { Helmet } from "react-helmet-async";
import { useTranslation } from "react-i18next";

/**
 * Minimal SEO helper for the web SPA.
 *
 * Web is an authenticated area — we don't need the full OpenGraph /
 * Twitter coverage the marketing site has. This component sets title,
 * description, `<html lang>`, and a `noindex` default because most
 * app pages shouldn't appear in search results.
 *
 * Pages mount exactly one `<Seo>` at the top of their tree.
 */
export interface SeoProps {
  readonly title: string;
  readonly description?: string;
  /** Defaults to `true` — the auth area is not meant for crawlers. */
  readonly noIndex?: boolean;
}

const APP_NAME = "Mavrynt";

export const Seo = ({ title, description, noIndex = true }: SeoProps) => {
  const { i18n } = useTranslation();
  const locale = i18n.language || "pl";
  const fullTitle = `${title} — ${APP_NAME}`;

  return (
    <Helmet>
      <html lang={locale} />
      <title>{fullTitle}</title>
      {description !== undefined && (
        <meta name="description" content={description} />
      )}
      {noIndex && <meta name="robots" content="noindex, nofollow" />}
    </Helmet>
  );
};
