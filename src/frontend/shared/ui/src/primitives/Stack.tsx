import type { HTMLAttributes } from "react";
import { cn } from "./cn.ts";

/**
 * Stack — flex layout primitive (vertical or horizontal). Encapsulates
 * the recurring `flex flex-col gap-*` / `flex flex-row gap-*` pattern
 * so app code stays declarative.
 */
export type StackDirection = "row" | "column";
export type StackAlign = "start" | "center" | "end" | "stretch" | "baseline";
export type StackJustify =
  | "start"
  | "center"
  | "end"
  | "between"
  | "around"
  | "evenly";
export type StackGap = 0 | 1 | 2 | 3 | 4 | 5 | 6 | 8 | 10 | 12;

export interface StackProps extends HTMLAttributes<HTMLDivElement> {
  readonly direction?: StackDirection;
  readonly align?: StackAlign;
  readonly justify?: StackJustify;
  readonly gap?: StackGap;
  readonly wrap?: boolean;
}

const directionClass: Record<StackDirection, string> = {
  row: "flex-row",
  column: "flex-col",
};

const alignClass: Record<StackAlign, string> = {
  start: "items-start",
  center: "items-center",
  end: "items-end",
  stretch: "items-stretch",
  baseline: "items-baseline",
};

const justifyClass: Record<StackJustify, string> = {
  start: "justify-start",
  center: "justify-center",
  end: "justify-end",
  between: "justify-between",
  around: "justify-around",
  evenly: "justify-evenly",
};

const gapClass: Record<StackGap, string> = {
  0: "gap-0",
  1: "gap-1",
  2: "gap-2",
  3: "gap-3",
  4: "gap-4",
  5: "gap-5",
  6: "gap-6",
  8: "gap-8",
  10: "gap-10",
  12: "gap-12",
};

export const Stack = ({
  direction = "column",
  align = "stretch",
  justify = "start",
  gap = 4,
  wrap = false,
  className,
  ...rest
}: StackProps) => (
  <div
    className={cn(
      "flex",
      directionClass[direction],
      alignClass[align],
      justifyClass[justify],
      gapClass[gap],
      wrap && "flex-wrap",
      className,
    )}
    {...rest}
  />
);
