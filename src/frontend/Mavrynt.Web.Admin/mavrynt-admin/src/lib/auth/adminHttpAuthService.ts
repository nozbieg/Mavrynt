import {
  AuthError,
  type AuthService,
  type AuthSession,
  type LoginCredentials,
  type RegisterCredentials,
} from "@mavrynt/auth-ui";
import { clearAdminSession, saveAdminSession } from "./adminSession.ts";

type BackendAuthResponse = {
  accessToken: string;
  expiresAt?: string;
  requiresPasswordChange?: boolean;
  user: {
    id: string;
    email: string;
    displayName?: string;
    role?: string;
  };
};

const LOGIN_URL = "/admin-api/auth/login";

const normalizeRole = (role: string | undefined): string | undefined => {
  if (!role) return undefined;
  return role.trim().toLowerCase();
};

const mapStatus = (status: number): AuthError => {
  if (status === 401 || status === 403) {
    return new AuthError("invalid_credentials", "Invalid credentials.");
  }
  if (status === 422 || status === 400) {
    return new AuthError("validation", "Validation failed.");
  }
  if (status === 429) {
    return new AuthError("rate_limited", "Too many requests.");
  }
  return new AuthError("server", `Server responded with ${String(status)}.`);
};

const toAuthSession = (response: BackendAuthResponse): AuthSession => {
  const normalizedRole = normalizeRole(response.user.role);
  return {
    token: response.accessToken,
    ...(response.expiresAt !== undefined ? { expiresAt: response.expiresAt } : {}),
    ...(response.requiresPasswordChange !== undefined
      ? { requiresPasswordChange: response.requiresPasswordChange }
      : {}),
    user: {
      id: response.user.id,
      email: response.user.email,
      ...(response.user.displayName !== undefined ? { name: response.user.displayName } : {}),
      roles: normalizedRole ? [normalizedRole] : [],
    },
  };
};

export const createAdminHttpAuthService = (): AuthService => ({
  login: async (credentials: LoginCredentials): Promise<AuthSession> => {
    let response: Response;
    try {
      response = await fetch(LOGIN_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
        body: JSON.stringify({
          email: credentials.email,
          password: credentials.password,
        }),
        credentials: "include",
      });
    } catch (cause) {
      throw new AuthError("network", "Network request failed.", { cause });
    }

    if (!response.ok) throw mapStatus(response.status);

    const payload = (await response.json()) as BackendAuthResponse;
    const session = toAuthSession(payload);
    saveAdminSession(session);
    return session;
  },
  register: async (_credentials: RegisterCredentials): Promise<AuthSession> => {
    throw new AuthError("server", "Admin registration is not available.");
  },
  logout: async (): Promise<void> => {
    clearAdminSession();
  },
});
