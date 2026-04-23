import {
  createConsoleAuthService,
  createHttpAuthService,
  type AuthService,
} from "@mavrynt/auth-ui";

/**
 * Resolve the AuthService adapter for `mavrynt-web`.
 *
 * Default = console adapter (mock, see `@mavrynt/auth-ui` README), which
 * keeps the SPA usable end-to-end while the Mavrynt Users module
 * endpoints are still being designed.
 *
 * Set `VITE_AUTH=http` to switch to the real HTTP adapter — talks to
 * `/api/auth/*` (proxied to `Mavrynt.Api` in dev via `vite.config.ts`).
 *
 * Endpoints are resolved at module init: re-import the module to pick
 * up changed env vars (HMR handles this in dev).
 */
const useHttp = import.meta.env.VITE_AUTH === "http";

export const authService: AuthService = useHttp
  ? createHttpAuthService({
      endpoints: {
        login: "/api/auth/login",
        register: "/api/auth/register",
        logout: "/api/auth/logout",
      },
    })
  : createConsoleAuthService();
