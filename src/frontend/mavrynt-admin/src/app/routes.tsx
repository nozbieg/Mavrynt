import { lazy, Suspense, type ReactNode } from "react";
import { type RouteObject } from "react-router";
import { AdminLayout } from "../layouts/AdminLayout.tsx";

/**
 * Route tree for `mavrynt-admin`.
 *
 * Mirrors `mavrynt-web`'s route shape so the two auth-facing SPAs stay
 * aligned. `/register` is always mounted for route parity — the page
 * itself renders a friendly "registration disabled" banner when the
 * `admin.register.enabled` feature flag is off.
 */
const HomePage = lazy(() => import("../pages/HomePage.tsx"));
const LoginPage = lazy(() => import("../pages/LoginPage.tsx"));
const RegisterPage = lazy(() => import("../pages/RegisterPage.tsx"));
const NotFoundPage = lazy(() => import("../pages/NotFoundPage.tsx"));

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
    element: <AdminLayout />,
    children: [
      { index: true, element: withSuspense(<HomePage />) },
      { path: "login", element: withSuspense(<LoginPage />) },
      { path: "register", element: withSuspense(<RegisterPage />) },
      { path: "*", element: withSuspense(<NotFoundPage />) },
    ],
  },
];
