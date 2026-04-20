/**
 * @mavrynt/auth-ui — public surface.
 *
 * Re-exports forms, service port, analytics port, and i18n resources.
 * Apps should import from here unless they want to keep bundle size
 * razor-thin, in which case they can import from sub-paths
 * (`@mavrynt/auth-ui/forms`, `@mavrynt/auth-ui/service`, etc).
 */
export * from "./forms/index.ts";
export * from "./service/index.ts";
export * from "./analytics/index.ts";
export * from "./i18n/index.ts";
