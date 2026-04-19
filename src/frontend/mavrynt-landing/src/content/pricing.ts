/**
 * Pricing tier descriptor. All text lives in i18n under `pricing.tiers.<id>`.
 * This file only owns structure: feature-row count, highlight, CTA target.
 */
export type TierId = "starter" | "team" | "enterprise";

export interface PricingTier {
  readonly id: TierId;
  /** Number of feature rows to render (keys `features.1` … `features.N`). */
  readonly featureCount: number;
  readonly highlight?: boolean;
  readonly ctaTo: string;
  readonly ctaVariant: "primary" | "secondary";
}

export const pricingTiers: ReadonlyArray<PricingTier> = [
  {
    id: "starter",
    featureCount: 4,
    ctaTo: "/contact",
    ctaVariant: "secondary",
  },
  {
    id: "team",
    featureCount: 5,
    highlight: true,
    ctaTo: "/contact",
    ctaVariant: "primary",
  },
  {
    id: "enterprise",
    featureCount: 5,
    ctaTo: "/contact",
    ctaVariant: "secondary",
  },
];
