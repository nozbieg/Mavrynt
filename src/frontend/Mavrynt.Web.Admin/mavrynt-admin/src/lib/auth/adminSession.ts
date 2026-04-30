import type { AuthSession } from "@mavrynt/auth-ui";

const SESSION_KEY = "mavrynt.admin.session";

const parseIsoDate = (value: string | undefined): number | null => {
  if (!value) return null;
  const timestamp = Date.parse(value);
  return Number.isNaN(timestamp) ? null : timestamp;
};

export const getAdminSession = (): AuthSession | null => {
  const raw = globalThis.localStorage.getItem(SESSION_KEY);
  if (!raw) return null;

  try {
    const session = JSON.parse(raw) as AuthSession;
    const expiresAt = parseIsoDate(session.expiresAt);
    if (expiresAt !== null && expiresAt <= Date.now()) {
      globalThis.localStorage.removeItem(SESSION_KEY);
      return null;
    }
    return session;
  } catch {
    globalThis.localStorage.removeItem(SESSION_KEY);
    return null;
  }
};

export const saveAdminSession = (session: AuthSession): void => {
  globalThis.localStorage.setItem(SESSION_KEY, JSON.stringify(session));
};

export const clearAdminSession = (): void => {
  globalThis.localStorage.removeItem(SESSION_KEY);
};

export const getAdminAccessToken = (): string | null => getAdminSession()?.token ?? null;

export const isAdminAuthenticated = (): boolean => getAdminSession() !== null;

export const hasAdminRole = (): boolean => {
  const roles = getAdminSession()?.user.roles ?? [];
  return roles.some((role) => role.toLowerCase() === "admin");
};
