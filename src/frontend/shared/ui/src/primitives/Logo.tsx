import type { HTMLAttributes } from "react";
import { cn } from "./cn.ts";

/**
 * Logo — placeholder wordmark + glyph. Replace the inline SVG when the
 * brand asset is finalized (Phase 3 / brand). Designed so consumers can
 * pass `className` to color or size it via Tailwind utilities.
 */
export interface LogoProps extends HTMLAttributes<HTMLSpanElement> {
  readonly variant?: "full" | "mark";
  readonly label?: string;
}

export const Logo = ({
  variant = "full",
  label = "Mavrynt",
  className,
  ...rest
}: LogoProps) => (
  <span
    className={cn(
      "inline-flex items-center gap-2 font-display text-fg",
      className,
    )}
    aria-label={label}
    {...rest}
  >
    <svg
      viewBox="0 0 24 24"
      width="24"
      height="24"
      role="presentation"
      aria-hidden="true"
      className="text-primary"
    >
      <path
        d="M4 20 L12 4 L20 20 L16 20 L12 12 L8 20 Z"
        fill="currentColor"
      />
    </svg>
    {variant === "full" && (
      <span className="text-lg font-semibold tracking-tight">{label}</span>
    )}
  </span>
);
