import type { ReactNode } from "react";
import { Navigate } from "react-router";
import { useAdminSession } from "./AdminSessionProvider.tsx";

export const RequireAdminAuth = ({ children }: { children: ReactNode }) => {
  const { session } = useAdminSession();

  if (!session) return <Navigate to="/login" replace />;
  if (session.requiresPasswordChange) {
    return <Navigate to="/change-password" replace />;
  }

  return <>{children}</>;
};
