import type { SVGProps } from "react";

/**
 * Icon — tiny inline-SVG icon set, hand-picked for the marketing sections.
 *
 * Rationale:
 *  - Zero runtime deps (no lucide-react / heroicons bundle).
 *  - All paths stroked at 1.5, sharing the same 24x24 viewbox, so icons
 *    stay visually consistent.
 *  - A single component + discriminated `name` keeps call sites simple
 *    and searchable.
 *
 * Adding a new icon: add an entry to `PATHS` and extend `IconName`.
 */
export type IconName =
  | "puzzle"
  | "code"
  | "activity"
  | "shield"
  | "trending-up"
  | "plug"
  | "sparkles"
  | "layers"
  | "rocket"
  | "check"
  | "chevron-down"
  | "quote"
  | "mail"
  | "arrow-right";

type IconPath = { readonly d: string; readonly fill?: "none" | "currentColor" };

const PATHS: Record<IconName, ReadonlyArray<IconPath>> = {
  puzzle: [
    {
      d: "M10 3h4v3a2 2 0 1 0 4 0h3v4a2 2 0 1 1 0 4v4h-4a2 2 0 1 0-4 0H6v-4a2 2 0 1 1 0-4V6h4v-3Z",
    },
  ],
  code: [{ d: "m8 8-4 4 4 4M16 8l4 4-4 4M14 4l-4 16" }],
  activity: [{ d: "M3 12h4l3-8 4 16 3-8h4" }],
  shield: [
    {
      d: "M12 3 4 6v5c0 5 3.4 9 8 10 4.6-1 8-5 8-10V6l-8-3Z",
    },
  ],
  "trending-up": [{ d: "M3 17 9 11l4 4 8-8M14 7h7v7" }],
  plug: [
    {
      d: "M9 3v6M15 3v6M7 9h10v4a5 5 0 1 1-10 0V9ZM12 18v3",
    },
  ],
  sparkles: [{ d: "M12 3v6M12 15v6M3 12h6M15 12h6M6 6l3 3M15 15l3 3M18 6l-3 3M9 15l-3 3" }],
  layers: [
    {
      d: "m12 3 9 5-9 5-9-5 9-5Zm-9 9 9 5 9-5M3 17l9 5 9-5",
    },
  ],
  rocket: [
    {
      d: "M5 19c0-4 2-7 6-9 4 2 6 5 6 9M12 3c3 2 5 5 5 9a6 6 0 0 1-5 6 6 6 0 0 1-5-6c0-4 2-7 5-9ZM12 10a1 1 0 1 0 0 2 1 1 0 0 0 0-2Z",
    },
  ],
  check: [{ d: "m5 12 5 5 9-10" }],
  "chevron-down": [{ d: "m6 9 6 6 6-6" }],
  quote: [
    {
      d: "M7 7h4v4a4 4 0 0 1-4 4V7Zm10 0h-4v4a4 4 0 0 0 4 4V7Z",
    },
  ],
  mail: [
    {
      d: "M4 6h16v12H4V6Zm0 0 8 7 8-7",
    },
  ],
  "arrow-right": [{ d: "M5 12h14M13 5l7 7-7 7" }],
};

export interface IconProps extends Omit<SVGProps<SVGSVGElement>, "children"> {
  readonly name: IconName;
  readonly size?: number;
}

export const Icon = ({
  name,
  size = 24,
  strokeWidth = 1.5,
  className,
  ...rest
}: IconProps) => (
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width={size}
    height={size}
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    strokeWidth={strokeWidth}
    strokeLinecap="round"
    strokeLinejoin="round"
    aria-hidden="true"
    className={className}
    {...rest}
  >
    {PATHS[name].map((p, i) => (
      <path key={i} d={p.d} fill={p.fill ?? "none"} />
    ))}
  </svg>
);
