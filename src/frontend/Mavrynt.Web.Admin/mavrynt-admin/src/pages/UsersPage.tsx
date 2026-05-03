import { useState, type FormEvent } from "react";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, ApiError, type UserDto } from "../lib/api/adminApi.ts";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminCard } from "../components/AdminCard.tsx";

// Backend has no GET /api/admin/users list endpoint.
// Only PATCH /{userId}/role exists. If a list endpoint is added, replace
// this page with a full user table using AdminTable.

const ROLES = ["admin", "user"] as const;
type Role = (typeof ROLES)[number];

const fieldClass =
  "w-full rounded-md border border-border bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-focus-ring";
const fieldErrorClass =
  "w-full rounded-md border border-red-500 bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-red-500/50";
const labelClass = "block text-sm font-medium text-fg-muted";
const formGroupClass = "flex flex-col gap-1";

function apiErrorMessage(err: unknown): string {
  if (err instanceof ApiError) {
    if (err.status === 404) return "User not found.";
    if (err.status === 400) return "Invalid role value.";
    if (err.status === 0) return "Network error. Check your connection and try again.";
    return `Request failed (HTTP ${String(err.status)}).`;
  }
  return "An unexpected error occurred.";
}

const UsersPage = () => {
  const [userId, setUserId] = useState("");
  const [role, setRole] = useState<Role>("user");
  const [submitting, setSubmitting] = useState(false);
  const [result, setResult] = useState<UserDto | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [userIdError, setUserIdError] = useState<string | null>(null);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (submitting) return;
    setError(null);
    setResult(null);
    setUserIdError(null);

    const trimmedId = userId.trim();
    if (!trimmedId) {
      setUserIdError("User ID is required.");
      return;
    }

    setSubmitting(true);
    try {
      const updated = await adminApi.assignUserRole(trimmedId, role);
      setResult(updated);
      setUserId("");
    } catch (err) {
      setError(apiErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <>
      <Seo title="Users — Mavrynt Admin" description="Manage user accounts" />
      <AdminPageHeader
        title="Users"
        description="Manage user accounts and roles."
      />

      <div className="flex flex-col gap-5">
        <AdminCard>
          <p className="text-sm text-fg-muted">
            <strong className="font-medium text-fg">Note:</strong> User listing
            is not available — the backend does not expose a list users endpoint.
            To assign a role to a specific user, enter their user ID below.
          </p>
        </AdminCard>

        <AdminCard title="Assign role">
          <form
            onSubmit={(e) => void handleSubmit(e)}
            className="flex flex-col gap-4 sm:max-w-md"
            noValidate
          >
            <div className={formGroupClass}>
              <label htmlFor="user-id" className={labelClass}>
                User ID
              </label>
              <input
                id="user-id"
                value={userId}
                onChange={(e) => {
                  setUserId(e.target.value);
                  setUserIdError(null);
                }}
                className={userIdError ? fieldErrorClass : fieldClass}
                placeholder="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
                autoComplete="off"
                aria-describedby={userIdError ? "user-id-error" : undefined}
                aria-invalid={userIdError ? "true" : undefined}
              />
              {userIdError && (
                <p
                  id="user-id-error"
                  role="alert"
                  className="text-xs text-red-600 dark:text-red-400"
                >
                  {userIdError}
                </p>
              )}
            </div>

            <div className={formGroupClass}>
              <label htmlFor="user-role" className={labelClass}>
                New role
              </label>
              <select
                id="user-role"
                value={role}
                onChange={(e) => setRole(e.target.value as Role)}
                className={fieldClass}
              >
                {ROLES.map((r) => (
                  <option key={r} value={r}>
                    {r.charAt(0).toUpperCase() + r.slice(1)}
                  </option>
                ))}
              </select>
            </div>

            {error && (
              <div
                role="alert"
                className="rounded-lg border border-red-500/30 bg-red-500/10 p-3 text-sm text-fg"
              >
                {error}
              </div>
            )}

            {result && (
              <div
                role="status"
                aria-live="polite"
                className="rounded-lg border border-green-500/30 bg-green-500/10 p-3 text-sm text-fg"
              >
                Role updated.{" "}
                <span className="font-medium">{result.email}</span> is now{" "}
                <strong className="font-medium">{result.role}</strong>.
              </div>
            )}

            <div>
              <button
                type="submit"
                disabled={submitting}
                className={cn(buttonStyles({ variant: "primary", size: "sm" }))}
              >
                {submitting ? "Saving…" : "Assign role"}
              </button>
            </div>
          </form>
        </AdminCard>
      </div>
    </>
  );
};

export default UsersPage;
