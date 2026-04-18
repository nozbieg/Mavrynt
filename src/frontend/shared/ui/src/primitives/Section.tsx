import type { HTMLAttributes, ReactNode } from "react";
import { cn } from "./cn.ts";
import { Container, type ContainerSize } from "./Container.tsx";

/**
 * Section — vertical rhythm primitive used by every page.
 * Wraps children in a width-constrained Container by default; opt out
 * with `bleed` when the section needs to break the grid.
 */
export type SectionTone = "default" | "subtle" | "muted" | "inverted";
export type SectionSpacing = "sm" | "md" | "lg";

export interface SectionProps extends HTMLAttributes<HTMLElement> {
  readonly tone?: SectionTone;
  readonly spacing?: SectionSpacing;
  readonly container?: ContainerSize | "none";
  readonly children: ReactNode;
}

const toneClass: Record<SectionTone, string> = {
  default: "bg-bg text-fg",
  subtle: "bg-bg-subtle text-fg",
  muted: "bg-bg-muted text-fg",
  inverted: "bg-fg text-bg",
};

const spacingClass: Record<SectionSpacing, string> = {
  sm: "py-section-sm",
  md: "py-section",
  lg: "py-section-lg",
};

export const Section = ({
  tone = "default",
  spacing = "md",
  container = "xl",
  className,
  children,
  ...rest
}: SectionProps) => {
  const inner =
    container === "none" ? children : <Container size={container}>{children}</Container>;

  return (
    <section
      className={cn(toneClass[tone], spacingClass[spacing], className)}
      {...rest}
    >
      {inner}
    </section>
  );
};
