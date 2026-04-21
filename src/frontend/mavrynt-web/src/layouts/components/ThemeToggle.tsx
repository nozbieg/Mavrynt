import { useTranslation } from "react-i18next";
import { Button, useTheme } from "@mavrynt/ui";

/**
 * ThemeToggle — cycles light → dark → system. Uses the shared
 * `useTheme` hook from `@mavrynt/ui` so every SPA picks up the same
 * persistence + `<html data-theme>` contract.
 */
const NEXT_MODE = {
  light: "dark",
  dark: "system",
  system: "light",
} as const;

export const ThemeToggle = () => {
  const { t } = useTranslation();
  const { mode, setMode } = useTheme();
  const next = NEXT_MODE[mode];

  return (
    <Button
      variant="ghost"
      size="sm"
      aria-label={t("theme.toggle")}
      title={t("theme.toggle")}
      onClick={() => {
        setMode(next);
      }}
    >
      <span aria-hidden="true">
        {mode === "light" ? "☀" : mode === "dark" ? "☾" : "⌬"}
      </span>
      <span className="ml-2 hidden sm:inline">{t(`theme.${mode}`)}</span>
    </Button>
  );
};
