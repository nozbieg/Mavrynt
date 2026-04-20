import { createContext, useContext } from "react";
import { createConsoleAuthService } from "./consoleAuthService.ts";
import type { AuthService } from "./types.ts";

/**
 * React context for the auth service. Defaults to the console adapter so
 * components always have *some* working implementation (Liskov — the
 * default honours the same contract, just doesn't leave the browser).
 *
 * Consuming apps override this in their `Providers` by wrapping with
 * `<AuthServiceContext.Provider value={createHttpAuthService({...})}>`.
 */
export const AuthServiceContext = createContext<AuthService>(
  createConsoleAuthService(),
);

export const useAuthService = (): AuthService => useContext(AuthServiceContext);
