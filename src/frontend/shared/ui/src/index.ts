/**
 * @mavrynt/ui — public surface.
 *
 * Re-exports primitives, layout, and theme. Apps should import from here
 * unless they need to keep bundle size razor-thin, in which case they can
 * import from the sub-paths (`@mavrynt/ui/primitives`, etc.).
 */
export * from "./primitives/index.ts";
export * from "./layout/index.ts";
export * from "./theme/index.ts";
