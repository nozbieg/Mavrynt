/**
 * Navigation model — shared between the top nav and the mobile menu.
 *
 * Labels are i18n keys (not raw strings) so the same descriptor works for
 * both PL and EN. Render in components via `t(item.labelKey)`.
 */
export interface NavItem {
  readonly id: string;
  readonly labelKey: string;
  readonly to: string;
  /** Internal = React Router navigation. External = plain anchor. */
  readonly external?: boolean;
}

export const primaryNav: ReadonlyArray<NavItem> = [
  { id: "features", labelKey: "nav.features", to: "/features" },
  { id: "pricing", labelKey: "nav.pricing", to: "/pricing" },
  { id: "contact", labelKey: "nav.contact", to: "/contact" },
];
