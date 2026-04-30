import { useEffect, useState } from "react";
import {
  smtpSettingsClient,
  type SmtpSettings,
} from "../lib/admin-api/smtpSettingsClient";
import { useAdminAuth } from "../lib/auth/useAdminAuth";
import { AdminApiAuthError } from "../lib/admin-api/adminApiClient";

export default function SmtpSettingsPage() {
  const { accessToken, logout } = useAdminAuth();
  const [items, setItems] = useState<SmtpSettings[] | null>(null);
  const [message, setMessage] = useState("");

  const load = () =>
    accessToken &&
    smtpSettingsClient
      .list(accessToken)
      .then(setItems)
      .catch(async (err) => {
        if (err instanceof AdminApiAuthError) {
          await logout();
          return;
        }
        setMessage("Failed to load SMTP settings");
      });

  useEffect(() => {
    void load();
  }, [accessToken]);

  if (!items) return <p>Loading…</p>;

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">SMTP Settings</h1>
      {message && <p role="alert">{message}</p>}

      {items.length === 0 ? (
        <p>No SMTP settings configured.</p>
      ) : (
        <ul className="space-y-2">
          {items.map((item) => (
            <li key={item.id} className="rounded border p-3">
              {item.providerName} {item.host}:{item.port} ({item.isEnabled ? "enabled" : "disabled"})
              <button
                className="ml-2 underline"
                onClick={() => accessToken && smtpSettingsClient.enable(accessToken, item.id).then(load)}
              >
                Enable
              </button>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
