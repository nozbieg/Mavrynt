import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

/**
 * Vite config for `mavrynt-admin` (admin SPA).
 *
 * Targets `Mavrynt.AdminApp` (admin host) instead of `Mavrynt.Api` —
 * preserves the existing per-app proxy convention.
 */
const adminApiTarget =
  process.env.ADMINAPI_HTTPS ??
  process.env.ADMINAPI_HTTP ??
  "http://localhost:5001";

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  optimizeDeps: {
    exclude: ["@mavrynt/ui", "@mavrynt/config", "@mavrynt/design-tokens"],
  },
  server: {
    port: 5175,
    proxy: {
      "/api": {
        target: adminApiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
  preview: {
    port: 5175,
    proxy: {
      "/api": {
        target: adminApiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
