import { useCallback, useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useAuthAnalytics } from "../analytics/index.ts";
import { AUTH_I18N_NAMESPACE } from "../i18n/index.ts";
import {
  AuthError,
  type AuthErrorCode,
  type AuthSession,
} from "../service/types.ts";
import { useAuthService } from "../service/context.ts";

/**
 * useRegisterForm — tiny state machine for the register form.
 *
 * Same shape as `useLoginForm` (and `useContactForm` in landing) so the
 * components stay symmetric and predictable. Adds a `confirmPassword`
 * field with cross-field validation; everything else is identical.
 */

export interface RegisterFormValues {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export type RegisterField = keyof RegisterFormValues;

export type RegisterStatus = "idle" | "submitting" | "success" | "error";

export interface RegisterFormErrors {
  readonly name?: string;
  readonly email?: string;
  readonly password?: string;
  readonly confirmPassword?: string;
}

const PASSWORD_MIN_LENGTH = 8;
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const EMPTY_VALUES: RegisterFormValues = {
  name: "",
  email: "",
  password: "",
  confirmPassword: "",
};

export interface UseRegisterFormOptions {
  /** Free-form source tag, e.g. `"web:register"` or `"admin:register"`. */
  readonly source: string;
  /**
   * Called once on successful registration with the resulting session.
   * Apps decide what to do (redirect, store token, send to onboarding).
   */
  readonly onSuccess?: (session: AuthSession) => void;
  /** Override the password minimum length validation (default 8). */
  readonly passwordMinLength?: number;
}

export interface UseRegisterFormResult {
  readonly values: RegisterFormValues;
  readonly errors: RegisterFormErrors;
  readonly status: RegisterStatus;
  readonly errorCode: AuthErrorCode | undefined;
  readonly setField: (field: RegisterField, value: string) => void;
  readonly handleSubmit: (event: FormEvent<HTMLFormElement>) => void;
  readonly reset: () => void;
  readonly passwordMinLength: number;
}

export const useRegisterForm = (
  options: UseRegisterFormOptions,
): UseRegisterFormResult => {
  const { source, onSuccess } = options;
  const passwordMinLength = options.passwordMinLength ?? PASSWORD_MIN_LENGTH;

  const { t, i18n } = useTranslation(AUTH_I18N_NAMESPACE);
  const analytics = useAuthAnalytics();
  const auth = useAuthService();

  const [values, setValues] = useState<RegisterFormValues>(EMPTY_VALUES);
  const [errors, setErrors] = useState<RegisterFormErrors>({});
  const [status, setStatus] = useState<RegisterStatus>("idle");
  const [errorCode, setErrorCode] = useState<AuthErrorCode | undefined>(
    undefined,
  );

  const validate = useCallback(
    (input: RegisterFormValues): RegisterFormErrors => {
      const out: {
        name?: string;
        email?: string;
        password?: string;
        confirmPassword?: string;
      } = {};
      if (input.name.trim().length === 0) {
        out.name = t("register.form.validation.required");
      }
      if (input.email.trim().length === 0) {
        out.email = t("register.form.validation.required");
      } else if (!EMAIL_PATTERN.test(input.email.trim())) {
        out.email = t("register.form.validation.email");
      }
      if (input.password.length === 0) {
        out.password = t("register.form.validation.required");
      } else if (input.password.length < passwordMinLength) {
        out.password = t("register.form.validation.passwordMin", {
          min: passwordMinLength,
        });
      }
      if (input.confirmPassword.length === 0) {
        out.confirmPassword = t("register.form.validation.required");
      } else if (input.confirmPassword !== input.password) {
        out.confirmPassword = t("register.form.validation.passwordsDontMatch");
      }
      return out;
    },
    [t, passwordMinLength],
  );

  const setField = useCallback<UseRegisterFormResult["setField"]>(
    (field, value) => {
      setValues((prev) => ({ ...prev, [field]: value }));
      setErrors((prev) => {
        if (prev[field] === undefined) return prev;
        const next = { ...prev };
        delete next[field];
        return next;
      });
      if (errorCode !== undefined) {
        setErrorCode(undefined);
        setStatus((prev) => (prev === "error" ? "idle" : prev));
      }
    },
    [errorCode],
  );

  const reset = useCallback(() => {
    setValues(EMPTY_VALUES);
    setErrors({});
    setStatus("idle");
    setErrorCode(undefined);
  }, []);

  const handleSubmit = useCallback<UseRegisterFormResult["handleSubmit"]>(
    (event) => {
      event.preventDefault();
      if (status === "submitting") return;

      const nextErrors = validate(values);
      if (Object.keys(nextErrors).length > 0) {
        setErrors(nextErrors);
        return;
      }
      setErrors({});
      setErrorCode(undefined);
      setStatus("submitting");
      analytics.track("auth_register_attempt", { source });

      void (async () => {
        try {
          const session = await auth.register({
            name: values.name.trim(),
            email: values.email.trim(),
            password: values.password,
            source,
            locale: i18n.language,
          });
          setStatus("success");
          analytics.track("auth_register_success", { source });
          onSuccess?.(session);
        } catch (error) {
          const code: AuthErrorCode =
            error instanceof AuthError ? error.code : "server";
          setStatus("error");
          setErrorCode(code);
          analytics.track("auth_register_error", { source, code });
        }
      })();
    },
    [analytics, auth, i18n.language, onSuccess, source, status, validate, values],
  );

  return {
    values,
    errors,
    status,
    errorCode,
    setField,
    handleSubmit,
    reset,
    passwordMinLength,
  };
};
