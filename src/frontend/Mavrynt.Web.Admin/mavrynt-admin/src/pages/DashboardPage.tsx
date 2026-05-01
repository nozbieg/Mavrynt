import { useEffect, useState } from "react";
import { Link } from "react-router";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, type AdminProfile } from "../lib/api/adminApi.ts";
import { AdminCard } from "../components/AdminCard.tsx";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminState } from "../components/AdminState.tsx";
import { cn } from "@mavrynt/ui";

type LoadState = "loading" | "ready" | "error";

const QUICK_ACTIONS = [
  { to: "/users", label: "Users", description: "Manage user accounts and roles" },
  { to: "/feature-flags", label: "Feature Flags", description: "Toggle feature availability" },
  { to: "/smtp-settings", label: "SMTP Settings", description: "Configure email delivery" },
  { to: "/settings", label: "Settings", description: "System configuration" },
] as const;

const DashboardPage = () => {
  const [state, setState] = useState<LoadState>("loading");
  const [profile, setProfile] = useState<AdminProfile | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    void (async () => {
      try {
        const data = await adminApi.getCurrentAdmin();
        setProfile(data);
        setState("ready");
      } catch {
        setError("Unable to load your admin profile. Please try refreshing the page.");
        setState("error");
      }
    })();
  }, []);

  return (
    <>
      <Seo title="Dashboard — Mavrynt Admin" description="Admin dashboard" />

      {/* Page header with extra bottom spacing */}
      <div className="mb-8">
        <AdminPageHeader
          title="Dashboard"
          description="Welcome to Mavrynt Admin. Here is an overview of your account and system status."
        />
      </div>

      {state === "loading" && <AdminState type="loading" message="Loading your profile…" />}
      {state === "error" && <AdminState type="error" message={error ?? undefined} />}

      {state === "ready" && profile && (
        <div className="grid gap-6 md:grid-cols-2">

          {/* Account overview — full width */}
          <AdminCard title="Account overview" className="md:col-span-2">
            <dl className="mt-2 grid gap-6 text-sm sm:grid-cols-2 lg:grid-cols-4">
              <div className="space-y-1">
                <dt className="text-xs font-medium uppercase tracking-wide text-fg-muted">
                  Email
                </dt>
                <dd className="text-base font-medium text-fg">{profile.email}</dd>
              </div>
              <div className="space-y-1">
                <dt className="text-xs font-medium uppercase tracking-wide text-fg-muted">
                  Display name
                </dt>
                <dd className="text-base font-medium text-fg">
                  {profile.displayName ?? <span className="text-fg-muted italic">Not set</span>}
                </dd>
              </div>
              <div className="space-y-1">
                <dt className="text-xs font-medium uppercase tracking-wide text-fg-muted">
                  Role
                </dt>
                <dd className="text-base font-medium capitalize text-fg">{profile.role}</dd>
              </div>
              <div className="space-y-1">
                <dt className="text-xs font-medium uppercase tracking-wide text-fg-muted">
                  Account status
                </dt>
                <dd className="text-base font-medium text-fg">{profile.status}</dd>
              </div>
            </dl>
          </AdminCard>

          {/* System status */}
          <AdminCard title="System status">
            <div className="mt-2 flex items-center gap-3">
              {/* aria-label on the status indicator so screen readers get context */}
              <span
                className="h-3 w-3 shrink-0 rounded-full bg-green-500"
                role="img"
                aria-label="Operational"
              />
              <span className="text-sm text-fg">Admin panel is operational</span>
            </div>
          </AdminCard>

          {/* Security */}
          <AdminCard title="Security">
            <div className="mt-2 space-y-4">
              <div className="flex items-center justify-between gap-4 rounded-md bg-bg-subtle px-4 py-3">
                <span className="text-sm text-fg">Password change required</span>
                <span
                  className={cn(
                    "rounded-full px-3 py-1 text-xs font-semibold",
                    profile.requiresPasswordChange
                      ? "bg-amber-500/15 text-amber-700 dark:text-amber-400"
                      : "bg-green-500/15 text-green-700 dark:text-green-400",
                  )}
                  aria-live="polite"
                >
                  {profile.requiresPasswordChange ? "Yes" : "No"}
                </span>
              </div>
            </div>
          </AdminCard>

          {/* Quick actions — full width */}
          <section aria-label="Quick actions" className="md:col-span-2">
            <h2 className="mb-4 text-base font-semibold text-fg">Quick actions</h2>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
              {QUICK_ACTIONS.map(({ to, label, description }) => (
                <Link
                  key={to}
                  to={to}
                  className={cn(
                    "group flex flex-col gap-1 rounded-lg border border-border bg-bg p-5",
                    "transition-colors hover:border-border hover:bg-bg-subtle",
                    // WCAG 2.2 — visible focus indicator
                    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg",
                  )}
                >
                  <span className="text-sm font-semibold text-fg">{label}</span>
                  <span className="text-xs leading-relaxed text-fg-muted">{description}</span>
                </Link>
              ))}
            </div>
          </section>

        </div>
      )}
    </>
  );
};

export default DashboardPage;
