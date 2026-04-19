/**
 * Placeholder "logo cloud" — neutral wordmarks used as social proof
 * while real partner assets aren't available. The strip signals the
 * kind of audience Mavrynt targets (desks, research groups, prop
 * shops) without making claims about specific customers.
 *
 * Phase 4+ swaps these for real partner logos served from `/public/logos`.
 */
export interface LogoEntry {
  readonly id: string;
  readonly label: string;
}

export const logos: ReadonlyArray<LogoEntry> = [
  { id: "arculus", label: "Arculus Capital" },
  { id: "northquant", label: "NorthQuant" },
  { id: "varenne", label: "Varenne Research" },
  { id: "orbis-fx", label: "Orbis FX" },
  { id: "linden", label: "Linden Markets" },
  { id: "meridian", label: "Meridian Trading" },
];
