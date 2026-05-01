import type { ReactNode } from "react";
import { cn } from "@mavrynt/ui";

interface AdminCardProps {
  readonly title?: string;
  readonly children: ReactNode;
  readonly className?: string;
  readonly actions?: ReactNode;
}

export const AdminCard = ({ title, children, className, actions }: AdminCardProps) => (
  <div
    className={cn(
      "rounded-lg border border-border bg-bg p-5",
      className,
    )}
  >
    {(title || actions) && (
      <div className="mb-4 flex items-center justify-between gap-4">
        {title && (
          <h2 className="text-base font-semibold text-fg">{title}</h2>
        )}
        {actions && <div className="flex shrink-0 gap-2">{actions}</div>}
      </div>
    )}
    {children}
  </div>
);
