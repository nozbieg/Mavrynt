import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";

/**
 * Vite config for `mavrynt-landing` (marketing SPA).
 *
 * The landing app is intentionally **independent of the backend** (ADR-010),
 * so no `/api` proxy is configured. Should the contact form ever post to
 * the Mavrynt API directly during development, add a proxy block here —
 * but prefer keeping the LeadService adapter abstraction.
 */
export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  optimizeDeps: {
    // Workspace packages are source-shipped; don't pre-bundle them so
    // changes are picked up immediately by HMR.
    exclude: ["@mavrynt/ui", "@mavrynt/config", "@mavrynt/design-tokens"],
  },
  server: {
    port: 5173,
  },
  preview: {
    port: 5173,
  },
});
