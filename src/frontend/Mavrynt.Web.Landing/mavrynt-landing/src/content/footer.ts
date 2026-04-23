import type { NavItem } from "./navigation.ts";

/**
 * Footer column descriptor. Column titles and item labels are i18n keys.
 */
export interface FooterColumnSpec {
  readonly id: string;
  readonly titleKey: string;
  readonly items: ReadonlyArray<NavItem>;
}

export const footerColumns: ReadonlyArray<FooterColumnSpec> = [
  {
    id: "product",
    titleKey: "footer.product.title",
    items: [
      { id: "features", labelKey: "nav.features", to: "/features" },
      { id: "pricing", labelKey: "nav.pricing", to: "/pricing" },
    ],
  },
  {
    id: "company",
    titleKey: "footer.company.title",
    items: [{ id: "contact", labelKey: "nav.contact", to: "/contact" }],
  },
  {
    id: "legal",
    titleKey: "footer.legal.title",
    items: [
      { id: "privacy", labelKey: "footer.legal.privacy", to: "/legal/privacy" },
      { id: "terms", labelKey: "footer.legal.terms", to: "/legal/terms" },
    ],
  },
];
