import { defineConfig, mergeConfig } from "vitest/config";
import viteConfig from "./vite.config.ts";

/**
 * Vitest configuration for `mavrynt-landing`.
 *
 * We reuse the full Vite config (tailwindcss, react, aliases) via
 * `mergeConfig` so tests resolve modules exactly the same way the
 * runtime SPA does. This keeps the DRY promise from Phase 2 — there
 * is one module-resolution truth per app.
 *
 * The test environment is `jsdom` because the landing SPA renders
 * marketing content that must exercise DOM APIs (forms, <details>,
 * focus management). `setupFiles` wires `@testing-library/jest-dom`
 * matchers and auto-cleanup after each test.
 */
export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: "jsdom",
      globals: false,
      css: false,
      setupFiles: ["./src/test/setup.ts"],
      include: ["src/**/*.{test,spec}.{ts,tsx}"],
      // Excluded by default in Vitest, but declare explicitly so future
      // Playwright specs under `tests/e2e/` never leak into the unit run.
      exclude: ["node_modules", "dist", "tests/e2e/**"],
      clearMocks: true,
      restoreMocks: true,
      unstubGlobals: true,
      coverage: {
        provider: "v8",
        reporter: ["text", "html"],
        include: ["src/**/*.{ts,tsx}"],
        exclude: [
          "src/**/*.d.ts",
          "src/**/*.{test,spec}.{ts,tsx}",
          "src/test/**",
          "src/main.tsx",
        ],
      },
    },
  }),
);
