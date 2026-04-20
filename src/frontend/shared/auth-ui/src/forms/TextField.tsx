import { useId, type ChangeEvent, type InputHTMLAttributes } from "react";
import { cn } from "@mavrynt/ui";

/**
 * Tiny presentational text input with label + error slot.
 *
 * Mirrors the visual + a11y conventions used by `mavrynt-landing`'s
 * contact form `TextField`, but is generic enough to be shared by both
 * login and register forms (and future auth flows like password reset).
 *
 * - i18n is handled by the caller; pass already-translated `label`,
 *   `placeholder`, and `error` strings. Keeping the component
 *   translation-free lets the auth-ui package stay free of namespace
 *   assumptions.
 * - Uses `aria-invalid` + `aria-describedby` to wire the error message
 *   to the input for screen readers.
 */
export interface TextFieldProps
  extends Omit<
    InputHTMLAttributes<HTMLInputElement>,
    "onChange" | "value" | "id"
  > {
  readonly label: string;
  readonly value: string;
  readonly onChange: (next: string) => void;
  readonly error?: string;
  readonly required?: boolean;
  readonly requiredMark?: string;
}

export const TextField = ({
  label,
  value,
  onChange,
  error,
  required = false,
  requiredMark = "*",
  type = "text",
  className,
  ...rest
}: TextFieldProps) => {
  const reactId = useId();
  const inputId = `${reactId}-field`;
  const errorId = `${inputId}-error`;
  const hasError = error !== undefined && error.length > 0;

  const inputClass = cn(
    "w-full rounded-md border bg-bg px-3 py-2 text-fg placeholder:text-fg-muted/70",
    "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg",
    hasError ? "border-danger-500/60" : "border-border",
    className,
  );

  const handleChange = (event: ChangeEvent<HTMLInputElement>): void => {
    onChange(event.target.value);
  };

  return (
    <div className="flex flex-col gap-1.5">
      <label htmlFor={inputId} className="text-sm font-medium text-fg">
        {label}
        {required && (
          <span aria-hidden="true" className="ml-1 text-danger-500">
            {requiredMark}
          </span>
        )}
      </label>
      <input
        id={inputId}
        type={type}
        value={value}
        onChange={handleChange}
        aria-invalid={hasError}
        aria-describedby={hasError ? errorId : undefined}
        required={required}
        className={inputClass}
        {...rest}
      />
      {hasError && (
        <span id={errorId} role="alert" className="text-xs text-danger-500">
          {error}
        </span>
      )}
    </div>
  );
};
