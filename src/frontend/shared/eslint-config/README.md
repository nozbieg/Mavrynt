# @mavrynt/eslint-config

Shared ESLint flat config for Mavrynt frontend code.

## Variants

| Entry point | Use for |
| --- | --- |
| `@mavrynt/eslint-config/react` (default) | React + Vite SPAs and the `@mavrynt/ui` package. Includes React Hooks, React Refresh, browser globals, and type-aware TypeScript rules. |
| `@mavrynt/eslint-config/library` | Non-React packages (`@mavrynt/config`, `@mavrynt/design-tokens`). Drops React/HMR plugins. |

## Usage in an app's `eslint.config.js`

```js
import mavryntReact from "@mavrynt/eslint-config/react";
import { defineConfig } from "eslint/config";

export default defineConfig([
  ...mavryntReact,
  // app-local overrides go here
]);
```

## Why type-aware rules

The `react` preset enables `tseslint.configs.recommendedTypeChecked` so we catch issues like:
- floating promises,
- mismatched async event handlers,
- incorrect template literal types.

These have repeatedly surfaced real bugs in similar codebases. The cost is a slightly slower lint — acceptable for the value.
