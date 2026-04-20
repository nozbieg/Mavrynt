import {
  AuthError,
  type AuthService,
  type AuthSession,
  type LoginCredentials,
  type RegisterCredentials,
} from "./types.ts";

/**
 * Default development adapter — logs the payload, returns a mock session.
 * Keeps the forms functional end-to-end without any backend commitment
 * while the Users module endpoints are still being designed.
 *
 * Two deterministic test triggers (so demos and tests can exercise the
 * error paths without a real backend):
 *   - email starting with `fail+invalid@`   → `invalid_credentials`
 *   - email starting with `fail+taken@`     → `email_taken` (register)
 *
 * Never ship this to production; consuming apps swap in `httpAuthService`
 * via context when `VITE_AUTH=http` (or equivalent flag) is set.
 */

const DEFAULT_LATENCY_MS = 300;

const wait = (ms: number): Promise<void> =>
  new Promise<void>((resolve) => {
    setTimeout(resolve, ms);
  });

interface ConsoleAuthOptions {
  /** Override simulated network latency (default 300 ms). */
  readonly latencyMs?: number;
  /**
   * Roles to embed in the mock user. Admin app passes `["admin"]` so its
   * route guards behave realistically against the mock.
   */
  readonly roles?: readonly string[];
}

const buildSession = (
  email: string,
  name: string,
  roles: readonly string[],
): AuthSession => ({
  token: `mock-${cryptoRandomId()}`,
  user: {
    id: `user-${cryptoRandomId()}`,
    email,
    name,
    roles,
  },
});

const cryptoRandomId = (): string => {
  // `crypto.randomUUID` is widely available in modern browsers + Node ≥ 19.
  // Fall back to Math.random for ancient runtimes (tests on jsdom may need it).
  if (
    typeof globalThis.crypto !== "undefined" &&
    typeof globalThis.crypto.randomUUID === "function"
  ) {
    return globalThis.crypto.randomUUID();
  }
  return Math.random().toString(36).slice(2, 12);
};

export const createConsoleAuthService = (
  options: ConsoleAuthOptions = {},
): AuthService => {
  const latencyMs = options.latencyMs ?? DEFAULT_LATENCY_MS;
  const roles = options.roles ?? [];

  return {
    login: async (credentials: LoginCredentials): Promise<AuthSession> => {
      // `console.warn` is allowed by the shared ESLint config; this adapter
      // is a development/test fallback, not a production auth path.
      console.warn("[AuthService:console] would login", {
        email: credentials.email,
        source: credentials.source,
        locale: credentials.locale,
      });
      await wait(latencyMs);
      if (credentials.email.startsWith("fail+invalid@")) {
        throw new AuthError("invalid_credentials", "Invalid credentials.");
      }
      return buildSession(credentials.email, credentials.email, roles);
    },

    register: async (credentials: RegisterCredentials): Promise<AuthSession> => {
      console.warn("[AuthService:console] would register", {
        email: credentials.email,
        name: credentials.name,
        source: credentials.source,
        locale: credentials.locale,
      });
      await wait(latencyMs);
      if (credentials.email.startsWith("fail+taken@")) {
        throw new AuthError("email_taken", "Email already registered.");
      }
      return buildSession(credentials.email, credentials.name, roles);
    },

    logout: async (): Promise<void> => {
      console.warn("[AuthService:console] would logout");
      await wait(latencyMs);
    },
  };
};
