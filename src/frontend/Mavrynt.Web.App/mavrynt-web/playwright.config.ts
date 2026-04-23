import { defineConfig, devices } from "@playwright/test";

/**
 * Playwright config for `mavrynt-web` smoke tests.
 *
 * Scope (Phase 5): login + register happy-path and invalid-credentials
 * journeys against the default console `AuthService`. The console
 * adapter returns a mock session after 300 ms (see
 * `createConsoleAuthService`), which is fast and deterministic — good
 * enough for smoke. End-to-end coverage of the real Mavrynt.Api auth
 * endpoint is a later phase, once the Users module ships.
 *
 * Browser matrix: Chromium only. Smoke tests should answer "is the
 * auth UI broken?" fast; cross-browser belongs with a regression suite
 * that justifies the cost.
 *
 * webServer: `npm run dev` on port 5174 (web's assigned Vite port).
 * Switch to `npm run preview` after `npm run build` to validate the
 * production bundle — useful when touching vite.config.ts.
 */
const PORT = 5174;
const BASE_URL = `http://127.0.0.1:${PORT}`;

export default defineConfig({
  testDir: "./tests/e2e",
  fullyParallel: true,
  forbidOnly: Boolean(process.env.CI),
  retries: process.env.CI ? 1 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: process.env.CI ? [["github"], ["html", { open: "never" }]] : "list",
  timeout: 30_000,
  expect: {
    timeout: 5_000,
  },
  use: {
    baseURL: BASE_URL,
    trace: "on-first-retry",
    video: "retain-on-failure",
    screenshot: "only-on-failure",
    // Force English so the assertions match en.json copy without
    // depending on the detector's browser-language guess.
    locale: "en-US",
    timezoneId: "Europe/Warsaw",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  webServer: {
    command: "npm run dev",
    url: BASE_URL,
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
    stdout: "pipe",
    stderr: "pipe",
  },
});
