/**
 * Static site metadata — single source of truth for the marketing SPA.
 *
 * Keep this free of runtime concerns (no React, no routing). Anything
 * dynamic (i18n, env-driven URLs) belongs in `lib/` instead.
 */
export interface SiteConfig {
  readonly name: string;
  readonly domain: string;
  readonly url: string;
  /** Default OpenGraph / twitter share image, served from /public. */
  readonly defaultOgImage: string;
  /** ISO 639-1 default locale, mirrored into <html lang>. */
  readonly defaultLocale: "pl" | "en";
  readonly social: {
    readonly github: string;
    readonly linkedin: string;
  };
  readonly contactEmail: string;
}

export const siteConfig: SiteConfig = {
  name: "Mavrynt",
  domain: "mavrynt.com",
  url: "https://mavrynt.com",
  defaultOgImage: "/og/default.png",
  defaultLocale: "pl",
  social: {
    github: "https://github.com/mavrynt",
    linkedin: "https://www.linkedin.com/company/mavrynt",
  },
  contactEmail: "hello@mavrynt.com",
};
