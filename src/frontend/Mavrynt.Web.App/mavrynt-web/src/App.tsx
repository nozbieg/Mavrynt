/**
 * DEPRECATED — Phase 2 superseded this file. Retained as an empty marker
 * only because the Cowork sandbox cannot delete files in-place.
 *
 * The real composition root lives under `src/app/` (App.tsx +
 * Providers.tsx + routes.tsx) and is loaded from `src/main.tsx`.
 *
 * To finish the cleanup from a developer shell, run from the repo root:
 *
 *   git rm src/frontend/mavrynt-web/src/App.tsx \
 *          src/frontend/mavrynt-web/src/App.css \
 *          src/frontend/mavrynt-admin/src/App.tsx \
 *          src/frontend/mavrynt-admin/src/App.css
 *
 * Nothing imports this file; `tsc --noEmit` and ESLint both treat it as
 * an empty module.
 */
export {};
