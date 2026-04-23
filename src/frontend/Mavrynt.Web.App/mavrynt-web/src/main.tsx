import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import type { i18n as I18nInstance } from "i18next";
import App from "./app/App.tsx";
import { i18nPromise } from "./lib/i18n/i18n.ts";
import "./index.css";

/**
 * Entry point for `mavrynt-web`.
 *
 * We **await** i18n before mounting so the first render already has
 * translations — avoids the "key flash" where raw i18n keys appear on
 * first paint. Mirrors the landing SPA bootstrap.
 *
 * The async wrapper keeps the file compatible with bundler targets that
 * don't support top-level await.
 */
const bootstrap = async (): Promise<void> => {
  const i18n = (await i18nPromise) as I18nInstance;

  const host = document.getElementById("root");
  if (host === null) {
    throw new Error("Mount node '#root' is missing from index.html");
  }

  createRoot(host).render(
    <StrictMode>
      <App i18n={i18n} />
    </StrictMode>,
  );
};

void bootstrap();
