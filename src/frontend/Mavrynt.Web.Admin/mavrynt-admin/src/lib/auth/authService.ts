import {
  createConsoleAuthService,
  createHttpAuthService,
  type AuthService,
} from "@mavrynt/auth-ui";

/**
 * Admin AuthService adapter.
 *
 * Default = console adapter (mock, see `@mavrynt/auth-ui` README), with
 * `roles: ["admin"]` stamped on the mock user so downstream role-gated
 * UI can be prototyped end-to-end without a backend. The Mavrynt Users
 * module endpoints are still being designed (Phase 1 decision), so the
 * mock lets the admin SPA boot and behave correctly in the meantime.
 *
 * Set `VITE_AUTH=http` to switch to the real HTTP adapter — talks to
 * `/api/auth/*` proxied to `Mavrynt.AdminApp` in dev via `vite.config.ts`.
 * Endpoints are resolved at module init: re-import the module to pick
 * up changed env vars (HMR handles this in dev).
 */
const ADMIN_ROLES: readonly string[] = ["admin"];

const useHttp = import.meta.env.VITE_AUTH === "http";

// The Vite dev proxy rewrites /admin-api/* → /api/* on Mavrynt.AdminApp.
export const authService: AuthService = useHttp
  ? createHttpAuthService({
      endpoints: {
        login: "/admin-api/auth/login",
        register: "/admin-api/auth/register",
        logout: "/admin-api/auth/logout",
      },
    })
  : createConsoleAuthService({ roles: ADMIN_ROLES });
