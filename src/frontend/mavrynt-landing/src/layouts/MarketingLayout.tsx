import { Outlet } from "react-router";
import { useTrackPageView } from "../lib/analytics/useTrackPageView.ts";
import { MarketingFooter } from "./components/MarketingFooter.tsx";
import { MarketingNav } from "./components/MarketingNav.tsx";
import { SkipLink } from "./components/SkipLink.tsx";

/**
 * MarketingLayout — site chrome shared by every marketing route.
 *
 * Wraps the routed page in:
 *  - `SkipLink` (a11y bypass)
 *  - `MarketingNav`
 *  - `<main id="main">` landmark (focusable for skip-link target)
 *  - `MarketingFooter`
 *
 * `useTrackPageView` sits here (inside Router context) so every route
 * transition gets one pageview event without per-page boilerplate.
 */
export const MarketingLayout = () => {
  useTrackPageView();

  return (
    <div className="flex min-h-screen flex-col bg-bg text-fg">
      <SkipLink />
      <MarketingNav />
      <main id="main" tabIndex={-1} className="flex-1 focus:outline-none">
        <Outlet />
      </main>
      <MarketingFooter />
    </div>
  );
};
