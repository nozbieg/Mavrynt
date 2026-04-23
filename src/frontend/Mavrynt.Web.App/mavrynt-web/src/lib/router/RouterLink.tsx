import { forwardRef, type ReactNode } from "react";
import { Link as RRLink, type LinkProps as RRLinkProps } from "react-router";
import { cn, linkStyles, type LinkVariant } from "@mavrynt/ui";

/**
 * RouterLink — React-Router `<Link>` wearing the shared `@mavrynt/ui`
 * link styles. Mirrors the `RouterLink` in `mavrynt-landing` so
 * cross-app navigation looks identical on both sides.
 */
export interface RouterLinkProps extends Omit<RRLinkProps, "children"> {
  readonly variant?: LinkVariant;
  readonly children: ReactNode;
}

export const RouterLink = forwardRef<HTMLAnchorElement, RouterLinkProps>(
  ({ variant = "inline", className, children, ...rest }, ref) => (
    <RRLink
      ref={ref}
      className={cn(linkStyles({ variant }), className)}
      {...rest}
    >
      {children}
    </RRLink>
  ),
);

RouterLink.displayName = "RouterLink";
