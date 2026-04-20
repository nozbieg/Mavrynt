/**
 * Minimal analytics port the auth forms call into. Defining a tiny local
 * interface (instead of importing the landing's analytics) keeps
 * `@mavrynt/auth-ui` independent of any specific app's analytics stack.
 *
 * Apps that already have an analytics layer (e.g. landing's `useAnalytics`)
 * should write a one-line adapter and pass it via `AuthAnalyticsContext`.
 * Apps that don't care can ignore the context entirely — the default
 * adapter is a noop, so the forms still work.
 */
export type AuthAnalyticsEvent =
  | "auth_login_attempt"
  | "auth_login_success"
  | "auth_login_error"
  | "auth_register_attempt"
  | "auth_register_success"
  | "auth_register_error";

export type AuthAnalyticsProps = Readonly<
  Record<string, string | number | boolean | undefined>
>;

export interface AuthAnalyticsPort {
  readonly track: (
    event: AuthAnalyticsEvent,
    props?: AuthAnalyticsProps,
  ) => void;
}

export const noopAuthAnalytics: AuthAnalyticsPort = {
  track: (): void => {
    /* intentional noop — see types.ts header */
  },
};
