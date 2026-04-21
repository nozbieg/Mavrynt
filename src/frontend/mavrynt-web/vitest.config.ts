import { defineConfig, mergeConfig } from "vitest/config";
import viteConfig from "./vite.config.ts";

/**
 * Vitest configuration for `mavrynt-web`.
 *
 * Mirrors `mavrynt-landing/vitest.config.ts` — the runtime Vite config
 * is reused via `mergeConfig` so tests resolve modules identically to
 * the SPA. DRY: one module-resolution truth per app.
 *
 * `tests/e2e/**` is explicitly excluded so Playwright specs never leak
 * into the unit run. Coverage stays narrow (src/**) and skips the
 * bootstrap + type-only files.
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
