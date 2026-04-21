import {
  createStaticFeatureFlagClient,
  type FeatureFlagClient,
} from "@mavrynt/config/feature-flags";

/**
 * Admin feature flags.
 *
 * Backed by the static in-memory adapter from `@mavrynt/config` (see
 * ADR-008). Swapping for a remote adapter later only changes this file
 * — call sites (`client.isEnabled("admin.register.enabled")`) stay the
 * same (Open/Closed).
 *
 * **`admin.register.enabled`** is `false` by default — Phase 1 decision:
 * self-registration on the admin SPA is invite-only. An operator can
 * flip it via `VITE_ADMIN_REGISTER_ENABLED=true` (dev/staging) without
 * touching code; production will sit behind the real flag service.
 */
const envFlag = (
  value: string | undefined,
  fallback: boolean,
): boolean => {
  if (value === undefined || value === "") return fallback;
  return value === "true" || value === "1";
};

const adminRegisterEnabled = envFlag(
  import.meta.env.VITE_ADMIN_REGISTER_ENABLED as string | undefined,
  false,
);

export const featureFlags: FeatureFlagClient = createStaticFeatureFlagClient({
  flags: {
    "admin.register.enabled": adminRegisterEnabled,
  },
});

/** Convenience re-export for components that only need this one flag. */
export const ADMIN_REGISTER_ENABLED_FLAG = "admin.register.enabled" as const;
