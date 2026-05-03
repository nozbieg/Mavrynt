import { useEffect, useState } from "react";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, ApiError, type UserDto } from "../lib/api/adminApi.ts";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminState } from "../components/AdminState.tsx";
import { AdminCard } from "../components/AdminCard.tsx";

type LoadState = "loading" | "ready" | "error";

// Roles the backend accepts (Enum.TryParse ignoreCase: true).
const ROLES = ["User", "Admin"] as const;
type Role = (typeof ROLES)[number];

type RowState = {
  selectedRole: Role;
  submitting: boolean;
  error: string | null;
  saved: boolean;
};

function normalizeRole(raw: string): Role {
  const found = ROLES.find((r) => r.toLowerCase() === raw.toLowerCase());
  return found ?? "User";
}

function apiErrorMessage(err: unknown): string {
  if (err instanceof ApiError) {
    if (err.status === 404) return "User not found.";
    if (err.status === 400) return "Invalid role.";
    if (err.status === 0) return "Network error.";
    return `Request failed (HTTP ${String(err.status)}).`;
  }
  return "Unexpected error.";
}

function formatDate(iso: string): string {
  try {
    return new Date(iso).toLocaleDateString(undefined, {
      year: "numeric",
      month: "short",
      day: "numeric",
    });
  } catch {
    return iso;
  }
}

const UsersPage = () => {
  const [state, setState] = useState<LoadState>("loading");
  const [users, setUsers] = useState<UserDto[]>([]);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [rowStates, setRowStates] = useState<Record<string, RowState>>({});

  useEffect(() => {
    void load();
  }, []);

  async function load() {
    setState("loading");
    try {
      const data = await adminApi.listUsers();
      setUsers(data);
      setState("ready");
      const initial: Record<string, RowState> = {};
      data.forEach((u) => {
        initial[u.id] = {
          selectedRole: normalizeRole(u.role),
          submitting: false,
          error: null,
          saved: false,
        };
      });
      setRowStates(initial);
    } catch {
      setLoadError("Failed to load users.");
      setState("error");
    }
  }

  function setRow(userId: string, patch: Partial<RowState>) {
    setRowStates((prev) => ({
      ...prev,
      [userId]: { ...prev[userId], ...patch } as RowState,
    }));
  }

  async function handleRoleSave(user: UserDto) {
    const rs = rowStates[user.id];
    if (!rs || rs.submitting) return;

    setRow(user.id, { submitting: true, error: null, saved: false });
    try {
      const updated = await adminApi.assignUserRole(user.id, rs.selectedRole);
      setUsers((prev) => prev.map((u) => (u.id === updated.id ? updated : u)));
      setRow(user.id, {
        submitting: false,
        selectedRole: normalizeRole(updated.role),
        saved: true,
        error: null,
      });
    } catch (err) {
      setRow(user.id, {
        submitting: false,
        error: apiErrorMessage(err),
        saved: false,
      });
    }
  }

  const STATUS_CLASSES: Record<string, string> = {
    active: "bg-green-500/15 text-green-700 dark:text-green-400",
    inactive: "bg-gray-500/15 text-gray-600 dark:text-gray-400",
    locked: "bg-red-500/15 text-red-700 dark:text-red-400",
  };

  function statusBadge(status: string) {
    const key = status.toLowerCase();
    const cls =
      STATUS_CLASSES[key] ?? "bg-gray-500/15 text-gray-600 dark:text-gray-400";
    return (
      <span
        className={cn(
          "inline-block rounded-full px-2.5 py-0.5 text-xs font-semibold capitalize",
          cls,
        )}
      >
        {status}
      </span>
    );
  }

  return (
    <>
      <Seo title="Users — Mavrynt Admin" description="Manage user accounts" />
      <AdminPageHeader
        title="Users"
        description="Manage user accounts and roles."
      />

      {state === "loading" && <AdminState type="loading" />}
      {state === "error" && (
        <AdminState type="error" message={loadError ?? undefined} />
      )}
      {state === "ready" && users.length === 0 && (
        <AdminState type="empty" message="No users found." />
      )}

      {state === "ready" && users.length > 0 && (
        <AdminCard>
          <div className="overflow-x-auto -mx-5">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-border bg-bg-subtle">
                  {[
                    "Email",
                    "Display name",
                    "Status",
                    "Role",
                    "Created",
                    "",
                  ].map((h) => (
                    <th
                      key={h}
                      scope="col"
                      className="px-5 py-3 text-left text-xs font-semibold uppercase tracking-wide text-fg-muted first:pl-5 last:pr-5"
                    >
                      {h}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-border">
                {users.map((user) => {
                  const rs = rowStates[user.id];
                  const isDirty =
                    rs && rs.selectedRole !== normalizeRole(user.role);
                  return (
                    <tr
                      key={user.id}
                      className="transition-colors hover:bg-bg-subtle"
                    >
                      <td className="px-5 py-3 font-medium text-fg">
                        {user.email}
                      </td>
                      <td className="px-5 py-3 text-fg-muted">
                        {user.displayName ?? (
                          <span className="italic">—</span>
                        )}
                      </td>
                      <td className="px-5 py-3">
                        {statusBadge(user.status)}
                      </td>
                      <td className="px-5 py-3">
                        <div className="flex flex-col gap-1">
                          <div className="flex items-center gap-2">
                            <select
                              value={rs?.selectedRole ?? normalizeRole(user.role)}
                              onChange={(e) =>
                                setRow(user.id, {
                                  selectedRole: e.target.value as Role,
                                  error: null,
                                  saved: false,
                                })
                              }
                              disabled={rs?.submitting}
                              className="rounded-md border border-border bg-bg px-2 py-1 text-xs text-fg focus:outline-none focus:ring-2 focus:ring-focus-ring disabled:opacity-50"
                              aria-label={`Role for ${user.email}`}
                            >
                              {ROLES.map((r) => (
                                <option key={r} value={r}>
                                  {r}
                                </option>
                              ))}
                            </select>
                            <button
                              type="button"
                              disabled={
                                rs?.submitting || (!isDirty && !rs?.error)
                              }
                              onClick={() => void handleRoleSave(user)}
                              className={cn(
                                buttonStyles({ variant: "secondary", size: "sm" }),
                                "text-xs",
                              )}
                              aria-label={`Save role for ${user.email}`}
                            >
                              {rs?.submitting ? "…" : "Save"}
                            </button>
                          </div>
                          {rs?.error && (
                            <p
                              role="alert"
                              className="text-xs text-red-600 dark:text-red-400"
                            >
                              {rs.error}
                            </p>
                          )}
                          {rs?.saved && !rs.error && (
                            <p
                              role="status"
                              aria-live="polite"
                              className="text-xs text-green-600 dark:text-green-400"
                            >
                              Saved
                            </p>
                          )}
                        </div>
                      </td>
                      <td className="px-5 py-3 text-fg-muted">
                        {formatDate(user.createdAt)}
                      </td>
                      <td className="px-5 py-3" />
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </AdminCard>
      )}
    </>
  );
};

export default UsersPage;
