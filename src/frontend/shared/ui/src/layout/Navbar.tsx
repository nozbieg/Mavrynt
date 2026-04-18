import type { ReactNode } from "react";
import { Container } from "../primitives/Container.tsx";
import { Stack } from "../primitives/Stack.tsx";
import { Logo } from "../primitives/Logo.tsx";
import { cn } from "../primitives/cn.ts";

/**
 * Navbar — shared top navigation shell.
 *
 * Apps inject their own `links`, `actions`, and optionally a custom
 * `brand` slot, so each SPA stays in control of routing while sharing
 * the same shell, height, and spacing.
 *
 * The component is intentionally **routing-agnostic**: `links` are
 * already-rendered ReactNodes (so apps can use React Router `<Link>`,
 * a plain `<a>`, or anything else).
 */
export interface NavbarProps {
  readonly brand?: ReactNode;
  readonly links?: ReactNode;
  readonly actions?: ReactNode;
  readonly sticky?: boolean;
  readonly className?: string;
  readonly ariaLabel?: string;
}

export const Navbar = ({
  brand,
  links,
  actions,
  sticky = true,
  className,
  ariaLabel = "Main navigation",
}: NavbarProps) => (
  <header
    className={cn(
      "z-40 w-full border-b border-border bg-bg/85 backdrop-blur-md",
      sticky && "sticky top-0",
      className,
    )}
  >
    <Container>
      <nav
        aria-label={ariaLabel}
        className="flex h-16 items-center justify-between gap-6"
      >
        <div className="flex items-center gap-8">
          <a
            href="/"
            className="rounded-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg"
          >
            {brand ?? <Logo />}
          </a>
          {links !== undefined && (
            <ul className="m-0 hidden list-none gap-6 p-0 md:flex md:items-center">
              {links}
            </ul>
          )}
        </div>
        {actions !== undefined && (
          <Stack direction="row" align="center" gap={3}>
            {actions}
          </Stack>
        )}
      </nav>
    </Container>
  </header>
);
