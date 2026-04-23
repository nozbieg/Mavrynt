import type { ReactNode } from "react";
import { cn } from "@mavrynt/ui";

/**
 * SectionHeader — DRY eyebrow + title + subtitle block used by every
 * marketing feature. Keeps spacing / typography consistent without
 * pushing each feature to reinvent it.
 */
export interface SectionHeaderProps {
  readonly eyebrow?: ReactNode;
  readonly title: ReactNode;
  readonly subtitle?: ReactNode;
  readonly align?: "start" | "center";
  readonly className?: string;
}

export const SectionHeader = ({
  eyebrow,
  title,
  subtitle,
  align = "start",
  className,
}: SectionHeaderProps) => (
  <div
    className={cn(
      "flex max-w-2xl flex-col gap-3",
      align === "center" && "mx-auto items-center text-center",
      className,
    )}
  >
    {eyebrow !== undefined && eyebrow !== null && (
      <span className="text-xs font-semibold uppercase tracking-wide text-primary">
        {eyebrow}
      </span>
    )}
    <h2 className="font-display text-3xl font-semibold tracking-tight text-fg sm:text-4xl">
      {title}
    </h2>
    {subtitle !== undefined && subtitle !== null && (
      <p className="text-lg text-fg-muted">{subtitle}</p>
    )}
  </div>
);
