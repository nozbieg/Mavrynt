import { Link } from "react-router";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminCard } from "../components/AdminCard.tsx";
import { useAdminSession } from "../lib/auth/AdminSessionProvider.tsx";

const SettingsPage = () => {
  const { session } = useAdminSession();

  return (
    <>
      <Seo title="Settings — Mavrynt Admin" description="Admin settings" />
      <AdminPageHeader
        title="Settings"
        description="System configuration and administration options."
      />

      <div className="grid gap-5 md:grid-cols-2">
        {session && (
          <AdminCard title="Session" className="md:col-span-2">
            <dl className="grid gap-3 text-sm sm:grid-cols-3">
              <div>
                <dt className="text-fg-muted">Email</dt>
                <dd className="mt-1 font-medium text-fg">{session.user.email}</dd>
              </div>
              <div>
                <dt className="text-fg-muted">Name</dt>
                <dd className="mt-1 font-medium text-fg">
                  {session.user.name ?? (
                    <span className="italic text-fg-muted">Not set</span>
                  )}
                </dd>
              </div>
              <div>
                <dt className="text-fg-muted">Roles</dt>
                <dd className="mt-1 font-medium capitalize text-fg">
                  {session.user.roles?.join(", ") ?? "—"}
                </dd>
              </div>
            </dl>
          </AdminCard>
        )}

        <AdminCard title="Email delivery">
          <p className="mb-4 text-sm text-fg-muted">
            Configure SMTP providers used for transactional email delivery.
          </p>
          <Link
            to="/smtp-settings"
            className={cn(buttonStyles({ variant: "secondary", size: "sm" }))}
          >
            Manage SMTP settings
          </Link>
        </AdminCard>

        <AdminCard title="Feature flags">
          <p className="mb-4 text-sm text-fg-muted">
            Enable or disable application features without code deployments.
          </p>
          <Link
            to="/feature-flags"
            className={cn(buttonStyles({ variant: "secondary", size: "sm" }))}
          >
            Manage feature flags
          </Link>
        </AdminCard>

        <AdminCard title="Users">
          <p className="mb-4 text-sm text-fg-muted">
            Manage user accounts and assign roles.
          </p>
          <Link
            to="/users"
            className={cn(buttonStyles({ variant: "secondary", size: "sm" }))}
          >
            Manage users
          </Link>
        </AdminCard>

        <AdminCard title="Environment">
          <dl className="grid gap-3 text-sm">
            <div>
              <dt className="text-fg-muted">Environment</dt>
              <dd className="mt-1 font-medium text-fg">
                {import.meta.env.MODE}
              </dd>
            </div>
            <div>
              <dt className="text-fg-muted">Base URL</dt>
              <dd className="mt-1 font-mono text-xs text-fg">
                {import.meta.env.BASE_URL}
              </dd>
            </div>
            <div>
              <dt className="text-fg-muted">Build</dt>
              <dd className="mt-1 font-medium text-fg">Mavrynt Admin</dd>
            </div>
          </dl>
        </AdminCard>

        <AdminCard className="md:col-span-2">
          <p className="text-sm text-fg-muted">
            Additional configuration options will be available in future
            releases. Test email SMTP and first-login password change are
            outside the scope of current settings.
          </p>
        </AdminCard>
      </div>
    </>
  );
};

export default SettingsPage;
