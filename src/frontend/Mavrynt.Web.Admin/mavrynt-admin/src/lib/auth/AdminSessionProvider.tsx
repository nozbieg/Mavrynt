import { createContext, useContext, useState, type ReactNode } from "react";
import type { AuthSession } from "@mavrynt/auth-ui";
import { clearAdminSession, getAdminSession } from "./adminSession.ts";

type AdminSessionContextValue = {
  session: AuthSession | null;
  syncSession: () => void;
  logout: () => void;
};

const AdminSessionContext = createContext<AdminSessionContextValue>({
  session: null,
  syncSession: () => {},
  logout: () => {},
});

export const AdminSessionProvider = ({ children }: { children: ReactNode }) => {
  const [session, setSession] = useState<AuthSession | null>(getAdminSession);

  const syncSession = () => setSession(getAdminSession());

  const logout = () => {
    clearAdminSession();
    setSession(null);
  };

  return (
    <AdminSessionContext.Provider value={{ session, syncSession, logout }}>
      {children}
    </AdminSessionContext.Provider>
  );
};

export const useAdminSession = () => useContext(AdminSessionContext);
