/**
 * SEO port — pages declare what they need, the `Seo` component handles
 * the head side-effects. Keeping the contract here (separate from the
 * `react-helmet-async` implementation) lets us swap the SEO adapter
 * later without touching pages (Dependency Inversion).
 */
export interface SeoProps {
  /** Page title, excluding the site name suffix — that is appended inside Seo. */
  readonly title: string;
  readonly description?: string;
  /** Canonical URL. If omitted, the current location is used at runtime. */
  readonly canonical?: string;
  /** Override default OG image (absolute or `/public`-relative URL). */
  readonly ogImage?: string;
  /** `noindex, nofollow` on pages we don't want crawled (drafts, 404s). */
  readonly noIndex?: boolean;
}
