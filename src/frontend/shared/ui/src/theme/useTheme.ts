import { useContext } from "react";
import { ThemeContext } from "./ThemeProvider.tsx";
import type { ThemeContextValue } from "./types.ts";

/**
 * useTheme — read and mutate the current theme.
 * Throws if used outside `<ThemeProvider>` to surface integration mistakes
 * early instead of silently falling back to defaults.
 */
export const useTheme = (): ThemeContextValue => {
  const ctx = useContext(ThemeContext);
  if (ctx === undefined) {
    throw new Error("useTheme must be used inside <ThemeProvider>");
  }
  return ctx;
};
