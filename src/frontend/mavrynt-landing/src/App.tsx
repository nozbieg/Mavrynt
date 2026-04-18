import { Button, Container, Section, Stack, ThemeProvider, useTheme } from "@mavrynt/ui";

/**
 * Phase 1 placeholder for `mavrynt-landing`.
 *
 * This file exists to verify that the shared workspace wiring works end
 * to end (tokens load, primitives render, theme toggling switches
 * `data-theme` on <html>). Phase 2 replaces it with the proper feature
 * folder layout and React Router setup.
 */

const ThemeSwitcher = () => {
  const { mode, resolved, setMode } = useTheme();
  const next = resolved === "dark" ? "light" : "dark";
  return (
    <Button
      variant="secondary"
      size="sm"
      onClick={() => {
        setMode(next);
      }}
      aria-label={`Switch to ${next} theme`}
    >
      {resolved === "dark" ? "Dark" : "Light"} ({mode})
    </Button>
  );
};

const App = () => (
  <ThemeProvider defaultMode="system">
    <Section spacing="lg" tone="subtle">
      <Container size="md">
        <Stack gap={6} align="start">
          <span className="rounded-full bg-bg-muted px-3 py-1 text-xs font-medium uppercase tracking-wide text-fg-muted">
            Mavrynt — landing
          </span>
          <h1 className="font-display text-5xl font-semibold tracking-tight text-fg sm:text-6xl">
            Shared design system, ready.
          </h1>
          <p className="max-w-2xl text-lg text-fg-muted">
            Phase 1 is wired: design tokens, theme provider, primitives and
            layout shells are all sourced from the workspace. Phase 2 adds
            routing, i18n and the real marketing sections on top.
          </p>
          <Stack direction="row" gap={3} wrap>
            <Button variant="primary">Get started</Button>
            <Button variant="secondary">Read the docs</Button>
            <ThemeSwitcher />
          </Stack>
        </Stack>
      </Container>
    </Section>
  </ThemeProvider>
);

export default App;
