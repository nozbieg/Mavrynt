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
 * useLoginForm — tiny state machine for the login form.
 *
 * Deliberately dependency-free (no Zod / RHF): the form has two fields
 * and simple rules. Mirrors `useContactForm` from `mavrynt-landing` so
 * future refactors (swap for Zod + RHF behind the same returned API)
 * touch all forms in one place.
 */

export interface LoginFormValues {
  email: string;
  password: string;
}

export type LoginField = keyof LoginFormValues;

export type LoginStatus = "idle" | "submitting" | "success" | "error";

export interface LoginFormErrors {
  readonly email?: string;
  readonly password?: string;
}

const PASSWORD_MIN_LENGTH = 8;
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const EMPTY_VALUES: LoginFormValues = {
  email: "",
  password: "",
};

export interface UseLoginFormOptions {
  /** Free-form source tag, e.g. `"web:login"` or `"admin:login"`. */
  readonly source: string;
  /**
   * Called once on successful login with the resulting session.
   * Apps decide what to do (redirect, store token, refresh user query).
   */
  readonly onSuccess?: (session: AuthSession) => void;
  /** Override the password minimum length validation (default 8). */
  readonly passwordMinLength?: number;
}

export interface UseLoginFormResult {
  readonly values: LoginFormValues;
  readonly errors: LoginFormErrors;
  readonly status: LoginStatus;
  readonly errorCode: AuthErrorCode | undefined;
  readonly setField: (field: LoginField, value: string) => void;
  readonly handleSubmit: (event: FormEvent<HTMLFormElement>) => void;
  readonly reset: () => void;
  readonly passwordMinLength: number;
}

export const useLoginForm = (
  options: UseLoginFormOptions,
): UseLoginFormResult => {
  const { source, onSuccess } = options;
  const passwordMinLength = options.passwordMinLength ?? PASSWORD_MIN_LENGTH;

  const { t, i18n } = useTranslation(AUTH_I18N_NAMESPACE);
  const analytics = useAuthAnalytics();
  const auth = useAuthService();

  const [values, setValues] = useState<LoginFormValues>(EMPTY_VALUES);
  const [errors, setErrors] = useState<LoginFormErrors>({});
  const [status, setStatus] = useState<LoginStatus>("idle");
  const [errorCode, setErrorCode] = useState<AuthErrorCode | undefined>(
    undefined,
  );

  const validate = useCallback(
    (input: LoginFormValues): LoginFormErrors => {
      const out: { email?: string; password?: string } = {};
      if (input.email.trim().length === 0) {
        out.email = t("login.form.validation.required");
      } else if (!EMAIL_PATTERN.test(input.email.trim())) {
        out.email = t("login.form.validation.email");
      }
      if (input.password.length === 0) {
        out.password = t("login.form.validation.required");
      } else if (input.password.length < passwordMinLength) {
        out.password = t("login.form.validation.passwordMin", {
          min: passwordMinLength,
        });
      }
      return out;
    },
    [t, passwordMinLength],
  );

  const setField = useCallback<UseLoginFormResult["setField"]>(
    (field, value) => {
      setValues((prev) => ({ ...prev, [field]: value }));
      setErrors((prev) => {
        if (prev[field] === undefined) return prev;
        const next = { ...prev };
        delete next[field];
        return next;
      });
      // A typed input means the user is recovering from a previous failure;
      // clear the global error banner so it doesn't shout while they fix it.
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

  const handleSubmit = useCallback<UseLoginFormResult["handleSubmit"]>(
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
      analytics.track("auth_login_attempt", { source });

      void (async () => {
        try {
          const session = await auth.login({
            email: values.email.trim(),
            password: values.password,
            source,
            locale: i18n.language,
          });
          setStatus("success");
          analytics.track("auth_login_success", { source });
          onSuccess?.(session);
        } catch (error) {
          const code: AuthErrorCode =
            error instanceof AuthError ? error.code : "server";
          setStatus("error");
          setErrorCode(code);
          analytics.track("auth_login_error", { source, code });
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
