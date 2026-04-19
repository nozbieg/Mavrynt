import { forwardRef, type AnchorHTMLAttributes, type ReactNode } from "react";
import { cn } from "./cn.ts";

/**
 * Link — semantic <a> with consistent styling and safe defaults for
 * external links. Routing libraries (React Router) should wrap their
 * own `<RouterLink>` and pass `asChild`-style children when needed.
 */
export type LinkVariant = "inline" | "subtle" | "muted";

export interface LinkProps extends AnchorHTMLAttributes<HTMLAnchorElement> {
  readonly variant?: LinkVariant;
  readonly external?: boolean;
  readonly children: ReactNode;
}

const variantClasses: Record<LinkVariant, string> = {
  inline:
    "text-primary underline underline-offset-4 decoration-primary/40 hover:decoration-primary",
  subtle: "text-fg hover:text-primary transition-colors",
  muted: "text-fg-muted hover:text-fg transition-colors",
};

const focusClasses =
  "rounded-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg";

export interface LinkStyleOptions {
  readonly variant?: LinkVariant;
}

/**
 * Shared link className recipe — exposed so router-aware links (React
 * Router `<Link>`, Next `<Link>`, etc.) can reuse the exact same look as
 * the headless `<Link>` primitive without duplicating the class lists.
 */
export const linkStyles = ({
  variant = "inline",
}: LinkStyleOptions = {}): string => cn(variantClasses[variant], focusClasses);

export const Link = forwardRef<HTMLAnchorElement, LinkProps>(
  ({ variant = "inline", external, className, children, ...rest }, ref) => (
    <a
      ref={ref}
      className={cn(linkStyles({ variant }), className)}
      {...(external
        ? { target: "_blank", rel: "noopener noreferrer" }
        : undefined)}
      {...rest}
    >
      {children}
    </a>
  ),
);

Link.displayName = "Link";
