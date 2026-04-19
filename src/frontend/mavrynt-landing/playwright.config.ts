import { defineConfig, devices } from "@playwright/test";

/**
 * Playwright config for mavrynt-landing smoke tests.
 *
 * Scope (Phase 4e): three end-to-end user journeys that exercise the
 * shell-to-page routing, the contact-form submission pipeline, the
 * native FAQ accordion, and the i18n switcher. This is deliberately
 * NOT full regression — unit/integration behaviour is owned by Vitest.
 *
 * Browser matrix: Chromium only. Smoke tests should catch "is the site
 * broken?" fast; cross-browser coverage belongs to a later phase once
 * the regression suite justifies the cost.
 *
 * webServer: `pnpm dev` for fast iteration. Switch to `pnpm preview`
 * (after `pnpm build`) if you want to validate the production bundle —
 * useful when touching vite.config.ts / chunking / compression.
 */
const PORT = 5173;
const BASE_URL = `http://127.0.0.1:${PORT}`;

export default defineConfig({
  testDir: "./tests/e2e",
  // Each spec is independent; parallelism per file is fine.
  fullyParallel: true,
  // Prevent accidental `test.only` landing in CI.
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
    // Pin locale + timezone so date/number/currency assertions are stable.
    // Polish is the default site locale; the browser's Accept-Language
    // here matches that so `i18next-browser-languagedetector` resolves
    // to "pl" on fresh visits. Per-test overrides can set a different
    // `locale` via test.use({ locale: "en-US" }).
    locale: "pl-PL",
    timezoneId: "Europe/Warsaw",
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  webServer: {
    command: "pnpm dev",
    url: BASE_URL,
    reuseExistingServer: !process.env.CI,
    timeout: 120_000,
    stdout: "pipe",
    stderr: "pipe",
  },
});
