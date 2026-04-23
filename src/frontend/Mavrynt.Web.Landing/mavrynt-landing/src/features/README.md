# Features (landing SPA)

Each folder is a self-contained marketing section composed by the
`pages/` layer. Features depend on:

- `@mavrynt/ui` primitives (Section, Stack, Button helpers)
- `src/components/` shared helpers (Icon, SectionHeader)
- `src/content/` typed descriptors (structure only, copy lives in i18n)
- `src/lib/` ports (analytics, lead service, seo, i18n, router)

No feature imports another feature — cross-feature reuse flows through
`src/components/` or `src/lib/`, which keeps Phase 4+ splits safe.

## Present

- `hero/` — top-of-page pitch + CTA pair
- `logo-cloud/` — social-proof strip
- `feature-grid/` — capability grid (`compact` + full variants)
- `how-it-works/` — three-step walkthrough
- `pricing-matrix/` — three-tier pricing cards
- `testimonials/` — customer-quote grid
- `faq/` — native `<details>` accordion
- `cta-banner/` — shared closing CTA
- `contact-form/` — form + `useContactForm` hook wired to `LeadService`

## Intentionally out of scope (Phase 4+)

- Motion (Framer Motion / view transitions)
- Carousel for testimonials
- A/B-testing hooks on CTAs
- Real partner logos (SVGs in `/public/logos/`)
