# Features (landing SPA)

Feature folders live here — each encapsulates a marketing section with
its own components, hooks, and (where needed) small state machines.

Phase 2 intentionally leaves this directory empty; Phase 3 fills it with:

- `hero/` — animated hero variant(s)
- `logo-cloud/` — customer logos
- `feature-grid/` — capability cards
- `how-it-works/` — step-by-step illustration
- `pricing-matrix/` — pricing tiers + comparison
- `testimonials/` — customer quotes
- `faq/` — accordion with i18n-aware questions
- `contact-form/` — form + `LeadService` adapter wiring

Folder-per-feature keeps each marketing block independently deletable,
reviewable, and testable.
