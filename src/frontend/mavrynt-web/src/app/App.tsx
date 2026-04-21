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
 */
const router = createBrowserRouter([...routes]);

export interface AppProps {
  readonly i18n: I18nInstance;
}

const App = ({ i18n }: AppProps) => (
  <Providers i18n={i18n}>
    <RouterProvider router={router} />
  </Providers>
);

export default App;
