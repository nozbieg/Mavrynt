# @mavrynt/ui

Shared React UI for all Mavrynt SPAs (web, admin, landing). Built on top of `@mavrynt/design-tokens` and Tailwind v4.

## Public surface

```ts
import {
  // primitives
  Container, Section, Stack, Button, Link, Logo, buttonStyles, cn,
  // layout
  Navbar, Footer,
  // theme
  ThemeProvider, useTheme,
} from "@mavrynt/ui";
```

## Architectural rules

- **Routing-agnostic.** `Navbar` and `Footer` accept `links` / `actions` / `bottom` slots as `ReactNode`, so each app injects its own router-aware components without forking the layout.
- **Stateless first.** Only `ThemeProvider` owns state. Other components are pure visual primitives.
- **Tokens over magic numbers.** Every spacing / color value resolves to a design token. If you need a value that does not exist as a token, add it to `@mavrynt/design-tokens` first.
- **Open/Closed via `className`.** Every component accepts `className` and merges it last, so apps can extend visuals without forking.
- **Accessibility by default.** Focus rings, semantic landmarks, ARIA labels, reduced-motion-aware. Do not strip these in app overrides.

## Theme usage

```tsx
import { ThemeProvider, useTheme } from "@mavrynt/ui";

const App = () => (
  <ThemeProvider defaultMode="system">
    <MyPage />
  </ThemeProvider>
);

const ThemeToggle = () => {
  const { mode, resolved, setMode } = useTheme();
  return (
    <button onClick={() => setMode(resolved === "dark" ? "light" : "dark")}>
      {resolved === "dark" ? "🌙" : "☀️"} ({mode})
    </button>
  );
};
```

`<ThemeProvider>` writes `data-theme="light"` or `data-theme="dark"` on `<html>`, persists the user's choice to `localStorage`, and reacts to OS theme changes when the user is on `system`.

## Tailwind v4 — class scanning

For Tailwind to detect classes used inside this package, each consuming app must add a `@source` directive in its main CSS:

```css
@import "tailwindcss";
@import "@mavrynt/design-tokens/styles/reset.css";
@import "@mavrynt/design-tokens/styles/tokens.css";

@source "../../shared/ui/src/**/*.{ts,tsx}";
```

The relative path is identical for all three apps (they all live at `src/frontend/<app>/src/index.css`).
