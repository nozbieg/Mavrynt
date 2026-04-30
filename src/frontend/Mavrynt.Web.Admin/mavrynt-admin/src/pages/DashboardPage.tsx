import { useEffect, useState } from "react";
import { RouterLink } from "../lib/router/RouterLink";
import { useAdminAuth } from "../lib/auth/useAdminAuth";
import {
  AdminApiAuthError,
  adminApiRequest,
} from "../lib/admin-api/adminApiClient";

type Profile = {
  email: string;
  displayName?: string;
  role?: string;
  status?: string;
  requiresPasswordChange?: boolean;
};

export default function DashboardPage() {
  const { accessToken, logout, session } = useAdminAuth();
  const [profile, setProfile] = useState<Profile | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!accessToken) return;

    void adminApiRequest<Profile>("/admin/me", accessToken)
      .then(setProfile)
      .catch(async (err) => {
        if (err instanceof AdminApiAuthError) {
          await logout();
          return;
        }
        setError(err instanceof Error ? err.message : "Error");
      });
  }, [accessToken, logout]);

  if (!accessToken) return <p>Unauthorized.</p>;
  if (error) return <p role="alert">{error}</p>;
  if (!profile) return <p>Loading profile…</p>;

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">Admin Dashboard</h1>

      <div className="space-y-2 rounded-lg border p-4">
        <p>Email: {profile.email}</p>
        <p>Display name: {profile.displayName ?? session?.user.name ?? "—"}</p>
        <p>Role: {profile.role ?? session?.user.roles?.[0] ?? "—"}</p>
        <p>Status: {profile.status ?? "—"}</p>
        <p>
          Requires password change: {String(profile.requiresPasswordChange ?? false)}
        </p>
      </div>

      <div className="grid gap-3 md:grid-cols-3">
        <RouterLink to="/feature-flags" variant="inline" className="rounded border p-3">
          Feature Flags
        </RouterLink>
        <RouterLink to="/smtp-settings" variant="inline" className="rounded border p-3">
          SMTP Settings
        </RouterLink>
        <RouterLink to="/users" variant="inline" className="rounded border p-3">
          Users
        </RouterLink>
      </div>
    </div>
  );
}
