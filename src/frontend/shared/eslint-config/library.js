// @ts-check
/**
 * Lint config for non-React shared packages (e.g. @mavrynt/config,
 * @mavrynt/design-tokens). Drops React/HMR plugins and DOM globals.
 */
import js from "@eslint/js";
import globals from "globals";
import tseslint from "typescript-eslint";

const config = [
  {
    ignores: ["dist", "build", "coverage", "node_modules"],
  },
  {
    files: ["**/*.{ts,tsx}"],
    languageOptions: {
      ecmaVersion: 2023,
      sourceType: "module",
      globals: {
        ...globals.es2023,
      },
    },
    rules: {
      ...js.configs.recommended.rules,
      "no-console": ["warn", { allow: ["warn", "error"] }],
      "no-debugger": "error",
      eqeqeq: ["error", "always", { null: "ignore" }],
    },
  },
  ...tseslint.configs.recommended.map((cfg) => ({
    ...cfg,
    files: ["**/*.{ts,tsx}"],
  })),
  {
    files: ["**/*.{ts,tsx}"],
    rules: {
      "@typescript-eslint/consistent-type-imports": [
        "error",
        { prefer: "type-imports", fixStyle: "inline-type-imports" },
      ],
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          argsIgnorePattern: "^_",
          varsIgnorePattern: "^_",
          caughtErrorsIgnorePattern: "^_",
        },
      ],
    },
  },
];

export default config;
