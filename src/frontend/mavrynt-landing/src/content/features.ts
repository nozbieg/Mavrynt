import type { IconName } from "../components/Icon.tsx";

/**
 * Feature descriptor. Labels come from `features.items.<id>` in the
 * i18n JSON; this file only holds the shape (id, icon, order).
 */
export interface FeatureItem {
  readonly id: string;
  readonly icon: IconName;
}

export const features: ReadonlyArray<FeatureItem> = [
  { id: "modular", icon: "puzzle" },
  { id: "typed", icon: "code" },
  { id: "observable", icon: "activity" },
  { id: "secure", icon: "shield" },
  { id: "scalable", icon: "trending-up" },
  { id: "extensible", icon: "plug" },
];
