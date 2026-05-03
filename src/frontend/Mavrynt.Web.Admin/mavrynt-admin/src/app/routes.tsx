import { lazy, Suspense, type ReactNode } from "react";
import { type RouteObject } from "react-router";
import { AdminLayout } from "../layouts/AdminLayout.tsx";
import { RequireAdminAuth } from "../lib/auth/RequireAdminAuth.tsx";

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
const ChangePasswordPage = lazy(() => import("../pages/ChangePasswordPage.tsx"));
const DashboardPage = lazy(() => import("../pages/DashboardPage.tsx"));
const UsersPage = lazy(() => import("../pages/UsersPage.tsx"));
const FeatureFlagsPage = lazy(() => import("../pages/FeatureFlagsPage.tsx"));
const SmtpSettingsPage = lazy(() => import("../pages/SmtpSettingsPage.tsx"));
const SettingsPage = lazy(() => import("../pages/SettingsPage.tsx"));
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

const protect = (element: ReactNode): ReactNode => (
  <RequireAdminAuth>{element}</RequireAdminAuth>
);

export const routes: ReadonlyArray<RouteObject> = [
  {
    path: "/",
    element: <AdminLayout />,
    children: [
      { index: true, element: withSuspense(<HomePage />) },
      { path: "login", element: withSuspense(<LoginPage />) },
      { path: "register", element: withSuspense(<RegisterPage />) },
      { path: "change-password", element: withSuspense(<ChangePasswordPage />) },
      { path: "dashboard", element: withSuspense(protect(<DashboardPage />)) },
      { path: "users", element: withSuspense(protect(<UsersPage />)) },
      { path: "feature-flags", element: withSuspense(protect(<FeatureFlagsPage />)) },
      { path: "smtp-settings", element: withSuspense(protect(<SmtpSettingsPage />)) },
      { path: "settings", element: withSuspense(protect(<SettingsPage />)) },
      { path: "*", element: withSuspense(<NotFoundPage />) },
    ],
  },
];
