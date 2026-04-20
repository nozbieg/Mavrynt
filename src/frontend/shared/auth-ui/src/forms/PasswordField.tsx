import {
  useId,
  useState,
  type ChangeEvent,
  type InputHTMLAttributes,
} from "react";
import { cn } from "@mavrynt/ui";

/**
 * Password input with show/hide toggle. Shares the visual + a11y rules
 * of `TextField` (label / aria-invalid / inline error), but adds a
 * trailing toggle button that flips the input `type` between `password`
 * and `text`.
 *
 * The toggle button labels (`showLabel` / `hideLabel`) are passed in
 * already translated, keeping this component i18n-namespace-free.
 */
export interface PasswordFieldProps
  extends Omit<
    InputHTMLAttributes<HTMLInputElement>,
    "onChange" | "value" | "id" | "type"
  > {
  readonly label: string;
  readonly value: string;
  readonly onChange: (next: string) => void;
  readonly error?: string;
  readonly required?: boolean;
  readonly requiredMark?: string;
  readonly showLabel: string;
  readonly hideLabel: string;
}

export const PasswordField = ({
  label,
  value,
  onChange,
  error,
  required = false,
  requiredMark = "*",
  showLabel,
  hideLabel,
  className,
  autoComplete = "current-password",
  ...rest
}: PasswordFieldProps) => {
  const reactId = useId();
  const inputId = `${reactId}-field`;
  const errorId = `${inputId}-error`;
  const [visible, setVisible] = useState<boolean>(false);
  const hasError = error !== undefined && error.length > 0;

  const inputClass = cn(
    "w-full rounded-md border bg-bg pl-3 pr-10 py-2 text-fg placeholder:text-fg-muted/70",
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
      <div className="relative">
        <input
          id={inputId}
          type={visible ? "text" : "password"}
          value={value}
          onChange={handleChange}
          aria-invalid={hasError}
          aria-describedby={hasError ? errorId : undefined}
          required={required}
          autoComplete={autoComplete}
          className={inputClass}
          {...rest}
        />
        <button
          type="button"
          onClick={() => {
            setVisible((prev) => !prev);
          }}
          aria-label={visible ? hideLabel : showLabel}
          aria-pressed={visible}
          className={cn(
            "absolute inset-y-0 right-0 flex items-center px-3 text-xs font-medium text-fg-muted",
            "hover:text-fg focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring rounded-r-md",
          )}
        >
          {visible ? hideLabel : showLabel}
        </button>
      </div>
      {hasError && (
        <span id={errorId} role="alert" className="text-xs text-danger-500">
          {error}
        </span>
      )}
    </div>
  );
};
