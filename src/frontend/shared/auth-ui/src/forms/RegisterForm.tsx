import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import { buttonStyles, cn } from "@mavrynt/ui";
import { AUTH_I18N_NAMESPACE } from "../i18n/index.ts";
import type { AuthSession } from "../service/types.ts";
import { TextField } from "./TextField.tsx";
import { PasswordField } from "./PasswordField.tsx";
import { useRegisterForm } from "./useRegisterForm.ts";

/**
 * RegisterForm — pure-presentational form bound to `useRegisterForm`.
 *
 * `disabled` mode renders an info banner instead of the form, so admin
 * (where self-registration is invite-only — Phase 1 decision) can still
 * mount the page for layout/route consistency without exposing inputs.
 */
export interface RegisterFormProps {
  /** Source tag for analytics / audit. Default `"auth:register"`. */
  readonly source?: string;
  /** Override password minimum length (must match server policy). */
  readonly passwordMinLength?: number;
  /** Called once the session is established. */
  readonly onSuccess?: (session: AuthSession) => void;
  /**
   * If `true`, render a friendly "registration disabled" banner instead
   * of the form (admin uses this).
   */
  readonly disabled?: boolean;
  /** Slot under submit / disabled banner — typically a "Sign in" link. */
  readonly secondaryAction?: ReactNode;
}

export const RegisterForm = ({
  source = "auth:register",
  passwordMinLength,
  onSuccess,
  disabled = false,
  secondaryAction,
}: RegisterFormProps) => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const form = useRegisterForm({
    source,
    ...(passwordMinLength !== undefined ? { passwordMinLength } : {}),
    ...(onSuccess !== undefined ? { onSuccess } : {}),
  });

  if (disabled) {
    return (
      <div className="flex w-full flex-col gap-5">
        <div
          role="status"
          className="flex flex-col gap-1 rounded-lg border border-border bg-bg-muted/60 p-4 text-sm text-fg"
        >
          <strong className="font-semibold">
            {t("register.disabled.title")}
          </strong>
          <span className="text-fg-muted">
            {t("register.disabled.message")}
          </span>
        </div>
        {secondaryAction !== undefined && (
          <div className="flex flex-col items-center gap-2 text-sm text-fg-muted">
            {secondaryAction}
          </div>
        )}
      </div>
    );
  }

  if (form.status === "success") {
    return (
      <div
        role="status"
        className="flex w-full flex-col items-start gap-2 rounded-lg border border-primary/30 bg-primary/5 p-5"
      >
        <strong className="font-display text-lg font-semibold text-fg">
          {t("register.form.success.title")}
        </strong>
        <p className="text-sm text-fg-muted">
          {t("register.form.success.message", {
            email: form.values.email.trim(),
          })}
        </p>
      </div>
    );
  }

  const submitting = form.status === "submitting";
  const showError = form.status === "error" && form.errorCode !== undefined;
  const passwordPlaceholder = t("register.form.password.placeholder", {
    min: form.passwordMinLength,
  });

  return (
    <form
      noValidate
      onSubmit={form.handleSubmit}
      className="flex w-full flex-col gap-5"
      aria-describedby={showError ? "register-form-error" : undefined}
    >
      <TextField
        label={t("register.form.name.label")}
        placeholder={t("register.form.name.placeholder")}
        autoComplete="name"
        required
        value={form.values.name}
        onChange={(value) => {
          form.setField("name", value);
        }}
        {...(form.errors.name !== undefined
          ? { error: form.errors.name }
          : {})}
      />

      <TextField
        label={t("register.form.email.label")}
        placeholder={t("register.form.email.placeholder")}
        type="email"
        autoComplete="email"
        required
        value={form.values.email}
        onChange={(value) => {
          form.setField("email", value);
        }}
        {...(form.errors.email !== undefined
          ? { error: form.errors.email }
          : {})}
      />

      <PasswordField
        label={t("register.form.password.label")}
        placeholder={passwordPlaceholder}
        autoComplete="new-password"
        required
        value={form.values.password}
        onChange={(value) => {
          form.setField("password", value);
        }}
        showLabel={t("common.passwordVisibility.show")}
        hideLabel={t("common.passwordVisibility.hide")}
        {...(form.errors.password !== undefined
          ? { error: form.errors.password }
          : {})}
      />

      <PasswordField
        label={t("register.form.confirmPassword.label")}
        placeholder={t("register.form.confirmPassword.placeholder")}
        autoComplete="new-password"
        required
        value={form.values.confirmPassword}
        onChange={(value) => {
          form.setField("confirmPassword", value);
        }}
        showLabel={t("common.passwordVisibility.show")}
        hideLabel={t("common.passwordVisibility.hide")}
        {...(form.errors.confirmPassword !== undefined
          ? { error: form.errors.confirmPassword }
          : {})}
      />

      {showError && (
        <div
          id="register-form-error"
          role="alert"
          className="flex items-start gap-3 rounded-lg border border-danger-500/30 bg-danger-500/10 p-4 text-sm text-fg"
        >
          <span aria-hidden="true" className="mt-0.5 text-danger-500">
            {"!"}
          </span>
          <div className="flex flex-col gap-1">
            <strong className="font-semibold">
              {t("register.form.error.title")}
            </strong>
            <span>
              {t(`register.form.error.${form.errorCode ?? "server"}`)}
            </span>
          </div>
        </div>
      )}

      <button
        type="submit"
        disabled={submitting}
        className={cn(
          buttonStyles({ variant: "primary", size: "md", fullWidth: true }),
        )}
      >
        {submitting
          ? t("register.form.submitting")
          : t("register.form.submit")}
      </button>

      {secondaryAction !== undefined && (
        <div className="flex flex-col items-center gap-2 text-sm text-fg-muted">
          {secondaryAction}
        </div>
      )}
    </form>
  );
};
