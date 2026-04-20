/**
 * AuthService port — login/register/logout abstraction the forms depend on.
 *
 * The form components and hooks never import a concrete adapter; backend
 * choice (Mavrynt API host, admin host, mock for tests) is decided in the
 * consuming app's `Providers` and swapped in via context. This keeps the
 * UI package routing-agnostic and backend-independent (Dependency
 * Inversion / mirrors the LeadService pattern in `mavrynt-landing`).
 */

export interface AuthUser {
  readonly id: string;
  readonly email: string;
  readonly name?: string;
  /** Optional roles claim; admins use this to gate routes. */
  readonly roles?: readonly string[];
}

export interface AuthSession {
  readonly token: string;
  readonly user: AuthUser;
  /** ISO 8601 instant; absent means the host treats the token as session-bound. */
  readonly expiresAt?: string;
}

export interface LoginCredentials {
  readonly email: string;
  readonly password: string;
  /** IETF BCP 47 locale of the form when submitted (for server-side audit). */
  readonly locale: string;
  /** Free-form source tag, e.g. "web:login" or "admin:login". */
  readonly source: string;
}

export interface RegisterCredentials {
  readonly name: string;
  readonly email: string;
  readonly password: string;
  readonly locale: string;
  readonly source: string;
}

/**
 * Structured error code so the UI can render precise copy without parsing
 * response bodies. Each code maps 1:1 to an i18n key under
 * `auth.<form>.error.<code>`.
 */
export type AuthErrorCode =
  | "network"
  | "validation"
  | "invalid_credentials"
  | "email_taken"
  | "rate_limited"
  | "server";

export class AuthError extends Error {
  public override readonly name = "AuthError";
  public readonly code: AuthErrorCode;

  public constructor(
    code: AuthErrorCode,
    message: string,
    options?: { readonly cause?: unknown },
  ) {
    super(message, options);
    this.code = code;
  }
}

export interface AuthService {
  /**
   * Authenticate an existing user. Resolves with a session on success;
   * throws `AuthError` on any failure.
   */
  readonly login: (credentials: LoginCredentials) => Promise<AuthSession>;
  /**
   * Create a new user. Resolves with a session on success; throws
   * `AuthError` on any failure (e.g. `email_taken`).
   */
  readonly register: (credentials: RegisterCredentials) => Promise<AuthSession>;
  /**
   * Tear down the active session (revoke server-side, clear local state).
   * Implementations should be idempotent.
   */
  readonly logout: () => Promise<void>;
}
