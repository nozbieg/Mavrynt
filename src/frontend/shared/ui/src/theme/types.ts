/**
 * Theme types shared between the provider, the hook, and consumers.
 */
export type ThemeMode = "light" | "dark" | "system";

export type ResolvedTheme = "light" | "dark";

export interface ThemeContextValue {
  /** What the user picked. May be "system". */
  readonly mode: ThemeMode;
  /** What is actually applied right now. Always "light" or "dark". */
  readonly resolved: ResolvedTheme;
  /** Switch the user's preferred mode. Persists to localStorage. */
  readonly setMode: (mode: ThemeMode) => void;
}

export const THEME_STORAGE_KEY = "mavrynt:theme";
export const THEME_DATA_ATTRIBUTE = "data-theme";
