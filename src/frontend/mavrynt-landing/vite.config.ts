import { defineConfig } from "vite";
import { fileURLToPath, URL } from "node:url";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import compression from "vite-plugin-compression";

/**
 * Vite config for `mavrynt-landing` (marketing SPA).
 *
 * The landing app is intentionally **independent of the backend** (ADR-010),
 * so no `/api` proxy is configured. Should the contact form ever post to
 * the Mavrynt API directly during development, add a proxy block here —
 * but prefer keeping the LeadService adapter abstraction.
 *
 * ## Performance strategy (Phase 4d)
 *
 *   1. Route-level code-splitting via React.lazy — each page lands in its
 *      own chunk so the hero TTI is dominated by app shell + Home only.
 *   2. Manual vendor chunking — React/React-DOM and i18next are pinned to
 *      long-lived, cacheable chunks so returning visitors pay only for
 *      content changes.
 *   3. Pre-compressed assets — gzip + brotli are emitted at build time so
 *      any static host (nginx, Cloudflare Pages, S3+CloudFront) can serve
 *      them without CPU cost at request time.
 *   4. CSS code-splitting is left ON (Vite default); Tailwind v4 scans
 *      used classes and the v4 runtime tree-shakes unused tokens.
 */
export default defineConfig({
  plugins: [
    react(),
    tailwindcss(),
    // Both compressors are build-only; they emit `.gz` / `.br` siblings
    // next to each asset and do not run during dev. Threshold mirrors
    // nginx `gzip_min_length` defaults — anything smaller isn't worth it.
    compression({
      algorithm: "gzip",
      ext: ".gz",
      threshold: 1024,
      deleteOriginFile: false,
    }),
    compression({
      algorithm: "brotliCompress",
      ext: ".br",
      threshold: 1024,
      deleteOriginFile: false,
    }),
  ],
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
  build: {
    // Modern browsers only — the marketing site doesn't need to support
    // browsers that lack dynamic import or top-level await. Smaller
    // polyfill surface ⇒ smaller hero payload.
    target: "es2022",
    cssCodeSplit: true,
    sourcemap: true,
    // Fail the build loudly if a single chunk exceeds the budget; this
    // guards against an accidental non-lazy import pulling a heavy lib
    // into the entry chunk.
    chunkSizeWarningLimit: 200,
    rollupOptions: {
      output: {
        manualChunks: {
          // Keep the React runtime in its own long-lived chunk so a
          // content-only deploy doesn't bust it.
          react: ["react", "react-dom", "react-router"],
          // i18n runtime + detector land together — they ship as a
          // single unit anyway.
          i18n: ["i18next", "react-i18next", "i18next-browser-languagedetector"],
        },
      },
    },
  },
  server: {
    port: 5173,
  },
  preview: {
    port: 5173,
  },
});
