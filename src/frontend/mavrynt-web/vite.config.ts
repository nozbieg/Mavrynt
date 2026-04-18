import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

/**
 * Vite config for `mavrynt-web` (user-facing SPA).
 *
 * Keeps the existing `/api` proxy so the SPA can talk to `Mavrynt.Api`
 * during local development without CORS configuration.
 */
const apiTarget =
  process.env.API_HTTPS ?? process.env.API_HTTP ?? "http://localhost:5000";

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
    port: 5174,
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
  preview: {
    port: 5174,
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
