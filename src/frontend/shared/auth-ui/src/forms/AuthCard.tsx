import type { ReactNode } from "react";
import { cn } from "@mavrynt/ui";

/**
 * Centred card layout used by `LoginForm` and `RegisterForm`.
 *
 * Pure presentational — no auth state, no i18n. Apps drop it inside
 * their own page shell (with their own nav/footer); the card just owns
 * width, padding, and the header/content/footer rhythm so the two
 * forms stay visually consistent across web and admin.
 */
export interface AuthCardProps {
  readonly title: ReactNode;
  readonly subtitle?: ReactNode;
  readonly eyebrow?: ReactNode;
  readonly footer?: ReactNode;
  readonly children: ReactNode;
  readonly className?: string;
}

export const AuthCard = ({
  title,
  subtitle,
  eyebrow,
  footer,
  children,
  className,
}: AuthCardProps) => (
  <section
    className={cn(
      "mx-auto flex w-full max-w-md flex-col gap-6 rounded-2xl border border-border bg-bg-muted/40 p-8 shadow-sm backdrop-blur-sm",
      className,
    )}
  >
    <header className="flex flex-col gap-2 text-center">
      {eyebrow !== undefined && (
        <span className="text-xs font-semibold uppercase tracking-wider text-primary">
          {eyebrow}
        </span>
      )}
      <h1 className="font-display text-2xl font-semibold text-fg">{title}</h1>
      {subtitle !== undefined && (
        <p className="text-sm text-fg-muted">{subtitle}</p>
      )}
    </header>
    <div className="flex flex-col gap-5">{children}</div>
    {footer !== undefined && (
      <footer className="flex flex-col items-center gap-2 border-t border-border/60 pt-5 text-sm text-fg-muted">
        {footer}
      </footer>
    )}
  </section>
);
