import { useEffect, useState } from "react";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, type FeatureFlagDto } from "../lib/api/adminApi.ts";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminState } from "../components/AdminState.tsx";
import { AdminCard } from "../components/AdminCard.tsx";

type LoadState = "loading" | "ready" | "error";

const FeatureFlagsPage = () => {
  const [state, setState] = useState<LoadState>("loading");
  const [flags, setFlags] = useState<FeatureFlagDto[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [toggling, setToggling] = useState<Set<string>>(new Set());

  useEffect(() => {
    void load();
  }, []);

  async function load() {
    setState("loading");
    try {
      const data = await adminApi.listFeatureFlags();
      setFlags(data);
      setState("ready");
    } catch {
      setError("Failed to load feature flags.");
      setState("error");
    }
  }

  async function handleToggle(key: string) {
    if (toggling.has(key)) return;
    setToggling((prev) => new Set(prev).add(key));
    try {
      const updated = await adminApi.toggleFeatureFlag(key);
      setFlags((prev) => prev.map((f) => (f.key === updated.key ? updated : f)));
    } catch {
      // silently ignore; state stays as-is
    } finally {
      setToggling((prev) => {
        const next = new Set(prev);
        next.delete(key);
        return next;
      });
    }
  }

  return (
    <>
      <Seo title="Feature Flags — Mavrynt Admin" description="Manage feature flags" />
      <AdminPageHeader
        title="Feature Flags"
        description="Enable or disable features across the system."
      />

      {state === "loading" && <AdminState type="loading" />}
      {state === "error" && <AdminState type="error" message={error ?? undefined} />}
      {state === "ready" && flags.length === 0 && (
        <AdminState type="empty" message="No feature flags found." />
      )}
      {state === "ready" && flags.length > 0 && (
        <AdminCard>
          <ul className="divide-y divide-border">
            {flags.map((flag) => (
              <li
                key={flag.id}
                className="flex items-start justify-between gap-4 py-4 first:pt-0 last:pb-0"
              >
                <div className="min-w-0">
                  <p className="truncate text-sm font-semibold text-fg">{flag.name}</p>
                  <p className="text-xs font-mono text-fg-muted">{flag.key}</p>
                  {flag.description && (
                    <p className="mt-1 text-sm text-fg-muted">{flag.description}</p>
                  )}
                </div>
                <div className="flex shrink-0 items-center gap-3">
                  <span
                    className={cn(
                      "text-xs font-medium",
                      flag.isEnabled ? "text-green-600" : "text-fg-muted",
                    )}
                  >
                    {flag.isEnabled ? "Enabled" : "Disabled"}
                  </span>
                  <button
                    type="button"
                    disabled={toggling.has(flag.key)}
                    onClick={() => void handleToggle(flag.key)}
                    className={cn(
                      buttonStyles({ variant: "secondary", size: "sm" }),
                    )}
                  >
                    {toggling.has(flag.key) ? "…" : flag.isEnabled ? "Disable" : "Enable"}
                  </button>
                </div>
              </li>
            ))}
          </ul>
        </AdminCard>
      )}
    </>
  );
};

export default FeatureFlagsPage;
