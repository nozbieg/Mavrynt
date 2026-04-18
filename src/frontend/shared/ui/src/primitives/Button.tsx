import { forwardRef, type ButtonHTMLAttributes, type ReactNode } from "react";
import { cn } from "./cn.ts";

/**
 * Button — accessible, themeable, variant-driven.
 *
 * Variants are pure visual concerns; behaviour stays in callers.
 * Use `<a className={buttonStyles({ variant: ... })}>` if you need a link
 * styled as a button — the styles function is exported for that.
 */

export type ButtonVariant = "primary" | "secondary" | "ghost" | "danger";
export type ButtonSize = "sm" | "md" | "lg";

export interface ButtonStyleOptions {
  readonly variant?: ButtonVariant;
  readonly size?: ButtonSize;
  readonly fullWidth?: boolean;
}

const baseClasses =
  "inline-flex items-center justify-center font-medium rounded-md transition-colors " +
  "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 " +
  "focus-visible:ring-offset-bg disabled:opacity-50 disabled:cursor-not-allowed select-none whitespace-nowrap";

const variantClasses: Record<ButtonVariant, string> = {
  primary:
    "bg-primary text-primary-fg hover:bg-primary-hover active:bg-primary-hover",
  secondary:
    "bg-bg-muted text-fg hover:bg-bg-subtle border border-border",
  ghost: "bg-transparent text-fg hover:bg-bg-muted",
  danger:
    "bg-danger-500 text-neutral-0 hover:bg-danger-600 active:bg-danger-600",
};

const sizeClasses: Record<ButtonSize, string> = {
  sm: "h-9 px-3 text-sm gap-1.5",
  md: "h-11 px-5 text-base gap-2",
  lg: "h-12 px-6 text-lg gap-2",
};

export const buttonStyles = ({
  variant = "primary",
  size = "md",
  fullWidth = false,
}: ButtonStyleOptions = {}): string =>
  cn(
    baseClasses,
    variantClasses[variant],
    sizeClasses[size],
    fullWidth && "w-full",
  );

export interface ButtonProps
  extends ButtonHTMLAttributes<HTMLButtonElement>,
    ButtonStyleOptions {
  readonly leadingIcon?: ReactNode;
  readonly trailingIcon?: ReactNode;
}

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      variant,
      size,
      fullWidth,
      leadingIcon,
      trailingIcon,
      className,
      children,
      type = "button",
      ...rest
    },
    ref,
  ) => (
    <button
      ref={ref}
      type={type}
      className={cn(buttonStyles({ variant, size, fullWidth }), className)}
      {...rest}
    >
      {leadingIcon}
      {children}
      {trailingIcon}
    </button>
  ),
);

Button.displayName = "Button";
