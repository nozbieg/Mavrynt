/**
 * Placeholder "logo cloud" — fictional customers rendered as wordmarks.
 * Phase 4+ swaps these for real partner logos served from `/public/logos`.
 *
 * Kept as plain data so the component stays declarative.
 */
export interface LogoEntry {
  readonly id: string;
  readonly label: string;
}

export const logos: ReadonlyArray<LogoEntry> = [
  { id: "finstack", label: "FinStack" },
  { id: "orbit", label: "Orbit" },
  { id: "greenline", label: "Greenline" },
  { id: "northwind", label: "Northwind" },
  { id: "halcyon", label: "Halcyon" },
  { id: "lumen", label: "Lumen Labs" },
];
