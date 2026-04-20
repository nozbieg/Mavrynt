import type { ReactNode } from "react";
import { useTranslation } from "react-i18next";
import { buttonStyles, cn } from "@mavrynt/ui";
import { AUTH_I18N_NAMESPACE } from "../i18n/index.ts";
import type { AuthSession } from "../service/types.ts";
import { TextField } from "./TextField.tsx";
import { PasswordField } from "./PasswordField.tsx";
import { useLoginForm } from "./useLoginForm.ts";

/**
 * LoginForm — pure-presentational form bound to `useLoginForm`.
 *
 * Same separation as `mavrynt-landing`'s `ContactForm`: state, validation,
 * and side-effects live in the hook so the view stays trivially testable
 * and can be dropped into any page that needs a login surface (web SPA,
 * admin SPA, embedded modal, etc).
 *
 * Apps inject behaviour via:
 *  - `<AuthServiceContext.Provider>` — the actual login adapter
 *  - `<AuthAnalyticsContext.Provider>` — optional event tracking
 *  - `secondaryAction` slot — typically a `<RouterLink to="/register">`
 *    or a "Back to mavrynt.com" link, depending on the host app
 */
export interface LoginFormProps {
  /** Source tag for analytics / audit. Default `"auth:login"`. */
  readonly source?: string;
  /** Override password minimum length (must match server policy). */
  readonly passwordMinLength?: number;
  /** Called once the session is established. */
  readonly onSuccess?: (session: AuthSession) => void;
  /**
   * Slot rendered under the submit button — usually links to register
   * or back to the marketing site. Routing-agnostic (`ReactNode`), so
   * each app passes its own router-aware components.
   */
  readonly secondaryAction?: ReactNode;
}

export const LoginForm = ({
  source = "auth:login",
  passwordMinLength,
  onSuccess,
  secondaryAction,
}: LoginFormProps) => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const form = useLoginForm({
    source,
    ...(passwordMinLength !== undefined ? { passwordMinLength } : {}),
    ...(onSuccess !== undefined ? { onSuccess } : {}),
  });

  const submitting = form.status === "submitting";
  const showError = form.status === "error" && form.errorCode !== undefined;

  return (
    <form
      noValidate
      onSubmit={form.handleSubmit}
      className="flex w-full flex-col gap-5"
      aria-describedby={showError ? "login-form-error" : undefined}
    >
      <TextField
        label={t("login.form.email.label")}
        placeholder={t("login.form.email.placeholder")}
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
        label={t("login.form.password.label")}
        placeholder={t("login.form.password.placeholder")}
        autoComplete="current-password"
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

      {showError && (
        <div
          id="login-form-error"
          role="alert"
          className="flex items-start gap-3 rounded-lg border border-danger-500/30 bg-danger-500/10 p-4 text-sm text-fg"
        >
          <span aria-hidden="true" className="mt-0.5 text-danger-500">
            {"!"}
          </span>
          <div className="flex flex-col gap-1">
            <strong className="font-semibold">
              {t("login.form.error.title")}
            </strong>
            <span>{t(`login.form.error.${form.errorCode ?? "server"}`)}</span>
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
        {submitting ? t("login.form.submitting") : t("login.form.submit")}
      </button>

      {secondaryAction !== undefined && (
        <div className="flex flex-col items-center gap-2 text-sm text-fg-muted">
          {secondaryAction}
        </div>
      )}
    </form>
  );
};
