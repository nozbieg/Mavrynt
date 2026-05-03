import { useEffect, useState, type FormEvent } from "react";
import { cn, buttonStyles } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { adminApi, ApiError, type FeatureFlagDto } from "../lib/api/adminApi.ts";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminState } from "../components/AdminState.tsx";
import { AdminCard } from "../components/AdminCard.tsx";

type LoadState = "loading" | "ready" | "error";
type FormMode = "none" | "create" | { edit: FeatureFlagDto };

type FlagForm = {
  key: string;
  name: string;
  description: string;
  isEnabled: boolean;
};

type FormErrors = Partial<Record<keyof FlagForm, string>>;

const FLAG_KEY_RE = /^[a-z0-9][a-z0-9._-]*$/;

function validateFlagForm(data: FlagForm, isCreate: boolean): FormErrors {
  const errors: FormErrors = {};
  if (isCreate) {
    if (!data.key.trim()) {
      errors.key = "Key is required.";
    } else if (!FLAG_KEY_RE.test(data.key.trim())) {
      errors.key =
        "Key must be lowercase and contain only letters, numbers, dots, hyphens, or underscores.";
    } else if (data.key.trim().length > 100) {
      errors.key = "Key must be 100 characters or fewer.";
    }
  }
  if (!data.name.trim()) {
    errors.name = "Name is required.";
  } else if (data.name.trim().length > 200) {
    errors.name = "Name must be 200 characters or fewer.";
  }
  if (data.description.length > 500) {
    errors.description = "Description must be 500 characters or fewer.";
  }
  return errors;
}

function apiErrorMessage(err: unknown): string {
  if (err instanceof ApiError) {
    if (err.status === 409) return "A flag with this key already exists.";
    if (err.status === 404) return "Feature flag not found.";
    if (err.status === 400) return "Invalid request. Check your input.";
    if (err.status === 0) return "Network error. Check your connection.";
    return `Request failed (HTTP ${String(err.status)}).`;
  }
  return "An unexpected error occurred.";
}

const emptyForm: FlagForm = {
  key: "",
  name: "",
  description: "",
  isEnabled: false,
};

const fieldClass =
  "w-full rounded-md border border-border bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-focus-ring";
const fieldErrorClass =
  "w-full rounded-md border border-red-500 bg-bg px-3 py-2 text-sm text-fg placeholder:text-fg-muted focus:outline-none focus:ring-2 focus:ring-red-500/50";
const labelClass = "block text-sm font-medium text-fg-muted";
const formGroupClass = "flex flex-col gap-1";

