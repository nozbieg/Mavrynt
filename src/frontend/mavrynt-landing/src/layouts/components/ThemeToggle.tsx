import { useTranslation } from "react-i18next";
import { Button, useTheme } from "@mavrynt/ui";

/**
 * ThemeToggle — cycles light → dark → system. Label updates with the
 * active mode so screen-reader users know the current state.
 *
 * Why a cycle (not a dropdown): keeps the nav compact. A fuller
 * settings panel can replace this later without touching callers.
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
