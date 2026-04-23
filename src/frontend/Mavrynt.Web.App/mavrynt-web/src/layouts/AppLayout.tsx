import { Outlet } from "react-router";
import { useTrackPageView } from "../lib/analytics/useTrackPageView.ts";
import { AppNav } from "./components/AppNav.tsx";
import { AppFooter } from "./components/AppFooter.tsx";
import { SkipLink } from "./components/SkipLink.tsx";

/**
 * AppLayout — chrome shared by every route in `mavrynt-web`.
 *
 * Mirrors `MarketingLayout` in `mavrynt-landing` so users moving between
 * the two SPAs see a consistent shell (nav bar, skip link, main region,
 * footer).
 *
 * `useTrackPageView` lives here (inside Router context) so every route
 * transition emits exactly one pageview event without per-page glue.
 */
export const AppLayout = () => {
  useTrackPageView();

  return (
    <div className="flex min-h-screen flex-col bg-bg text-fg">
      <SkipLink />
      <AppNav />
      <main id="main" tabIndex={-1} className="flex-1 focus:outline-none">
        <Outlet />
      </main>
      <AppFooter />
    </div>
  );
};
