import { useEffect, useState } from "react";
import {
  featureFlagsClient,
  type FeatureFlag,
} from "../lib/admin-api/featureFlagsClient";
import { AdminApiAuthError } from "../lib/admin-api/adminApiClient";
import { useAdminAuth } from "../lib/auth/useAdminAuth";

export default function FeatureFlagsPage() {
  const { accessToken, logout } = useAdminAuth();
  const [items, setItems] = useState<FeatureFlag[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  const load = () =>
    accessToken &&
    featureFlagsClient
      .list(accessToken)
      .then(setItems)
      .catch(async (err) => {
        if (err instanceof AdminApiAuthError) {
          await logout();
          return;
        }
        setError("Failed to load flags");
      });

  useEffect(() => {
    void load();
  }, [accessToken]);

  if (error) return <p role="alert">{error}</p>;
  if (!items) return <p>Loading…</p>;

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">Feature Flags</h1>

      {items.length === 0 ? (
        <p>No feature flags found.</p>
      ) : (
        <ul className="space-y-2">
          {items.map((item) => (
            <li key={item.key} className="rounded border p-3">
              {item.key} — {item.name} ({item.isEnabled ? "enabled" : "disabled"})
              <button
                onClick={() =>
                  accessToken && featureFlagsClient.toggle(accessToken, item.key).then(load)
                }
                className="ml-2 underline"
              >
                Toggle
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
