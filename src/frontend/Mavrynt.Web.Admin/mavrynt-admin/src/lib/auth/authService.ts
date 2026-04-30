import { createConsoleAuthService, type AuthService } from "@mavrynt/auth-ui";
import { createAdminHttpAuthService } from "./adminHttpAuthService.ts";

const ADMIN_ROLES: readonly string[] = ["admin"];

const useConsoleAuth =
  import.meta.env.VITE_AUTH === "console" || import.meta.env.VITE_AUTH === "mock";

export const authService: AuthService = useConsoleAuth
  ? createConsoleAuthService({ roles: ADMIN_ROLES })
  : createAdminHttpAuthService();
