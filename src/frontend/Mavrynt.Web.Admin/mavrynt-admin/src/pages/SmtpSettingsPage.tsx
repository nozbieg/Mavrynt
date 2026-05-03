import { useEffect, useState, type FormEvent } from "react";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, ApiError, type SmtpSettingsDto } from "../lib/api/adminApi.ts";
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

type FormErrors = Partial<Record<keyof SmtpFormData, string>>;

const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

function validateSmtpForm(
  data: SmtpFormData,
  isCreate: boolean,
): FormErrors {
  const errors: FormErrors = {};
  if (!data.providerName.trim()) errors.providerName = "Provider name is required.";
  if (!data.host.trim()) errors.host = "Host is required.";
  const port = parseInt(data.port, 10);
  if (!data.port || isNaN(port) || port < 1 || port > 65535) {
    errors.port = "Port must be a number between 1 and 65535.";
  }
  if (!data.username.trim()) errors.username = "Username is required.";
  if (isCreate && !data.password) errors.password = "Password is required.";
  if (!data.senderEmail.trim()) {
    errors.senderEmail = "Sender email is required.";
  } else if (!EMAIL_RE.test(data.senderEmail.trim())) {
    errors.senderEmail = "Enter a valid email address.";
  }
  if (!data.senderName.trim()) errors.senderName = "Sender name is required.";
  return errors;
}

function apiErrorMessage(err: unknown): string {
  if (err instanceof ApiError) {
    if (err.status === 404) return "SMTP configuration not found.";
    if (err.status === 400) return "Invalid request. Check your input.";
    if (err.status === 0) return "Network error. Check your connection.";
    return `Request failed (HTTP ${String(err.status)}).`;
  }
  return "An unexpected error occurred.";
}

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

const fieldClass =
  "w-full rounded-md border border-border bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-focus-ring";
const fieldErrorClass =
  "w-full rounded-md border border-red-500 bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-red-500/50";
const labelClass = "block text-sm font-medium text-fg-muted";
const formGroupClass = "flex flex-col gap-1";

type FieldProps = {
  id: string;
  label: string | React.ReactNode;
  error: string | undefined;
  required?: boolean;
  children: React.ReactNode;
};

const FormField = ({ id, label, error, required, children }: FieldProps) => (
  <div className={formGroupClass}>
    <label htmlFor={id} className={labelClass}>
      {label}
      {required && <span aria-hidden="true"> *</span>}
    </label>
    {children}
    {error && (
      <p
        id={`${id}-error`}
        role="alert"
        className="text-xs text-red-600 dark:text-red-400"
      >
        {error}
      </p>
    )}
  </div>
);

