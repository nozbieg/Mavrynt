import type { ReactNode } from "react";
import { Navigate } from "react-router";
import { useAdminAuth } from "./useAdminAuth";
export const RequireAdminAuth = ({ children }: { children: ReactNode }) => {
  const { session } = useAdminAuth();
  if (!session) return <Navigate to="/login" replace />;
  if (session.requiresPasswordChange) return <Navigate to="/change-password" replace />;
  return <>{children}</>;
};
