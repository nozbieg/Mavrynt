import { lazy, Suspense, type ReactNode } from "react";
import { type RouteObject } from "react-router";
import { AppLayout } from "../layouts/AppLayout.tsx";

/**
 * Route tree for `mavrynt-web`.
 *
 * Lazy-loaded pages keep the initial shell tiny — only the route the
 * user lands on ships. Adding a page = adding one entry here.
 */
const HomePage = lazy(() => import("../pages/HomePage.tsx"));
const LoginPage = lazy(() => import("../pages/LoginPage.tsx"));
const RegisterPage = lazy(() => import("../pages/RegisterPage.tsx"));
const NotFoundPage = lazy(() => import("../pages/NotFoundPage.tsx"));

/** Minimal route fallback — a skeleton UI ships later. */
const RouteFallback = (): ReactNode => (
  <div
    className="p-8 text-sm text-fg-muted"
    aria-busy="true"
    aria-live="polite"
  >
    …
  </div>
);

const withSuspense = (element: ReactNode): ReactNode => (
  <Suspense fallback={<RouteFallback />}>{element}</Suspense>
);

export const routes: ReadonlyArray<RouteObject> = [
  {
    path: "/",
    element: <AppLayout />,
    children: [
      { index: true, element: withSuspense(<HomePage />) },
      { path: "login", element: withSuspense(<LoginPage />) },
      { path: "register", element: withSuspense(<RegisterPage />) },
      { path: "*", element: withSuspense(<NotFoundPage />) },
    ],
  },
];
