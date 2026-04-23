import { useId, type ChangeEvent, type ReactNode } from "react";
import { useTranslation } from "react-i18next";
import { Stack, buttonStyles, cn } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";
import { siteConfig } from "../../content/site.ts";
import {
  useContactForm,
  type ContactField,
  type ContactFormErrors,
} from "./useContactForm.ts";

/**
 * ContactForm — main entry point for lead capture.
 *
 * The component stays pure-presentational: state, validation, and
 * side-effects all live in `useContactForm`. That separation keeps
 * the view trivially testable (render the hook return) and lets us
 * drop the form into any page that needs it (contact, pricing
 * "talk-to-sales", dedicated demo page) with one prop: `source`.
 */
export interface ContactFormProps {
  readonly source?: string;
}

export const ContactForm = ({ source = "landing:contact" }: ContactFormProps) => {
  const { t } = useTranslation();
  const form = useContactForm(source);

  if (form.status === "success") {
    return (
      <SuccessBanner
        title={t("contact.form.success.title")}
        message={t("contact.form.success.message")}
      />
    );
  }

  const submitting = form.status === "submitting";

  return (
    <form
      noValidate
      onSubmit={form.handleSubmit}
      className="flex w-full flex-col gap-5"
      aria-describedby={form.status === "error" ? "contact-form-error" : undefined}
    >
      <TextField
        field="name"
        autoComplete="name"
        required
        errors={form.errors}
        value={form.values.name}
        onChange={(value) => {
          form.setField("name", value);
        }}
      />
      <TextField
        field="email"
        type="email"
        autoComplete="email"
        required
        errors={form.errors}
        value={form.values.email}
        onChange={(value) => {
          form.setField("email", value);
        }}
      />
      <TextField
        field="company"
        autoComplete="organization"
        errors={form.errors}
        value={form.values.company}
        onChange={(value) => {
          form.setField("company", value);
        }}
      />
      <TextField
        field="message"
        multiline
        required
        errors={form.errors}
        value={form.values.message}
        onChange={(value) => {
          form.setField("message", value);
        }}
      />

      {form.status === "error" && (
        <div
          id="contact-form-error"
          role="alert"
          className="flex items-start gap-3 rounded-lg border border-danger-500/30 bg-danger-500/10 p-4 text-sm text-fg"
        >
          <Icon name="mail" size={18} className="mt-0.5 shrink-0 text-danger-500" />
          <div className="flex flex-col gap-1">
            <strong className="font-semibold">
              {t("contact.form.error.title")}
            </strong>
            <span>
              {t("contact.form.error.message", {
                email: siteConfig.contactEmail,
              })}
            </span>
          </div>
        </div>
      )}

      <Stack direction="row" gap={3} align="center" wrap>
        <button
          type="submit"
          disabled={submitting}
          className={cn(buttonStyles({ variant: "primary", size: "md" }))}
        >
          {submitting ? t("contact.form.submitting") : t("contact.form.submit")}
        </button>
        <span className="text-xs text-fg-muted">
          {t("contact.subtitle")}
        </span>
      </Stack>
    </form>
  );
};

/* ---------- local pieces (deliberately not exported) ---------- */

interface SuccessBannerProps {
  readonly title: ReactNode;
  readonly message: ReactNode;
}

const SuccessBanner = ({ title, message }: SuccessBannerProps) => (
  <div
    role="status"
    className="flex items-start gap-3 rounded-xl border border-primary/30 bg-primary/5 p-6"
  >
    <Icon name="check" size={22} className="mt-0.5 text-primary" />
    <div className="flex flex-col gap-1">
      <h3 className="font-display text-lg font-semibold text-fg">{title}</h3>
      <p className="text-sm text-fg-muted">{message}</p>
    </div>
  </div>
);

interface TextFieldProps {
  readonly field: ContactField;
  readonly value: string;
  readonly onChange: (value: string) => void;
  readonly errors: ContactFormErrors;
  readonly type?: "text" | "email";
  readonly multiline?: boolean;
  readonly required?: boolean;
  readonly autoComplete?: string;
}

const TextField = ({
  field,
  value,
  onChange,
  errors,
  type = "text",
  multiline = false,
  required = false,
  autoComplete,
}: TextFieldProps) => {
  const { t } = useTranslation();
  const reactId = useId();
  const inputId = `${reactId}-${field}`;
  const errorId = `${inputId}-error`;
  const error = field in errors ? errors[field as keyof ContactFormErrors] : undefined;
  const hasError = error !== undefined;

  const commonClass = cn(
    "w-full rounded-md border bg-bg px-3 py-2 text-fg placeholder:text-fg-muted/70 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg",
    hasError ? "border-danger-500/60" : "border-border",
  );

  const handleChange = (
    event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>,
  ): void => {
    onChange(event.target.value);
  };

  return (
    <div className="flex flex-col gap-1.5">
      <label
        htmlFor={inputId}
        className="text-sm font-medium text-fg"
      >
        {t(`contact.form.${field}.label`)}
        {required && (
          <span aria-hidden="true" className="ml-1 text-danger-500">
            *
          </span>
        )}
      </label>
      {multiline ? (
        <textarea
          id={inputId}
          name={field}
          value={value}
          onChange={handleChange}
          placeholder={t(`contact.form.${field}.placeholder`)}
          rows={5}
          aria-invalid={hasError}
          aria-describedby={hasError ? errorId : undefined}
          required={required}
          className={commonClass}
        />
      ) : (
        <input
          id={inputId}
          name={field}
          type={type}
          value={value}
          onChange={handleChange}
          placeholder={t(`contact.form.${field}.placeholder`)}
          aria-invalid={hasError}
          aria-describedby={hasError ? errorId : undefined}
          required={required}
          autoComplete={autoComplete}
          className={commonClass}
        />
      )}
      {hasError && (
        <span
          id={errorId}
          className="text-xs text-danger-500"
        >
          {error}
        </span>
      )}
    </div>
  );
};
