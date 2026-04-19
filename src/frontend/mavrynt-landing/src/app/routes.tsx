import { lazy, Suspense, type ReactNode } from "react";
import { type RouteObject } from "react-router";
import { MarketingLayout } from "../layouts/MarketingLayout.tsx";

/**
 * Route tree for the marketing SPA.
 *
 * Pages are `React.lazy` to keep the landing bundle lean — only the
 * shell + the active route ship on first paint. Adding a new page
 * means adding one entry here and nothing else.
 */
const HomePage = lazy(() => import("../pages/HomePage.tsx"));
const FeaturesPage = lazy(() => import("../pages/FeaturesPage.tsx"));
const PricingPage = lazy(() => import("../pages/PricingPage.tsx"));
const ContactPage = lazy(() => import("../pages/ContactPage.tsx"));
const NotFoundPage = lazy(() => import("../pages/NotFoundPage.tsx"));
const PrivacyPage = lazy(() => import("../pages/legal/PrivacyPage.tsx"));
const TermsPage = lazy(() => import("../pages/legal/TermsPage.tsx"));

/**
 * Thin wrapper that keeps the chrome visible while the page chunk
 * resolves. Intentionally minimal — a proper skeleton shows up in Phase 4.
 */
const RouteFallback = (): ReactNode => (
  <div className="p-8 text-sm text-fg-muted" aria-busy="true" aria-live="polite">
    …
  </div>
);

const withSuspense = (element: ReactNode): ReactNode => (
  <Suspense fallback={<RouteFallback />}>{element}</Suspense>
);

export const routes: ReadonlyArray<RouteObject> = [
  {
    path: "/",
    element: <MarketingLayout />,
    children: [
      { index: true, element: withSuspense(<HomePage />) },
      { path: "features", element: withSuspense(<FeaturesPage />) },
      { path: "pricing", element: withSuspense(<PricingPage />) },
      { path: "contact", element: withSuspense(<ContactPage />) },
      {
        path: "legal",
        children: [
          { path: "privacy", element: withSuspense(<PrivacyPage />) },
          { path: "terms", element: withSuspense(<TermsPage />) },
        ],
      },
      { path: "*", element: withSuspense(<NotFoundPage />) },
    ],
  },
];
