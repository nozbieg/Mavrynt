# @mavrynt/config

Shared runtime concerns for Mavrynt SPAs. Three independent modules:

| Module | Purpose |
| --- | --- |
| `env` | Schema-driven, type-safe env variable loader. |
| `feature-flags` | `FeatureFlagClient` interface (port) + static in-memory adapter. |
| `i18n` | `createI18n` factory wrapping `i18next` + `react-i18next` with PL/EN defaults. |

## env — typed env loader

```ts
import { defineEnvSchema, loadEnv } from "@mavrynt/config";

const schema = defineEnvSchema({
  VITE_API_BASE_URL: { kind: "url", required: true },
  VITE_FEATURE_FLAG_KEY: { kind: "string", required: false, default: "" },
  VITE_TELEMETRY_ENABLED: { kind: "boolean", required: false, default: false },
});

export const env = loadEnv(schema);
// env.VITE_API_BASE_URL is `string`
// env.VITE_TELEMETRY_ENABLED is `boolean`
```

Throws `EnvValidationError` if a required variable is missing or malformed.

## feature-flags — port + adapter

```ts
import {
  createStaticFeatureFlagClient,
  type FeatureFlagClient,
} from "@mavrynt/config";

export const flags: FeatureFlagClient = createStaticFeatureFlagClient({
  flags: { newPricing: true, betaSignup: false },
});

if (flags.isEnabled("newPricing")) {
  // …
}
```

The static adapter is for development; once the backend feature service ships (ADR-008), a remote adapter implementing `FeatureFlagClient` will replace it transparently.

## i18n — PL/EN bootstrap

```ts
import { createI18n } from "@mavrynt/config";
import en from "./locales/en.json";
import pl from "./locales/pl.json";

export const i18n = await createI18n({
  resources: {
    en: { common: en },
    pl: { common: pl },
  },
});
```

`i18next` and `react-i18next` are listed as **optional peer dependencies** here. Apps that need translations install them locally (the landing app does; the admin and web apps may opt in later).

## Why these three live together

They share one trait: they bootstrap the runtime environment of an SPA. Co-locating them keeps initialization logic in one place and prevents the apps from each rolling their own variants.
