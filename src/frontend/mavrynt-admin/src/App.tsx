import { useState } from "react";
import { Button, Container, Section, Stack, ThemeProvider } from "@mavrynt/ui";

/**
 * Phase 1 placeholder for `mavrynt-admin`.
 *
 * Wires the admin SPA into the shared design system. Keeps the original
 * `/api/ping` smoke test pointed at `Mavrynt.AdminApp` (proxy in
 * `vite.config.ts`).
 */
const App = () => {
  const [result, setResult] = useState<string>("not called");

  const callApi = async (): Promise<void> => {
    try {
      const response = await fetch("/api/ping");
      const data = (await response.json()) as { message?: string };
      setResult(data.message ?? "no message");
    } catch (error) {
      console.error("Admin API call failed", error);
      setResult("failed");
    }
  };

  return (
    <ThemeProvider defaultMode="system">
      <Section spacing="md">
        <Container size="md">
          <Stack gap={6} align="start">
            <h1 className="font-display text-4xl font-semibold tracking-tight text-fg">
              Mavrynt Admin
            </h1>
            <p className="text-fg-muted">
              Admin SPA. Shared design system + admin API smoke test.
            </p>
            <Stack direction="row" gap={3} align="center" wrap>
              <Button
                onClick={() => {
                  void callApi();
                }}
              >
                Call Admin API
              </Button>
              <span className="text-sm text-fg-muted">Result: {result}</span>
            </Stack>
          </Stack>
        </Container>
      </Section>
    </ThemeProvider>
  );
};

export default App;
