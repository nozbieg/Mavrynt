# @mavrynt/tsconfig-base

Shared TypeScript configurations for every Mavrynt frontend package.

## Available configs

| File | Use for |
| --- | --- |
| `base.json` | Library packages with no DOM or React (e.g. `config`). |
| `react.json` | React + DOM applications and the `ui` package. |
| `node.json` | Files executed by Node (e.g. `vite.config.ts`). |

## Usage

In an app or package `tsconfig.app.json`:

```json
{
  "extends": "@mavrynt/tsconfig-base/react.json",
  "compilerOptions": {
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.app.tsbuildinfo",
    "paths": { "@/*": ["./src/*"] }
  },
  "include": ["src"]
}
```

In a Node-side `tsconfig.node.json`:

```json
{
  "extends": "@mavrynt/tsconfig-base/node.json",
  "compilerOptions": {
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.node.tsbuildinfo"
  },
  "include": ["vite.config.ts"]
}
```

## What is enforced

- Strict mode (`strict`, `noUncheckedIndexedAccess`, `exactOptionalPropertyTypes`, `noImplicitReturns`).
- Bundler-friendly module resolution (`moduleResolution: "bundler"`, `verbatimModuleSyntax`).
- Erasable-only syntax (`erasableSyntaxOnly`) so files compile cleanly without TS-specific runtime helpers.
- ES2023 target and `lib`.

Override only when you have a concrete reason — record the reason in the consumer's `tsconfig` as a comment.
