import type { IconName } from "../components/Icon.tsx";

/**
 * Feature descriptor. Labels come from `features.items.<id>` in the
 * i18n JSON; this file only holds the shape (id, icon, order).
 *
 * Ordering here drives the grid layout on Home (compact) and the
 * dedicated Features page, so the first three items should read as
 * the strongest pitch.
 */
export interface FeatureItem {
  readonly id: string;
  readonly icon: IconName;
}

export const features: ReadonlyArray<FeatureItem> = [
  { id: "monitoring", icon: "activity" },
  { id: "signals", icon: "sparkles" },
  { id: "trends", icon: "trending-up" },
  { id: "context", icon: "layers" },
  { id: "watchlists", icon: "puzzle" },
  { id: "alerts", icon: "shield" },
];
