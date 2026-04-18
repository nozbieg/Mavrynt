import { useState } from "react";
import { Button, Container, Section, Stack, ThemeProvider } from "@mavrynt/ui";

/**
 * Phase 1 placeholder for `mavrynt-web`.
 *
 * Wires the user-facing SPA into the shared design system. Keeps the
 * existing `/api/ping` smoke test so backend orchestration via
 * `Mavrynt.AppHost` continues to verify end-to-end connectivity.
 */
const App = () => {
  const [result, setResult] = useState<string>("not called");

  const callApi = async (): Promise<void> => {
    try {
      const response = await fetch("/api/ping");
      const data = (await response.json()) as { message?: string };
      setResult(data.message ?? "no message");
    } catch (error) {
      console.error("API call failed", error);
      setResult("failed");
    }
  };

  return (
    <ThemeProvider defaultMode="system">
      <Section spacing="md">
        <Container size="md">
          <Stack gap={6} align="start">
            <h1 className="font-display text-4xl font-semibold tracking-tight text-fg">
              Mavrynt Web
            </h1>
            <p className="text-fg-muted">
              User-facing SPA. Shared design system + API smoke test.
            </p>
            <Stack direction="row" gap={3} align="center" wrap>
              <Button
                onClick={() => {
                  void callApi();
                }}
              >
                Call API
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
