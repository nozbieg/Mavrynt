# mavrynt-landing

Public marketing site for **Mavrynt**. Built as a standalone React 19 + Vite
SPA that is intentionally **decoupled from the Mavrynt backend** (ADR-010),
so it can ship to any static host (Cloudflare Pages, S3 + CloudFront, Vercel)
without a runtime dependency on the API.

---

## Stack

| Concern         | Choice                                                |
| --------------- | ----------------------------------------------------- |
| Framework       | React 19.2 + Vite 8 (TypeScript, strict mode)         |
| Routing         | `react-router` v7 (`createBrowserRouter`, lazy pages) |
| i18n            | `react-i18next` v16 (PL default, EN secondary)        |
| SEO / head tags | `react-helmet-async` v3                               |
| Styling         | Tailwind CSS v4 + `@mavrynt/design-tokens`            |
| Shared UI       | `@mavrynt/ui` (workspace package)                     |
| Testing         | Vitest 3 + `@testing-library/react` + jsdom           |

All cross-cutting contracts (analytics, lead submission, feature flags)
are wired through **ports & adapters** — see `src/lib/` and `src/app/Providers.tsx`.

---

## Scripts

```bash
pnpm --filter mavrynt-landing dev          # Vite dev server (HMR)
pnpm --filter mavrynt-landing build        # tsc -b && vite build (+ .gz/.br)
pnpm --filter mavrynt-landing preview      # serve ./dist locally
pnpm --filter mavrynt-landing lint         # ESLint flat config
pnpm --filter mavrynt-landing typecheck    # tsc -b --noEmit
pnpm --filter mavrynt-landing test         # vitest run
pnpm --filter mavrynt-landing test:watch   # vitest (watch mode)
pnpm --filter mavrynt-landing test:cov     # vitest run --coverage
```

---

## Performance strategy

The landing site is the project's public front door, so we treat performance
as a first-class feature, not a cleanup task.

### Build-time

1. **Route-level code-splitting.** Every page under `src/app/routes.tsx`
   is loaded via `React.lazy`; the Home page's payload is all a first-time
   visitor pays for on initial TTI.
2. **Manual vendor chunks.** `react` + `react-router` share one long-lived
   chunk; `i18next` + detector share another. Content-only deploys do not
   bust either cache.
3. **Pre-compressed assets.** `vite-plugin-compression` emits `.gz` and
   `.br` siblings at build time (threshold 1 KiB). Any CDN can serve them
   without runtime CPU cost.
4. **Modern target.** `es2022` — no legacy polyfills for browsers that
   don't support dynamic import.
5. **CSS split + Tailwind v4 tree-shake.** Per-route stylesheets stay
   small; unused design tokens are dropped by Tailwind v4's runtime scan.

### Runtime budgets

Hard targets for production builds (measured on 4× CPU / Fast 3G in
Lighthouse Mobile):

| Metric                          | Budget     |
| ------------------------------- | ---------- |
| Largest Contentful Paint (LCP)  | ≤ 2.5 s    |
| Interaction to Next Paint (INP) | ≤ 200 ms   |
| Cumulative Layout Shift (CLS)   | ≤ 0.1      |
| Total JS (entry + Home)         | ≤ 150 KiB gzipped |
| Single chunk (warn at)          | 200 KiB raw       |

The Vite config fails the build if any chunk exceeds
`chunkSizeWarningLimit` (200 KiB raw), which catches accidental eager
imports of heavy libraries.

### Image guidelines

The marketing site is image-light by design, but when assets are added,
follow this checklist to preserve the budgets above:

- Ship **AVIF first**, **WebP fallback**, JPEG only as last resort. Use
  `<picture>` with `<source type="image/avif">` + `<source type="image/webp">`.
- **Always declare `width` and `height`** (or CSS aspect-ratio) on `<img>`
  tags — this prevents CLS (Core Web Vital).
- **Below-the-fold:** `loading="lazy"` + `decoding="async"`.
- **Above-the-fold hero:** `fetchpriority="high"` + eager loading; never
  lazy-load the hero image.
- Serve at 2×, 3× via `srcset` for retina displays; do not over-size.
- Run assets through `sharp` / `squoosh` in CI before committing.

### Fonts

The site uses the system-ui stack from `@mavrynt/design-tokens`
(`Inter` → `Segoe UI` → `system-ui`). No custom web fonts are loaded,
so there is no `<link rel="preload">` dance and no FOIT/FOUT risk.
If a custom font is ever added, preload it and use
`font-display: swap` to protect LCP.

---

## Accessibility

The site is built to WCAG 2.1 AA. A sweep of the primary concerns:

- **Skip-link** at the top of every layout routes focus to `#main`.
- **Landmarks:** `<header>` / `<nav>` (labelled) / `<main>` / `<footer>`
  (labelled) — every page has exactly one `<h1>`.
- **Focus visibility:** global `:focus-visible` ring from design tokens,
  never stripped.
- **Keyboard:** all interactive elements are keyboard-reachable; the FAQ
  uses native `<details>`/`<summary>` for zero-JS keyboard semantics.
- **Motion:** a global `prefers-reduced-motion: reduce` rule collapses
  animations and transitions to near-zero.
- **Forms:** every input has an `<label htmlFor>` pairing; invalid fields
  carry `aria-invalid` + `aria-describedby` → error id; the submit-error
  banner is announced via `role="alert"`; the success banner uses
  `role="status"`.
- **Language:** `<html lang>` is kept in sync with the active locale on
  every change so screen readers pick the right pronunciation dictionary.

Run accessibility checks locally via the browser's Lighthouse panel or
axe DevTools — there is no automated axe run in CI yet (tracked for a
future phase).

---

## Testing

- **Unit + integration:** Vitest + `@testing-library/react` + jsdom.
  Test harness is at `src/test/harness.tsx` and loads the real production
  locale JSON so assertions stay in lockstep with shipped copy.
- **Ports mocked, DOM real:** `AnalyticsClient` and `LeadService` are
  swapped with `vi.fn()`-backed fakes; we never mock React, Router, or
  i18next themselves.
- **Coverage:** `pnpm --filter mavrynt-landing test:cov` emits v8 HTML
  reports under `coverage/`.

E2E smoke tests (Playwright) land in the next phase under `tests/e2e/`.

---

## Folders

```
src/
  app/           # Router, shell (App.tsx), providers composition (Providers.tsx)
  components/    # Local presentational atoms (Icon, SectionHeader, …)
  content/       # Static site content (faq ids, footer columns, site config)
  features/      # Feature slices (contact-form, faq, hero, pricing, …)
  layouts/       # MarketingLayout + header/footer/nav/language switcher
  lib/           # Ports + adapters (analytics, lead, i18n, router, seo)
  pages/         # Route-level compositions
  test/          # Vitest setup + harness (not shipped)
```

---

## Relationship to the rest of the monorepo

- Consumes `@mavrynt/design-tokens`, `@mavrynt/ui`, `@mavrynt/config`,
  `@mavrynt/eslint-config`, `@mavrynt/tsconfig-base` from
  `src/frontend/shared/*`.
- Does **not** consume anything from `src/backend/*` — any integration
  with Mavrynt's API happens through the `LeadService` adapter.
- Deploys independently of the backend AppHost.
