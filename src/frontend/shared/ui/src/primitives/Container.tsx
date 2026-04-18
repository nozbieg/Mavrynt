import type { HTMLAttributes } from "react";
import { cn } from "./cn.ts";

/**
 * Container — width-constrained, horizontally centered, responsive padding.
 * Used by Section, Navbar, and Footer to keep horizontal alignment
 * consistent across all SPAs.
 */
export type ContainerSize = "sm" | "md" | "lg" | "xl" | "full";

export interface ContainerProps extends HTMLAttributes<HTMLDivElement> {
  readonly size?: ContainerSize;
}

const sizeClass: Record<ContainerSize, string> = {
  sm: "max-w-3xl",
  md: "max-w-5xl",
  lg: "max-w-6xl",
  xl: "max-w-7xl",
  full: "max-w-none",
};

export const Container = ({
  size = "xl",
  className,
  ...rest
}: ContainerProps) => (
  <div
    className={cn(
      "mx-auto w-full px-4 sm:px-6 lg:px-8",
      sizeClass[size],
      className,
    )}
    {...rest}
  />
);
