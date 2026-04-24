import type { i18n as I18nInstance } from "i18next";
import { RouterProvider, createBrowserRouter } from "react-router";
import { Providers } from "./Providers.tsx";
import { routes } from "./routes.tsx";

/**
 * App — pure composition. Receives the already-initialised `i18n`
 * instance from the `main.tsx` bootstrap and hands it to the providers.
 *
 * The router is created once at module load (not per render) so route
 * definitions aren't recreated on every re-render. Mirrors the
 * identical pattern in `mavrynt-landing`.
 *
 * `basename` is derived from Vite's `BASE_URL` (set to `base` in
 * vite.config.ts, e.g. "/app/"). React Router requires no trailing
 * slash, so we normalise it. This allows the router to match routes
 * like "login" when the browser URL is "/app/login".
 */
const basename = (import.meta.env.BASE_URL ?? "/").replace(/\/$/u, "") || "/";
const router = createBrowserRouter([...routes], { basename });

export interface AppProps {
  readonly i18n: I18nInstance;
}

const App = ({ i18n }: AppProps) => (
  <Providers i18n={i18n}>
    <RouterProvider router={router} />
  </Providers>
);

export default App;
