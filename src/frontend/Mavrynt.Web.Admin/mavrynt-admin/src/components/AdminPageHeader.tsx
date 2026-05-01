import type { ReactNode } from "react";

interface AdminPageHeaderProps {
  readonly title: string;
  readonly description?: string;
  readonly actions?: ReactNode;
}

export const AdminPageHeader = ({
  title,
  description,
  actions,
}: AdminPageHeaderProps) => (
  <div className="mb-8 flex items-start justify-between gap-6 border-b border-border pb-6">
    <div>
      <h1 className="text-2xl font-bold tracking-tight text-fg">{title}</h1>
      {description && (
        <p className="mt-2 text-sm leading-relaxed text-fg-muted">{description}</p>
      )}
    </div>
    {actions && (
      <div className="flex shrink-0 items-center gap-3">{actions}</div>
    )}
  </div>
);
