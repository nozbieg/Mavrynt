import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";
import { useAuthService, type AuthSession } from "@mavrynt/auth-ui";
import { clearAdminSession, getAdminSession } from "./adminSession.ts";

type AdminAuthContextValue = {
  session: AuthSession | null;
  user: AuthSession["user"] | null;
  isAuthenticated: boolean;
  accessToken: string | null;
  refreshSession: () => void;
  setSession: (session: AuthSession | null) => void;
  logout: () => Promise<void>;
};

const AdminAuthContext = createContext<AdminAuthContextValue | null>(null);

export const AdminAuthProvider = ({ children }: { children: ReactNode }) => {
  const authService = useAuthService();
  const [session, setSessionState] = useState<AuthSession | null>(() => getAdminSession());

  const refreshSession = useCallback(() => {
    setSessionState(getAdminSession());
  }, []);

  const setSession = useCallback((nextSession: AuthSession | null) => {
    setSessionState(nextSession);
  }, []);

  const logout = useCallback(async () => {
    try {
      await authService.logout();
    } finally {
      clearAdminSession();
      setSessionState(null);
      globalThis.location.assign("/login");
    }
  }, [authService]);

  const value = useMemo<AdminAuthContextValue>(
    () => ({
      session,
      user: session?.user ?? null,
      isAuthenticated: Boolean(session),
      accessToken: session?.token ?? null,
      refreshSession,
      setSession,
      logout,
    }),
    [logout, refreshSession, session, setSession],
  );

  return <AdminAuthContext.Provider value={value}>{children}</AdminAuthContext.Provider>;
};

export const useAdminAuthContext = () => {
  const context = useContext(AdminAuthContext);
  if (!context) throw new Error("useAdminAuth must be used within AdminAuthProvider");
  return context;
};
