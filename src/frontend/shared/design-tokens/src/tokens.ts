/**
 * Mavrynt design tokens — typed mirror of the CSS custom properties declared
 * in `./styles/tokens.css`. Keeping the same values here lets non-styling
 * consumers (charts, canvas, tests) read the brand palette without parsing
 * CSS. Whenever a token in the CSS file changes, update this file as well.
 *
 * Tokens are intentionally **semantic-free** — semantic mapping (surface,
 * primary, etc.) lives in CSS, where it can switch with the active theme.
 */

export const colors = {
  brand: {
    50: "oklch(0.97 0.02 255)",
    100: "oklch(0.94 0.04 255)",
    200: "oklch(0.88 0.07 255)",
    300: "oklch(0.80 0.11 255)",
    400: "oklch(0.71 0.16 255)",
    500: "oklch(0.62 0.20 255)",
    600: "oklch(0.54 0.21 255)",
    700: "oklch(0.46 0.19 255)",
    800: "oklch(0.38 0.16 255)",
    900: "oklch(0.30 0.12 255)",
    950: "oklch(0.22 0.08 255)",
  },
  neutral: {
    0: "oklch(1 0 0)",
    50: "oklch(0.985 0 0)",
    100: "oklch(0.97 0 0)",
    200: "oklch(0.93 0 0)",
    300: "oklch(0.87 0 0)",
    400: "oklch(0.71 0 0)",
    500: "oklch(0.55 0 0)",
    600: "oklch(0.44 0 0)",
    700: "oklch(0.34 0 0)",
    800: "oklch(0.24 0 0)",
    900: "oklch(0.16 0 0)",
    950: "oklch(0.10 0 0)",
  },
  success: {
    500: "oklch(0.66 0.18 150)",
    600: "oklch(0.58 0.18 150)",
  },
  warning: {
    500: "oklch(0.78 0.16 80)",
    600: "oklch(0.68 0.18 70)",
  },
  danger: {
    500: "oklch(0.62 0.22 25)",
    600: "oklch(0.54 0.22 25)",
  },
} as const;

export const spacing = {
  px: "1px",
  0: "0",
  1: "0.25rem",
  2: "0.5rem",
  3: "0.75rem",
  4: "1rem",
  5: "1.25rem",
  6: "1.5rem",
  8: "2rem",
  10: "2.5rem",
  12: "3rem",
  16: "4rem",
  20: "5rem",
  24: "6rem",
  32: "8rem",
  section: "6rem",
  "section-sm": "4rem",
  "section-lg": "8rem",
} as const;

export const radii = {
  none: "0",
  sm: "0.25rem",
  md: "0.5rem",
  lg: "0.75rem",
  xl: "1rem",
  "2xl": "1.5rem",
  full: "9999px",
} as const;

export const typography = {
  fontFamily: {
    sans: '"Inter", "Segoe UI", system-ui, -apple-system, sans-serif',
    display:
      '"Inter Display", "Inter", "Segoe UI", system-ui, -apple-system, sans-serif',
    mono: '"JetBrains Mono", ui-monospace, SFMono-Regular, Menlo, monospace',
  },
  fontSize: {
    xs: "0.75rem",
    sm: "0.875rem",
    base: "1rem",
    lg: "1.125rem",
    xl: "1.25rem",
    "2xl": "1.5rem",
    "3xl": "1.875rem",
    "4xl": "2.25rem",
    "5xl": "3rem",
    "6xl": "3.75rem",
    "7xl": "4.5rem",
  },
  lineHeight: {
    tight: "1.2",
    snug: "1.4",
    normal: "1.6",
  },
  fontWeight: {
    regular: "400",
    medium: "500",
    semibold: "600",
    bold: "700",
  },
} as const;

export const breakpoints = {
  xs: "480px",
  sm: "640px",
  md: "768px",
  lg: "1024px",
  xl: "1280px",
  "2xl": "1536px",
} as const;

export const shadows = {
  sm: "0 1px 2px rgb(0 0 0 / 0.06)",
  md: "0 4px 12px rgb(0 0 0 / 0.08)",
  lg: "0 12px 32px rgb(0 0 0 / 0.10)",
  xl: "0 24px 48px rgb(0 0 0 / 0.12)",
} as const;

export const motion = {
  duration: {
    fast: "120ms",
    base: "200ms",
    slow: "320ms",
  },
  easing: {
    standard: "cubic-bezier(0.2, 0, 0, 1)",
    emphasized: "cubic-bezier(0.3, 0, 0, 1)",
  },
} as const;

export const tokens = {
  colors,
  spacing,
  radii,
  typography,
  breakpoints,
  shadows,
  motion,
} as const;

export type Tokens = typeof tokens;
