import { createContext, useContext } from "react";
import { noopAuthAnalytics, type AuthAnalyticsPort } from "./types.ts";

export const AuthAnalyticsContext =
  createContext<AuthAnalyticsPort>(noopAuthAnalytics);

export const useAuthAnalytics = (): AuthAnalyticsPort =>
  useContext(AuthAnalyticsContext);