const FeatureFlagsPage = () => {
  const [state, setState] = useState<LoadState>("loading");
  const [flags, setFlags] = useState<FeatureFlagDto[]>([]);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [formMode, setFormMode] = useState<FormMode>("none");
  const [formData, setFormData] = useState<FlagForm>(emptyForm);
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [toggling, setToggling] = useState<Set<string>>(new Set());
  const [toggleErrors, setToggleErrors] = useState<Record<string, string>>({});

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
      setLoadError("Failed to load feature flags.");
      setState("error");
    }
  }

  function openCreate() {
    setFormData(emptyForm);
    setFormErrors({});
    setSubmitError(null);
    setSuccessMessage(null);
    setFormMode("create");
  }

  function openEdit(flag: FeatureFlagDto) {
    setFormData({
      key: flag.key,
      name: flag.name,
      description: flag.description ?? "",
      isEnabled: flag.isEnabled,
    });
    setFormErrors({});
    setSubmitError(null);
    setSuccessMessage(null);
    setFormMode({ edit: flag });
  }

  function cancelForm() {
    setFormMode("none");
    setFormErrors({});
    setSubmitError(null);
  }

  function clearFieldError(field: keyof FlagForm) {
    setFormErrors((prev) => {
      const next = { ...prev };
      delete next[field];
      return next;
    });
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    if (submitting) return;
    setSubmitError(null);
    setSuccessMessage(null);

    const isCreate = formMode === "create";
    const errors = validateFlagForm(formData, isCreate);
    if (Object.keys(errors).length > 0) {
      setFormErrors(errors);
      return;
    }
    setFormErrors({});

    setSubmitting(true);
    try {
      if (isCreate) {
        const created = await adminApi.createFeatureFlag({
          key: formData.key.trim(),
          name: formData.name.trim(),
          ...(formData.description.trim()
            ? { description: formData.description.trim() }
            : {}),
          isEnabled: formData.isEnabled,
        });
        setFlags((prev) => [...prev, created]);
        setFormMode("none");
        setSuccessMessage(`Flag "${created.name}" created.`);
      } else if (typeof formMode === "object") {
        const updated = await adminApi.updateFeatureFlag(formMode.edit.key, {
          name: formData.name.trim(),
          ...(formData.description.trim()
            ? { description: formData.description.trim() }
            : {}),
        });
        setFlags((prev) =>
          prev.map((f) => (f.key === updated.key ? updated : f)),
        );
        setFormMode("none");
        setSuccessMessage(`Flag "${updated.name}" updated.`);
      }
    } catch (err) {
      setSubmitError(apiErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  }

  async function handleToggle(key: string) {
    if (toggling.has(key)) return;
    setToggleErrors((prev) => {
      const next = { ...prev };
      delete next[key];
      return next;
    });
    setToggling((prev) => new Set(prev).add(key));
    try {
      const updated = await adminApi.toggleFeatureFlag(key);
      setFlags((prev) =>
        prev.map((f) => (f.key === updated.key ? updated : f)),
      );
    } catch (err) {
      setToggleErrors((prev) => ({ ...prev, [key]: apiErrorMessage(err) }));
    } finally {
      setToggling((prev) => {
        const next = new Set(prev);
        next.delete(key);
        return next;
      });
    }
  }

  const isCreate = formMode === "create";

  return (
    <>
      <Seo
        title="Feature Flags — Mavrynt Admin"
        description="Manage feature flags"
      />
      <AdminPageHeader
        title="Feature Flags"
        description="Enable or disable features across the system."
        actions={
          formMode === "none" ? (
            <button
              type="button"
              onClick={openCreate}
              className={cn(buttonStyles({ variant: "primary", size: "sm" }))}
            >
              New flag
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

      {state === "loading" && <AdminState type="loading" />}
      {state === "error" && (
        <AdminState type="error" message={loadError ?? undefined} />
      )}

      {state === "ready" && formMode !== "none" && (
        <AdminCard
          title={isCreate ? "New feature flag" : "Edit feature flag"}
          className="mb-5"
        >
          <form
            onSubmit={(e) => void handleSubmit(e)}
            className="flex flex-col gap-4 sm:max-w-lg"
            noValidate
            aria-label={
              isCreate ? "Create feature flag form" : "Edit feature flag form"
            }
          >
            {isCreate && (
              <div className={formGroupClass}>
                <label htmlFor="flag-key" className={labelClass}>
                  Key <span aria-hidden="true">*</span>
                </label>
                <input
                  id="flag-key"
                  value={formData.key}
                  onChange={(e) => {
                    setFormData((d) => ({ ...d, key: e.target.value }));
                    clearFieldError("key");
                  }}
                  className={formErrors.key ? fieldErrorClass : fieldClass}
                  placeholder="feature.module.name"
                  aria-describedby={
                    formErrors.key ? "flag-key-error" : "flag-key-hint"
                  }
                  aria-invalid={formErrors.key ? "true" : undefined}
                  aria-required="true"
                />
                {formErrors.key ? (
                  <p
                    id="flag-key-error"
                    role="alert"
                    className="text-xs text-red-600 dark:text-red-400"
                  >
                    {formErrors.key}
                  </p>
                ) : (
                  <p id="flag-key-hint" className="text-xs text-fg-muted">
                    Lowercase letters, numbers, dots, hyphens, underscores. Max
                    100 characters.
                  </p>
                )}
              </div>
            )}

            <div className={formGroupClass}>
              <label htmlFor="flag-name" className={labelClass}>
                Name <span aria-hidden="true">*</span>
              </label>
              <input
                id="flag-name"
                value={formData.name}
                onChange={(e) => {
                  setFormData((d) => ({ ...d, name: e.target.value }));
                  clearFieldError("name");
                }}
                className={formErrors.name ? fieldErrorClass : fieldClass}
                placeholder="Human-readable flag name"
                aria-describedby={formErrors.name ? "flag-name-error" : undefined}
                aria-invalid={formErrors.name ? "true" : undefined}
                aria-required="true"
              />
              {formErrors.name && (
                <p
                  id="flag-name-error"
                  role="alert"
                  className="text-xs text-red-600 dark:text-red-400"
                >
                  {formErrors.name}
                </p>
              )}
            </div>

            <div className={formGroupClass}>
              <label htmlFor="flag-description" className={labelClass}>
                Description
              </label>
              <textarea
                id="flag-description"
                rows={3}
                value={formData.description}
                onChange={(e) => {
                  setFormData((d) => ({ ...d, description: e.target.value }));
                  clearFieldError("description");
                }}
                className={cn(
                  fieldClass,
                  "resize-y",
                  formErrors.description ? "border-red-500" : "",
                )}
                placeholder="Optional description"
                aria-describedby={
                  formErrors.description ? "flag-description-error" : undefined
                }
                aria-invalid={formErrors.description ? "true" : undefined}
              />
              {formErrors.description && (
                <p
                  id="flag-description-error"
                  role="alert"
                  className="text-xs text-red-600 dark:text-red-400"
                >
                  {formErrors.description}
                </p>
              )}
            </div>

            {isCreate && (
              <label className="flex cursor-pointer items-center gap-2 text-sm text-fg">
                <input
                  type="checkbox"
                  checked={formData.isEnabled}
                  onChange={(e) =>
                    setFormData((d) => ({ ...d, isEnabled: e.target.checked }))
                  }
                  className="h-4 w-4 rounded border-border"
                />
                Enable immediately
              </label>
            )}

            {submitError && (
              <div
                role="alert"
                className="rounded-lg border border-red-500/30 bg-red-500/10 p-3 text-sm text-fg"
              >
                {submitError}
              </div>
            )}

            <div className="flex gap-2">
              <button
                type="submit"
                disabled={submitting}
                className={cn(buttonStyles({ variant: "primary", size: "sm" }))}
              >
                {submitting
                  ? "Saving…"
                  : isCreate
                    ? "Create flag"
                    : "Save changes"}
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

      {state === "ready" && flags.length === 0 && formMode === "none" && (
        <AdminState type="empty" message="No feature flags found." />
      )}

      {state === "ready" && flags.length > 0 && (
        <AdminCard>
          <ul className="divide-y divide-border" aria-label="Feature flags list">
            {flags.map((flag) => (
              <li
                key={flag.id}
                className="flex flex-wrap items-start justify-between gap-4 py-4 first:pt-0 last:pb-0"
              >
                <div className="min-w-0">
                  <p className="truncate text-sm font-semibold text-fg">
                    {flag.name}
                  </p>
                  <p className="text-xs font-mono text-fg-muted">{flag.key}</p>
                  {flag.description && (
                    <p className="mt-1 text-sm text-fg-muted">
                      {flag.description}
                    </p>
                  )}
                  {toggleErrors[flag.key] && (
                    <p
                      role="alert"
                      className="mt-1 text-xs text-red-600 dark:text-red-400"
                    >
                      {toggleErrors[flag.key]}
                    </p>
                  )}
                </div>
                <div className="flex shrink-0 items-center gap-2">
                  <span
                    className={cn(
                      "text-xs font-medium",
                      flag.isEnabled ? "text-green-600" : "text-fg-muted",
                    )}
                    aria-live="polite"
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
                    aria-label={`${flag.isEnabled ? "Disable" : "Enable"} ${flag.name}`}
                  >
                    {toggling.has(flag.key)
                      ? "…"
                      : flag.isEnabled
                        ? "Disable"
                        : "Enable"}
                  </button>
                  <button
                    type="button"
                    onClick={() => openEdit(flag)}
                    className={cn(buttonStyles({ variant: "ghost", size: "sm" }))}
                    aria-label={`Edit ${flag.name}`}
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

export default FeatureFlagsPage;
