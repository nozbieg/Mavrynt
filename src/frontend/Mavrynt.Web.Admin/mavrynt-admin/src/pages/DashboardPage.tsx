import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import { AuthCard } from "@mavrynt/auth-ui";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import {
  clearAdminSession,
  getAdminAccessToken,
  getAdminSession,
} from "../lib/auth/adminSession.ts";

type AdminProfile = {
  email: string;
  displayName?: string;
  role?: string;
  status?: string;
  requiresPasswordChange?: boolean;
};

type LoadState = "loading" | "ready" | "error";
const LOGIN_REDIRECT_REASON_KEY = "admin_login_redirect_reason";

const DashboardPage = () => {
  const navigate = useNavigate();
  const [state, setState] = useState<LoadState>("loading");
  const [profile, setProfile] = useState<AdminProfile | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const token = getAdminAccessToken();
    if (!token) {
      void navigate("/login", { replace: true });
      return;
    }

    void (async () => {
      try {
        const response = await fetch("/admin-api/admin/me", {
          method: "GET",
          headers: {
            Accept: "application/json",
            Authorization: `Bearer ${token}`,
          },
          credentials: "include",
        });

        if (response.status === 401 || response.status === 403) {
          sessionStorage.setItem(LOGIN_REDIRECT_REASON_KEY, "Session expired or access denied.");
          clearAdminSession();
          void navigate("/login", { replace: true });
          return;
        }

        if (response.status === 404) {
          setError("Admin profile was not found for the current token.");
          setState("error");
          return;
        }

        if (!response.ok) {
          let serverMessage = "";
          try {
            const payload = (await response.json()) as { detail?: string; title?: string; message?: string };
            serverMessage = payload.detail ?? payload.title ?? payload.message ?? "";
          } catch {
            // Ignore JSON parse issues and fall back to generic message.
          }

          setError(
            serverMessage
              ? `Unable to load your admin profile (HTTP ${response.status}): ${serverMessage}`
              : `Unable to load your admin profile (HTTP ${response.status}).`,
          );
          setState("error");
          return;
        }

        const data = (await response.json()) as AdminProfile;
        setProfile(data);
        setState("ready");
      } catch {
        setError("Network error while loading dashboard data.");
        setState("error");
      }
    })();
  }, [navigate]);

  const handleLogout = () => {
    clearAdminSession();
    void navigate("/login", { replace: true });
  };

  const fallbackSession = getAdminSession();

  return (
    <>
      <Seo title="Admin Dashboard" description="Mavrynt admin dashboard" />
      <Section spacing="lg" container="md">
        <AuthCard eyebrow="Mavrynt Admin" title="Admin Dashboard" subtitle="Your account overview.">
          {state === "loading" && <p>Loading your profile…</p>}
          {state === "error" && <p role="alert">{error}</p>}
          {state === "ready" && profile && (
            <Stack gap="sm">
              <p><strong>Email:</strong> {profile.email}</p>
              <p><strong>Display name:</strong> {profile.displayName ?? fallbackSession?.user.name ?? "—"}</p>
              <p><strong>Role:</strong> {profile.role ?? fallbackSession?.user.roles?.[0] ?? "—"}</p>
              <p><strong>Status:</strong> {profile.status ?? "—"}</p>
              <p><strong>Requires password change:</strong> {String(profile.requiresPasswordChange ?? false)}</p>
              <button type="button" onClick={handleLogout} className={cn(buttonStyles({ variant: "secondary", size: "md" }))}>Logout</button>
            </Stack>
          )}
        </AuthCard>
      </Section>
    </>
  );
};

export default DashboardPage;
