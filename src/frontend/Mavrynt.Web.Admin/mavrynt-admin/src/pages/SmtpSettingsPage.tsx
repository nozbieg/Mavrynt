import { useEffect, useState, type FormEvent } from "react";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, type SmtpSettingsDto } from "../lib/api/adminApi.ts";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminState } from "../components/AdminState.tsx";
import { AdminCard } from "../components/AdminCard.tsx";

type LoadState = "loading" | "ready" | "error";
type FormMode = "none" | "create" | { edit: SmtpSettingsDto };

type SmtpFormData = {
  providerName: string;
  host: string;
  port: string;
  username: string;
  password: string;
  senderEmail: string;
  senderName: string;
  useSsl: boolean;
  isEnabled: boolean;
};

const emptyForm: SmtpFormData = {
  providerName: "",
  host: "",
  port: "587",
  username: "",
  password: "",
  senderEmail: "",
  senderName: "",
  useSsl: true,
  isEnabled: false,
};

const fieldClass = "w-full rounded-md border border-border bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-focus-ring";
const labelClass = "block text-sm font-medium text-fg-muted";
const formGroupClass = "flex flex-col gap-1";

const SmtpSettingsPage = () => {
  const [loadState, setLoadState] = useState<LoadState>("loading");
  const [settings, setSettings] = useState<SmtpSettingsDto[]>([]);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [formMode, setFormMode] = useState<FormMode>("none");
  const [formData, setFormData] = useState<SmtpFormData>(emptyForm);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);
  const [enablingId, setEnablingId] = useState<string | null>(null);

  useEffect(() => {
    void load();
  }, []);

  async function load() {
    setLoadState("loading");
    try {
      const data = await adminApi.listSmtpSettings();
      setSettings(data);
      setLoadState("ready");
    } catch {
      setLoadError("Failed to load SMTP settings.");
      setLoadState("error");
    }
  }

  function openCreate() {
    setFormData(emptyForm);
    setFormError(null);
    setFormMode("create");
  }

  function openEdit(s: SmtpSettingsDto) {
    setFormData({
      providerName: s.providerName,
      host: s.host,
      port: String(s.port),
      username: s.username,
      password: "",
      senderEmail: s.senderEmail,
      senderName: s.senderName,
      useSsl: s.useSsl,
      isEnabled: s.isEnabled,
    });
    setFormError(null);
    setFormMode({ edit: s });
  }

  function cancelForm() {
    setFormMode("none");
    setFormError(null);
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (submitting) return;
    setFormError(null);
    setSubmitting(true);

    const port = parseInt(formData.port, 10);
    if (isNaN(port) || port < 1 || port > 65535) {
      setFormError("Port must be between 1 and 65535.");
      setSubmitting(false);
      return;
    }

    try {
      if (formMode === "create") {
        const created = await adminApi.createSmtpSettings({
          providerName: formData.providerName,
          host: formData.host,
          port,
          username: formData.username,
          password: formData.password,
          senderEmail: formData.senderEmail,
          senderName: formData.senderName,
          useSsl: formData.useSsl,
          isEnabled: formData.isEnabled,
        });
        setSettings((prev) => [...prev, created]);
      } else if (typeof formMode === "object") {
        const updated = await adminApi.updateSmtpSettings(formMode.edit.id, {
          providerName: formData.providerName,
          host: formData.host,
          port,
          username: formData.username,
          ...(formData.password ? { password: formData.password } : {}),
          senderEmail: formData.senderEmail,
          senderName: formData.senderName,
          useSsl: formData.useSsl,
        });
        setSettings((prev) => prev.map((s) => (s.id === updated.id ? updated : s)));
      }
      setFormMode("none");
    } catch {
      setFormError("Failed to save SMTP settings.");
    } finally {
      setSubmitting(false);
    }
  }

  async function handleEnable(id: string) {
    if (enablingId === id) return;
    setEnablingId(id);
    try {
      const updated = await adminApi.enableSmtpSettings(id);
      setSettings((prev) => prev.map((s) => (s.id === updated.id ? updated : s)));
    } catch {
      // ignore
    } finally {
      setEnablingId(null);
    }
  }

  return (
    <>
      <Seo title="SMTP Settings — Mavrynt Admin" description="Configure SMTP settings" />
      <AdminPageHeader
        title="SMTP Settings"
        description="Configure email delivery providers."
        actions={
          formMode === "none" ? (
            <button
              type="button"
              onClick={openCreate}
              className={cn(buttonStyles({ variant: "primary", size: "sm" }))}
            >
              Add configuration
            </button>
          ) : null
        }
      />

      {loadState === "loading" && <AdminState type="loading" />}
      {loadState === "error" && <AdminState type="error" message={loadError ?? undefined} />}

      {loadState === "ready" && formMode !== "none" && (
        <AdminCard
          title={formMode === "create" ? "New SMTP configuration" : "Edit SMTP configuration"}
          className="mb-5"
        >
          <form onSubmit={(e) => void handleSubmit(e)} className="grid gap-4 sm:grid-cols-2">
            <div className={formGroupClass}>
              <label htmlFor="smtp-provider" className={labelClass}>Provider name</label>
              <input
                id="smtp-provider"
                required
                value={formData.providerName}
                onChange={(e) => setFormData((d) => ({ ...d, providerName: e.target.value }))}
                className={fieldClass}
                placeholder="e.g. SendGrid"
              />
            </div>
            <div className={formGroupClass}>
              <label htmlFor="smtp-host" className={labelClass}>Host</label>
              <input
                id="smtp-host"
                required
                value={formData.host}
                onChange={(e) => setFormData((d) => ({ ...d, host: e.target.value }))}
                className={fieldClass}
                placeholder="smtp.example.com"
              />
            </div>
            <div className={formGroupClass}>
              <label htmlFor="smtp-port" className={labelClass}>Port</label>
              <input
                id="smtp-port"
                type="number"
                required
                value={formData.port}
                onChange={(e) => setFormData((d) => ({ ...d, port: e.target.value }))}
                className={fieldClass}
                min={1}
                max={65535}
              />
            </div>
            <div className={formGroupClass}>
              <label htmlFor="smtp-username" className={labelClass}>Username</label>
              <input
                id="smtp-username"
                required
                value={formData.username}
                onChange={(e) => setFormData((d) => ({ ...d, username: e.target.value }))}
                className={fieldClass}
                autoComplete="username"
              />
            </div>
            <div className={formGroupClass}>
              <label htmlFor="smtp-password" className={labelClass}>
                Password {formMode !== "create" && <span className="text-xs">(leave blank to keep current)</span>}
              </label>
              <input
                id="smtp-password"
                type="password"
                required={formMode === "create"}
                value={formData.password}
                onChange={(e) => setFormData((d) => ({ ...d, password: e.target.value }))}
                className={fieldClass}
                autoComplete="new-password"
                placeholder={formMode !== "create" ? "Leave blank to keep unchanged" : ""}
              />
            </div>
            <div className={formGroupClass}>
              <label htmlFor="smtp-sender-email" className={labelClass}>Sender email</label>
              <input
                id="smtp-sender-email"
                type="email"
                required
                value={formData.senderEmail}
                onChange={(e) => setFormData((d) => ({ ...d, senderEmail: e.target.value }))}
                className={fieldClass}
              />
            </div>
            <div className={formGroupClass}>
              <label htmlFor="smtp-sender-name" className={labelClass}>Sender name</label>
              <input
                id="smtp-sender-name"
                required
                value={formData.senderName}
                onChange={(e) => setFormData((d) => ({ ...d, senderName: e.target.value }))}
                className={fieldClass}
              />
            </div>
            <div className="flex items-center gap-3 sm:col-span-2">
              <label className="flex cursor-pointer items-center gap-2 text-sm text-fg">
                <input
                  type="checkbox"
                  checked={formData.useSsl}
                  onChange={(e) => setFormData((d) => ({ ...d, useSsl: e.target.checked }))}
                  className="h-4 w-4 rounded border-border"
                />
                Use SSL/TLS
              </label>
              {formMode === "create" && (
                <label className="flex cursor-pointer items-center gap-2 text-sm text-fg">
                  <input
                    type="checkbox"
                    checked={formData.isEnabled}
                    onChange={(e) => setFormData((d) => ({ ...d, isEnabled: e.target.checked }))}
                    className="h-4 w-4 rounded border-border"
                  />
                  Enable immediately
                </label>
              )}
            </div>

            {formError && (
              <div role="alert" className="rounded-lg border border-danger-500/30 bg-danger-500/10 p-3 text-sm text-fg sm:col-span-2">
                {formError}
              </div>
            )}

            <div className="flex gap-2 sm:col-span-2">
              <button
                type="submit"
                disabled={submitting}
                className={cn(buttonStyles({ variant: "primary", size: "sm" }))}
              >
                {submitting ? "Saving…" : "Save"}
              </button>
              <button
                type="button"
                onClick={cancelForm}
                className={cn(buttonStyles({ variant: "ghost", size: "sm" }))}
              >
                Cancel
              </button>
            </div>
          </form>
        </AdminCard>
      )}

      {loadState === "ready" && settings.length === 0 && formMode === "none" && (
        <AdminState type="empty" message="No SMTP configurations yet." />
      )}

      {loadState === "ready" && settings.length > 0 && (
        <AdminCard>
          <ul className="divide-y divide-border">
            {settings.map((s) => (
              <li key={s.id} className="flex flex-wrap items-start justify-between gap-4 py-4 first:pt-0 last:pb-0">
                <div className="min-w-0">
                  <p className="text-sm font-semibold text-fg">{s.providerName}</p>
                  <p className="text-xs text-fg-muted">
                    {s.host}:{s.port} · {s.username} · {s.useSsl ? "SSL" : "Plain"}
                  </p>
                  <p className="text-xs text-fg-muted">
                    From: {s.senderName} &lt;{s.senderEmail}&gt;
                  </p>
                </div>
                <div className="flex shrink-0 items-center gap-2">
                  <span className={cn("text-xs font-medium", s.isEnabled ? "text-green-600" : "text-fg-muted")}>
                    {s.isEnabled ? "Active" : "Inactive"}
                  </span>
                  {!s.isEnabled && (
                    <button
                      type="button"
                      disabled={enablingId === s.id}
                      onClick={() => void handleEnable(s.id)}
                      className={cn(buttonStyles({ variant: "secondary", size: "sm" }))}
                    >
                      {enablingId === s.id ? "…" : "Activate"}
                    </button>
                  )}
                  <button
                    type="button"
                    onClick={() => openEdit(s)}
                    className={cn(buttonStyles({ variant: "ghost", size: "sm" }))}
                  >
                    Edit
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

export default SmtpSettingsPage;
