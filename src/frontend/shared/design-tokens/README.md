# @mavrynt/design-tokens

Single source of truth for visual design tokens shared by all Mavrynt SPAs.

## Two consumption modes

### CSS (recommended for styling)

```css
/* in your app's main stylesheet, e.g. src/styles/index.css */
@import "tailwindcss";
@import "@mavrynt/design-tokens/styles/reset.css";
@import "@mavrynt/design-tokens/styles/tokens.css";
```

This unlocks:
- All static brand / neutral / status palettes as Tailwind v4 utilities (`bg-brand-500`, `text-neutral-700`, …).
- Semantic theme utilities that follow light / dark mode (`bg-bg`, `text-fg`, `border-border`, `text-fg-muted`, `bg-primary`, `text-primary-fg`).
- Custom radius, shadow, spacing and font tokens (`rounded-md`, `shadow-md`, `font-display`, `p-section`).

### TypeScript (for non-CSS consumers)

```ts
import { tokens, colors, spacing } from "@mavrynt/design-tokens";

const chartPalette = [colors.brand[500], colors.brand[300], colors.success[500]];
```

Use this for charts, canvas/WebGL, animations, or tests — anywhere CSS is not the right tool.

## Theming

Themes are switched by setting `data-theme="light"` or `data-theme="dark"` on the `<html>` element. Without `data-theme`, the user's OS preference is used. The `@mavrynt/ui` `ThemeProvider` handles this for you.

## Adding a new token

1. Add the value in `src/styles/tokens.css` (inside `@theme` for static tokens, or in the semantic `:root`/`[data-theme]` blocks for theme-dependent tokens).
2. Mirror it in `src/tokens.ts` so JS consumers stay in sync.
3. If it should also be exposed as a Tailwind utility for theme switching, list it inside the `@theme inline` block.

## Architectural rules

- This package contains **no logic and no React code** — pure values + CSS.
- The semantic / theme split is intentional: static palettes belong in `@theme`, runtime theme variables belong in `:root` / `[data-theme]` and are bridged to Tailwind via `@theme inline`.
