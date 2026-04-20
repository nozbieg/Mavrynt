import {
  AuthError,
  type AuthService,
  type AuthSession,
  type LoginCredentials,
  type RegisterCredentials,
} from "./types.ts";

/**
 * HTTP adapter — POSTs credentials as JSON to the configured endpoints.
 * Endpoints are injected so the same adapter works against:
 *  - the Mavrynt API (web SPA, default `/api/auth/*` via Vite proxy),
 *  - the Mavrynt AdminApp (admin SPA, separate origin),
 *  - a local mock server in tests.
 *
 * Maps HTTP status codes onto `AuthError.code` so the form renders
 * precise error copy without parsing response bodies.
 */
export interface HttpAuthEndpoints {
  readonly login: string;
  readonly register: string;
  readonly logout: string;
}

export interface HttpAuthServiceOptions {
  readonly endpoints: HttpAuthEndpoints;
  /** Extra headers (e.g. CSRF token, API key). */
  readonly headers?: Readonly<Record<string, string>>;
  /** Abort after this many ms. Default 10s. */
  readonly timeoutMs?: number;
}

const DEFAULT_TIMEOUT = 10_000;

const mapStatus = (status: number): AuthError => {
  if (status === 401 || status === 403) {
    return new AuthError("invalid_credentials", "Invalid credentials.");
  }
  if (status === 409) {
    return new AuthError("email_taken", "Email already registered.");
  }
  if (status === 422 || status === 400) {
    return new AuthError("validation", "Validation failed.");
  }
  if (status === 429) {
    return new AuthError("rate_limited", "Too many requests.");
  }
  return new AuthError(
    "server",
    `Server responded with ${String(status)}.`,
  );
};

const postJson = async <TResponse>(
  url: string,
  body: unknown,
  headers: Readonly<Record<string, string>> | undefined,
  timeoutMs: number,
): Promise<TResponse> => {
  const controller = new AbortController();
  const timer = setTimeout(() => {
    controller.abort();
  }, timeoutMs);

  let response: Response;
  try {
    response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Accept: "application/json",
        ...headers,
      },
      body: JSON.stringify(body),
      signal: controller.signal,
      credentials: "include",
    });
  } catch (cause) {
    throw new AuthError("network", "Network request failed.", { cause });
  } finally {
    clearTimeout(timer);
  }

  if (!response.ok) {
    throw mapStatus(response.status);
  }

  // The contract assumes the server returns JSON for success bodies.
  // Bodies are intentionally not validated here — that's the caller's job.
  return (await response.json()) as TResponse;
};

export const createHttpAuthService = (
  options: HttpAuthServiceOptions,
): AuthService => {
  const timeoutMs = options.timeoutMs ?? DEFAULT_TIMEOUT;

  return {
    login: async (credentials: LoginCredentials): Promise<AuthSession> =>
      postJson<AuthSession>(
        options.endpoints.login,
        credentials,
        options.headers,
        timeoutMs,
      ),

    register: async (credentials: RegisterCredentials): Promise<AuthSession> =>
      postJson<AuthSession>(
        options.endpoints.register,
        credentials,
        options.headers,
        timeoutMs,
      ),

    logout: async (): Promise<void> => {
      const controller = new AbortController();
      const timer = setTimeout(() => {
        controller.abort();
      }, timeoutMs);
      try {
        const response = await fetch(options.endpoints.logout, {
          method: "POST",
          headers: { Accept: "application/json", ...options.headers },
          signal: controller.signal,
          credentials: "include",
        });
        if (!response.ok && response.status !== 401) {
          throw mapStatus(response.status);
        }
      } catch (cause) {
        if (cause instanceof AuthError) throw cause;
        throw new AuthError("network", "Network request failed.", { cause });
      } finally {
        clearTimeout(timer);
      }
    },
  };
};
