import { useCallback, useState, type FormEvent } from "react";
import { useTranslation } from "react-i18next";
import { useAnalytics } from "../../lib/analytics/index.ts";
import { useLeadService, LeadSubmissionError } from "../../lib/lead/index.ts";

/**
 * useContactForm — tiny state machine for the contact form.
 *
 * Deliberately dependency-free (no Zod / RHF): the form has four
 * fields and simple rules, so 30 lines of hand-rolled validation are
 * cheaper than dragging in a validation library. If the form grows
 * past five fields, swap this for Zod + React Hook Form behind the
 * same returned API.
 */

export interface ContactFormValues {
  name: string;
  email: string;
  company: string;
  message: string;
}

export type ContactField = keyof ContactFormValues;

export type ContactStatus = "idle" | "submitting" | "success" | "error";

export interface ContactFormErrors {
  readonly name?: string;
  readonly email?: string;
  readonly message?: string;
}

const MESSAGE_MIN_LENGTH = 20;

/** Small, deliberate email regex — intentionally loose (RFC 5322 is not worth shipping). */
const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const EMPTY_VALUES: ContactFormValues = {
  name: "",
  email: "",
  company: "",
  message: "",
};

export interface UseContactFormResult {
  readonly values: ContactFormValues;
  readonly errors: ContactFormErrors;
  readonly status: ContactStatus;
  readonly setField: (field: ContactField, value: string) => void;
  readonly handleSubmit: (event: FormEvent<HTMLFormElement>) => void;
  readonly reset: () => void;
}

export const useContactForm = (source: string): UseContactFormResult => {
  const { t, i18n } = useTranslation();
  const analytics = useAnalytics();
  const leadService = useLeadService();

  const [values, setValues] = useState<ContactFormValues>(EMPTY_VALUES);
  const [errors, setErrors] = useState<ContactFormErrors>({});
  const [status, setStatus] = useState<ContactStatus>("idle");

  const validate = useCallback(
    (input: ContactFormValues): ContactFormErrors => {
      const out: {
        name?: string;
        email?: string;
        message?: string;
      } = {};
      if (input.name.trim().length === 0) {
        out.name = t("contact.form.validation.required");
      }
      if (input.email.trim().length === 0) {
        out.email = t("contact.form.validation.required");
      } else if (!EMAIL_PATTERN.test(input.email.trim())) {
        out.email = t("contact.form.validation.email");
      }
      if (input.message.trim().length === 0) {
        out.message = t("contact.form.validation.required");
      } else if (input.message.trim().length < MESSAGE_MIN_LENGTH) {
        out.message = t("contact.form.validation.minLength", {
          min: MESSAGE_MIN_LENGTH,
        });
      }
      return out;
    },
    [t],
  );

  const setField = useCallback<UseContactFormResult["setField"]>(
    (field, value) => {
      setValues((prev) => ({ ...prev, [field]: value }));
      // Clear the error for this field as the user types — reduces visual noise.
      if (field === "name" || field === "email" || field === "message") {
        setErrors((prev) => {
          if (prev[field] === undefined) return prev;
          const next = { ...prev };
          delete next[field];
          return next;
        });
      }
    },
    [],
  );

  const reset = useCallback(() => {
    setValues(EMPTY_VALUES);
    setErrors({});
    setStatus("idle");
  }, []);

  const handleSubmit = useCallback<UseContactFormResult["handleSubmit"]>(
    (event) => {
      event.preventDefault();
      if (status === "submitting") return;

      const nextErrors = validate(values);
      if (Object.keys(nextErrors).length > 0) {
        setErrors(nextErrors);
        return;
      }
      setErrors({});
      setStatus("submitting");
      analytics.track("lead_submit_attempt", { source });

      void (async () => {
        try {
          const company = values.company.trim();
          // `exactOptionalPropertyTypes` forbids passing explicit `undefined`;
          // build the payload with `company` only when present.
          const basePayload = {
            name: values.name.trim(),
            email: values.email.trim(),
            message: values.message.trim(),
            source,
            locale: i18n.language,
          };
          await leadService.submit(
            company.length > 0 ? { ...basePayload, company } : basePayload,
          );
          setStatus("success");
          analytics.track("lead_submit_success", { source });
        } catch (error) {
          setStatus("error");
          const code =
            error instanceof LeadSubmissionError ? error.code : "server";
          analytics.track("lead_submit_error", { source, code });
        }
      })();
    },
    [analytics, i18n.language, leadService, source, status, validate, values],
  );

  return { values, errors, status, setField, handleSubmit, reset };
};
