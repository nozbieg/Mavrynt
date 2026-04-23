import { Outlet } from "react-router";
import { useTrackPageView } from "../lib/analytics/useTrackPageView.ts";
import { AdminNav } from "./components/AdminNav.tsx";
import { AdminFooter } from "./components/AdminFooter.tsx";
import { SkipLink } from "./components/SkipLink.tsx";

/**
 * AdminLayout — chrome shared by every route in `mavrynt-admin`.
 *
 * Mirrors `AppLayout` (mavrynt-web) and `MarketingLayout` (landing) so
 * users moving between the three SPAs see a consistent shell.
 *
 * `useTrackPageView` sits here (inside Router context) so every route
 * transition emits exactly one pageview without per-page glue.
 */
export const AdminLayout = () => {
  useTrackPageView();

  return (
    <div className="flex min-h-screen flex-col bg-bg text-fg">
      <SkipLink />
      <AdminNav />
      <main id="main" tabIndex={-1} className="flex-1 focus:outline-none">
        <Outlet />
      </main>
      <AdminFooter />
    </div>
  );
};
