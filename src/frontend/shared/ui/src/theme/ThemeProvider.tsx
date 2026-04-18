import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import {
  THEME_DATA_ATTRIBUTE,
  THEME_STORAGE_KEY,
  type ResolvedTheme,
  type ThemeContextValue,
  type ThemeMode,
} from "./types.ts";

/**
 * ThemeProvider — applies the active theme to <html> via `data-theme`,
 * persists the user's preference, and reacts to OS theme changes when
 * the user's preference is "system".
 *
 * SOLID:
 * - Single Responsibility: only theme state + side effects on <html>.
 * - Open/Closed: extra theme variants can be added without touching this
 *   component (set new modes in design-tokens, change the type union).
 */

export const ThemeContext = createContext<ThemeContextValue | undefined>(
  undefined,
);

export interface ThemeProviderProps {
  readonly defaultMode?: ThemeMode;
  readonly children: ReactNode;
  /** Element that receives the `data-theme` attribute. Defaults to <html>. */
  readonly target?: HTMLElement;
}

const readStoredMode = (): ThemeMode | undefined => {
  if (typeof window === "undefined") return undefined;
  try {
    const value = window.localStorage.getItem(THEME_STORAGE_KEY);
    if (value === "light" || value === "dark" || value === "system") {
      return value;
    }
  } catch {
    /* localStorage may be blocked */
  }
  return undefined;
};

const resolveMode = (mode: ThemeMode): ResolvedTheme => {
  if (mode !== "system") return mode;
  if (typeof window === "undefined") return "light";
  return window.matchMedia("(prefers-color-scheme: dark)").matches
    ? "dark"
    : "light";
};

const applyTheme = (resolved: ResolvedTheme, target?: HTMLElement): void => {
  if (typeof document === "undefined") return;
  const el = target ?? document.documentElement;
  el.setAttribute(THEME_DATA_ATTRIBUTE, resolved);
};

export const ThemeProvider = ({
  defaultMode = "system",
  children,
  target,
}: ThemeProviderProps) => {
  const [mode, setModeState] = useState<ThemeMode>(
    () => readStoredMode() ?? defaultMode,
  );
  const [resolved, setResolved] = useState<ResolvedTheme>(() =>
    resolveMode(mode),
  );

  // Apply on mode change
  useEffect(() => {
    const next = resolveMode(mode);
    setResolved(next);
    applyTheme(next, target);
  }, [mode, target]);

  // Track OS changes when the user is on "system"
  useEffect(() => {
    if (mode !== "system" || typeof window === "undefined") return;
    const mq = window.matchMedia("(prefers-color-scheme: dark)");
    const handler = (event: MediaQueryListEvent): void => {
      const next: ResolvedTheme = event.matches ? "dark" : "light";
      setResolved(next);
      applyTheme(next, target);
    };
    mq.addEventListener("change", handler);
    return () => {
      mq.removeEventListener("change", handler);
    };
  }, [mode, target]);

  const setMode = useCallback((next: ThemeMode): void => {
    setModeState(next);
    if (typeof window === "undefined") return;
    try {
      window.localStorage.setItem(THEME_STORAGE_KEY, next);
    } catch {
      /* non-fatal */
    }
  }, []);

  const value = useMemo<ThemeContextValue>(
    () => ({ mode, resolved, setMode }),
    [mode, resolved, setMode],
  );

  return (
    <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>
  );
};
