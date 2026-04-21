import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

/**
 * Vite config for `mavrynt-admin` (admin SPA).
 *
 * Dev proxy:
 *   `/admin-api/*` → `Mavrynt.AdminApp` (admin host, separate from `Mavrynt.Api`).
 *   Includes `/admin-api/auth/login`, `/admin-api/auth/register`, `/admin-api/auth/logout`;
 *   the auth endpoints are consumed by `@mavrynt/auth-ui`'s HTTP adapter
 *   when `VITE_AUTH=http`.
 *
 * When running through YARP proxy:
 *   API calls are routed through `/admin-api/*` which YARP proxies to Mavrynt.AdminApp.
 *   This app is served at `/admin/*` through the YARP reverse proxy.
 *
 * Port 5175 matches the default in `@mavrynt/config`'s `DEFAULT_APP_URLS.admin`.
 */
const adminApiTarget =
  process.env.ADMINAPI_HTTPS ??
  process.env.ADMINAPI_HTTP ??
  "http://localhost:5001";

const apiProxy = {
  "/admin-api": {
    target: adminApiTarget,
    changeOrigin: true,
    secure: false,
    pathRewrite: {
      "^/admin-api": "/api",
    },
  },
} as const;

export default defineConfig({
  base: process.env.VITE_ADMIN_BASE ?? "/admin/",
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
    port: 5175,
    proxy: apiProxy,
  },
  preview: {
    port: 5175,
    proxy: apiProxy,
  },
});
