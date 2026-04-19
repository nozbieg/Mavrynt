import { Helmet } from "react-helmet-async";
import { useTranslation } from "react-i18next";
import { siteConfig } from "../../content/site.ts";
import type { SeoProps } from "./types.ts";

/**
 * Seo — single place that writes head tags. Pages declare intent via
 * `SeoProps`; this component handles title formatting, canonical, OG,
 * Twitter, and locale metadata.
 *
 * All pages mount exactly one `<Seo>` at the top of their render tree;
 * react-helmet-async merges conflicting tags by "last-wins" so the
 * layout can also set fallback defaults without colliding.
 */
export const Seo = ({
  title,
  description,
  canonical,
  ogImage,
  noIndex = false,
}: SeoProps) => {
  const { i18n } = useTranslation();
  const locale = i18n.language || siteConfig.defaultLocale;
  const fullTitle = `${title} — ${siteConfig.name}`;
  const resolvedImage = ogImage ?? siteConfig.defaultOgImage;
  const resolvedOgImageUrl = resolvedImage.startsWith("http")
    ? resolvedImage
    : `${siteConfig.url}${resolvedImage}`;

  return (
    <Helmet>
      <html lang={locale} />
      <title>{fullTitle}</title>
      {description !== undefined && (
        <meta name="description" content={description} />
      )}
      {canonical !== undefined && <link rel="canonical" href={canonical} />}
      {noIndex && <meta name="robots" content="noindex, nofollow" />}

      <meta property="og:site_name" content={siteConfig.name} />
      <meta property="og:title" content={fullTitle} />
      {description !== undefined && (
        <meta property="og:description" content={description} />
      )}
      <meta property="og:image" content={resolvedOgImageUrl} />
      <meta property="og:type" content="website" />
      <meta property="og:locale" content={locale} />

      <meta name="twitter:card" content="summary_large_image" />
      <meta name="twitter:title" content={fullTitle} />
      {description !== undefined && (
        <meta name="twitter:description" content={description} />
      )}
      <meta name="twitter:image" content={resolvedOgImageUrl} />
    </Helmet>
  );
};
