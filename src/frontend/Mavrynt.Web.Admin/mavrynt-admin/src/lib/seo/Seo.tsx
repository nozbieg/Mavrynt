import { Helmet } from "react-helmet-async";
import { useTranslation } from "react-i18next";

/**
 * Minimal SEO helper for the admin SPA.
 *
 * Admin is an internal console — crawlers must never index it, so
 * `noIndex` defaults to `true`. The component sets `<title>`,
 * `<meta name="description">` (optional), `<html lang>`, and a robots
 * meta.
 *
 * Pages mount exactly one `<Seo>` at the top of their render tree.
 */
export interface SeoProps {
  readonly title: string;
  readonly description?: string;
  /** Defaults to `true` — admin must never be indexed. */
  readonly noIndex?: boolean;
}

const APP_NAME = "Mavrynt Admin";

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