const SmtpSettingsPage = () => {
  const [loadState, setLoadState] = useState<LoadState>("loading");
  const [settings, setSettings] = useState<SmtpSettingsDto[]>([]);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [formMode, setFormMode] = useState<FormMode>("none");
  const [formData, setFormData] = useState<SmtpFormData>(emptyForm);
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [submitting, setSubmitting] = useState(false);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [enablingId, setEnablingId] = useState<string | null>(null);
  const [enableError, setEnableError] = useState<string | null>(null);

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
    setFormErrors({});
    setSubmitError(null);
    setSuccessMessage(null);
    setEnableError(null);
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
    setFormErrors({});
    setSubmitError(null);
    setSuccessMessage(null);
    setEnableError(null);
    setFormMode({ edit: s });
  }

  function cancelForm() {
    setFormMode("none");
    setFormErrors({});
    setSubmitError(null);
  }

  function clearFieldError(field: keyof SmtpFormData) {
    setFormErrors((prev) => {
      const next = { ...prev };
      delete next[field];
      return next;
    });
  }

  function field(key: keyof SmtpFormData) {
    return {
      value: formData[key] as string,
      onChange: (
        e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>,
      ) => {
        setFormData((d) => ({ ...d, [key]: e.target.value }));
        clearFieldError(key);
      },
      className: formErrors[key] ? fieldErrorClass : fieldClass,
      "aria-describedby": formErrors[key] ? `smtp-${key}-error` : undefined,
      "aria-invalid": formErrors[key] ? ("true" as const) : undefined,
    };
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (submitting) return;
    setSubmitError(null);
    setSuccessMessage(null);

    const isCreate = formMode === "create";
    const errors = validateSmtpForm(formData, isCreate);
    if (Object.keys(errors).length > 0) {
      setFormErrors(errors);
      return;
    }
    setFormErrors({});

    const port = parseInt(formData.port, 10);
    setSubmitting(true);
    try {
      if (isCreate) {
        const created = await adminApi.createSmtpSettings({
          providerName: formData.providerName.trim(),
          host: formData.host.trim(),
          port,
          username: formData.username.trim(),
          password: formData.password,
          senderEmail: formData.senderEmail.trim(),
          senderName: formData.senderName.trim(),
          useSsl: formData.useSsl,
          isEnabled: formData.isEnabled,
        });
        setSettings((prev) => [...prev, created]);
        setFormMode("none");
        setSuccessMessage(`SMTP configuration "${created.providerName}" created.`);
      } else if (typeof formMode === "object") {
        const updated = await adminApi.updateSmtpSettings(formMode.edit.id, {
          providerName: formData.providerName.trim(),
          host: formData.host.trim(),
          port,
          username: formData.username.trim(),
          ...(formData.password ? { password: formData.password } : {}),
          senderEmail: formData.senderEmail.trim(),
          senderName: formData.senderName.trim(),
          useSsl: formData.useSsl,
        });
        setSettings((prev) =>
          prev.map((s) => (s.id === updated.id ? updated : s)),
        );
        setFormMode("none");
        setSuccessMessage(`SMTP configuration "${updated.providerName}" updated.`);
      }
    } catch (err) {
      setSubmitError(apiErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  }

  async function handleEnable(id: string) {
    if (enablingId === id) return;
    setEnableError(null);
    setSuccessMessage(null);
    setEnablingId(id);
    try {
      const updated = await adminApi.enableSmtpSettings(id);
      setSettings((prev) =>
        prev.map((s) => (s.id === updated.id ? updated : s)),
      );
      setSuccessMessage(
        `SMTP configuration "${updated.providerName}" activated.`,
      );
    } catch (err) {
      setEnableError(apiErrorMessage(err));
    } finally {
      setEnablingId(null);
    }
  }

  const isCreate = formMode === "create";

  return (
    <>
      <Seo
        title="SMTP Settings — Mavrynt Admin"
        description="Configure SMTP settings"
      />
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

      {successMessage && (
        <div
          role="status"
          aria-live="polite"
          className="mb-5 rounded-lg border border-green-500/30 bg-green-500/10 p-3 text-sm text-fg"
        >
          {successMessage}
        </div>
      )}

      {enableError && (
        <div
          role="alert"
          className="mb-5 rounded-lg border border-red-500/30 bg-red-500/10 p-3 text-sm text-fg"
        >
          {enableError}
        </div>
      )}

      {loadState === "loading" && <AdminState type="loading" />}
      {loadState === "error" && (
        <AdminState type="error" message={loadError ?? undefined} />
      )}

      {loadState === "ready" && formMode !== "none" && (
        <AdminCard
          title={
            isCreate ? "New SMTP configuration" : "Edit SMTP configuration"
          }
          className="mb-5"
        >
          <form
            onSubmit={(e) => void handleSubmit(e)}
            className="grid gap-4 sm:grid-cols-2"
            noValidate
            aria-label={
              isCreate
                ? "Create SMTP configuration form"
                : "Edit SMTP configuration form"
            }
          >
            <FormField
              id="smtp-providerName"
              label="Provider name"
              error={formErrors.providerName}
              required
            >
              <input
                id="smtp-providerName"
                {...field("providerName")}
                placeholder="e.g. SendGrid"
                aria-required="true"
              />
            </FormField>

            <FormField
              id="smtp-host"
              label="Host"
              error={formErrors.host}
              required
            >
              <input
                id="smtp-host"
                {...field("host")}
                placeholder="smtp.example.com"
                aria-required="true"
              />
            </FormField>

            <FormField
              id="smtp-port"
              label="Port"
              error={formErrors.port}
              required
            >
              <input
                id="smtp-port"
                type="number"
                {...field("port")}
                min={1}
                max={65535}
                aria-required="true"
              />
            </FormField>

            <FormField
              id="smtp-username"
              label="Username"
              error={formErrors.username}
              required
            >
              <input
                id="smtp-username"
                {...field("username")}
                autoComplete="username"
                aria-required="true"
              />
            </FormField>

            <FormField
              id="smtp-password"
              label={
                isCreate
                  ? "Password"
                  : "Password (leave blank to keep current)"
              }
              error={formErrors.password}
              required={isCreate}
            >
              <input
                id="smtp-password"
                type="password"
                value={formData.password}
                onChange={(e) => {
                  setFormData((d) => ({ ...d, password: e.target.value }));
                  clearFieldError("password");
                }}
                className={formErrors.password ? fieldErrorClass : fieldClass}
                aria-describedby={
                  formErrors.password ? "smtp-password-error" : undefined
                }
                aria-invalid={formErrors.password ? "true" : undefined}
                aria-required={isCreate ? "true" : undefined}
                autoComplete="new-password"
                placeholder={isCreate ? "" : "Leave blank to keep unchanged"}
              />
            </FormField>

            <FormField
              id="smtp-senderEmail"
              label="Sender email"
              error={formErrors.senderEmail}
              required
            >
              <input
                id="smtp-senderEmail"
                type="email"
                {...field("senderEmail")}
                aria-required="true"
              />
            </FormField>

            <FormField
              id="smtp-senderName"
              label="Sender name"
              error={formErrors.senderName}
              required
            >
              <input
                id="smtp-senderName"
                {...field("senderName")}
                aria-required="true"
              />
            </FormField>

            <div className="flex flex-wrap items-center gap-4 sm:col-span-2">
              <label className="flex cursor-pointer items-center gap-2 text-sm text-fg">
                <input
                  type="checkbox"
                  checked={formData.useSsl}
                  onChange={(e) =>
                    setFormData((d) => ({ ...d, useSsl: e.target.checked }))
                  }
                  className="h-4 w-4 rounded border-border"
                />
                Use SSL/TLS
              </label>
              {isCreate && (
                <label className="flex cursor-pointer items-center gap-2 text-sm text-fg">
                  <input
                    type="checkbox"
                    checked={formData.isEnabled}
                    onChange={(e) =>
                      setFormData((d) => ({
                        ...d,
                        isEnabled: e.target.checked,
                      }))
                    }
                    className="h-4 w-4 rounded border-border"
                  />
                  Enable immediately
                </label>
              )}
            </div>

            {submitError && (
              <div
                role="alert"
                className="rounded-lg border border-red-500/30 bg-red-500/10 p-3 text-sm text-fg sm:col-span-2"
              >
                {submitError}
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
              <li
                key={s.id}
                className="flex flex-wrap items-start justify-between gap-4 py-4 first:pt-0 last:pb-0"
              >
                <div className="min-w-0">
                  <p className="text-sm font-semibold text-fg">
                    {s.providerName}
                  </p>
                  <p className="text-xs text-fg-muted">
                    {s.host}:{s.port} · {s.username} ·{" "}
                    {s.useSsl ? "SSL" : "Plain"}
                  </p>
                  <p className="text-xs text-fg-muted">
                    From: {s.senderName} &lt;{s.senderEmail}&gt;
                  </p>
                </div>
                <div className="flex shrink-0 items-center gap-2">
                  <span
                    className={cn(
                      "text-xs font-medium",
                      s.isEnabled ? "text-green-600" : "text-fg-muted",
                    )}
                  >
                    {s.isEnabled ? "Active" : "Inactive"}
                  </span>
                  {!s.isEnabled && (
                    <button
                      type="button"
                      disabled={enablingId === s.id}
                      onClick={() => void handleEnable(s.id)}
                      className={cn(
                        buttonStyles({ variant: "secondary", size: "sm" }),
                      )}
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
