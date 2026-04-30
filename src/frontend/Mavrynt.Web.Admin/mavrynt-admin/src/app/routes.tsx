import { lazy, Suspense, type ReactNode } from "react";
import { type RouteObject } from "react-router";
import { AdminLayout } from "../layouts/AdminLayout";
import { AdminShellLayout } from "../layouts/AdminShellLayout";
import { RequireAdminAuth } from "../lib/auth/RequireAdminAuth";

const HomePage = lazy(() => import("../pages/HomePage"));
const LoginPage = lazy(() => import("../pages/LoginPage"));
const RegisterPage = lazy(() => import("../pages/RegisterPage"));
const ChangePasswordPage = lazy(() => import("../pages/ChangePasswordPage"));
const DashboardPage = lazy(() => import("../pages/DashboardPage"));
const FeatureFlagsPage = lazy(() => import("../pages/FeatureFlagsPage"));
const SmtpSettingsPage = lazy(() => import("../pages/SmtpSettingsPage"));
const UsersPage = lazy(() => import("../pages/UsersPage"));
const NotFoundPage = lazy(() => import("../pages/NotFoundPage"));

const withSuspense = (element: ReactNode): ReactNode => (
  <Suspense fallback={<div className="p-8">…</div>}>{element}</Suspense>
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
      {
        element: (
          <RequireAdminAuth>
            <AdminShellLayout />
          </RequireAdminAuth>
        ),
        children: [
          { path: "dashboard", element: withSuspense(<DashboardPage />) },
          { path: "feature-flags", element: withSuspense(<FeatureFlagsPage />) },
          { path: "smtp-settings", element: withSuspense(<SmtpSettingsPage />) },
          { path: "users", element: withSuspense(<UsersPage />) },
        ],
      },
      { path: "*", element: withSuspense(<NotFoundPage />) },
    ],
  },
];
