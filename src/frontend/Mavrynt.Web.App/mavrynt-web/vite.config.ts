import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

/**
 * Vite config for `mavrynt-web` (user-facing SPA).
 *
 * Dev proxy:
 *   `/api/*` → `Mavrynt.Api` (includes `/api/auth/login`,
 *   `/api/auth/register`, `/api/auth/logout`). The auth endpoints are
 *   consumed by `@mavrynt/auth-ui`'s HTTP adapter when `VITE_AUTH=http`.
 *
 * Served via `Mavrynt.Web.App` (.NET SpaProxy wrapper).
 *   In development the .NET host proxies to this Vite dev server (port 5174).
 *   In production the .NET host serves the pre-built `dist/` output.
 *   This app is mounted at base path `/app/`.
 *
 * Port 5174 matches the default in `@mavrynt/config`'s `DEFAULT_APP_URLS.web`.
 * Fallback API target port 5046 matches `Mavrynt.Api` launchSettings (HTTP).
 * Override with env vars: API_HTTPS / API_HTTP (injected by Aspire in dev).
 */
const apiTarget =
  process.env.API_HTTPS ?? process.env.API_HTTP ?? "http://localhost:5046";

const apiProxy = {
  "/api": {
    target: apiTarget,
    changeOrigin: true,
    secure: false,
  },
} as const;

export default defineConfig({
  base: process.env.VITE_APP_BASE ?? "/app/",
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  optimizeDeps: {
    exclude: [
      "@mavrynt/ui",
      "@mavrynt/auth-ui",
      "@mavrynt/config",
      "@mavrynt/design-tokens",
    ],
  },
  server: {
    port: 5174,
    proxy: apiProxy,
  },
  preview: {
    port: 5174,
    proxy: apiProxy,
  },
});
