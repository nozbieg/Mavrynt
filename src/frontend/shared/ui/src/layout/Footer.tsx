import type { ReactNode } from "react";
import { Container } from "../primitives/Container.tsx";
import { Logo } from "../primitives/Logo.tsx";
import { cn } from "../primitives/cn.ts";

/**
 * Footer — shared bottom layout shell.
 *
 * Like Navbar, the footer is routing-agnostic. Apps inject their own
 * column groups and bottom bar content (legal links, language switcher,
 * social icons, etc.) so the shell stays DRY but each app keeps control
 * of its own copy and routes.
 */
export interface FooterColumn {
  readonly id: string;
  readonly title: ReactNode;
  readonly items: ReactNode;
}

export interface FooterProps {
  readonly brand?: ReactNode;
  readonly tagline?: ReactNode;
  readonly columns?: ReadonlyArray<FooterColumn>;
  readonly bottom?: ReactNode;
  readonly className?: string;
  readonly ariaLabel?: string;
}

export const Footer = ({
  brand,
  tagline,
  columns = [],
  bottom,
  className,
  ariaLabel = "Site footer",
}: FooterProps) => (
  <footer
    aria-label={ariaLabel}
    className={cn(
      "mt-auto w-full border-t border-border bg-bg-subtle text-fg",
      className,
    )}
  >
    <Container>
      <div className="grid grid-cols-1 gap-10 py-12 md:grid-cols-[2fr_repeat(auto-fit,minmax(140px,1fr))]">
        <div className="flex flex-col gap-3">
          {brand ?? <Logo />}
          {tagline !== undefined && (
            <p className="max-w-sm text-sm text-fg-muted">{tagline}</p>
          )}
        </div>
        {columns.map((column) => (
          <div key={column.id} className="flex flex-col gap-3">
            <h3 className="text-sm font-semibold uppercase tracking-wide text-fg-muted">
              {column.title}
            </h3>
            <ul className="m-0 flex list-none flex-col gap-2 p-0 text-sm">
              {column.items}
            </ul>
          </div>
        ))}
      </div>
      {bottom !== undefined && (
        <div className="flex flex-col items-start gap-3 border-t border-border py-6 text-sm text-fg-muted md:flex-row md:items-center md:justify-between">
          {bottom}
        </div>
      )}
    </Container>
  </footer>
);
