/**
 * Feature-flag client interface (port) and a static in-memory adapter.
 *
 * Design:
 * - The interface is the contract apps depend on (Dependency Inversion).
 * - The static adapter is a deterministic default so apps work without a
 *   backend during development (KISS). When the backend gains a flag
 *   service (ADR-008), a remote adapter implementing the same interface
 *   will replace the static one with no call-site changes (Open/Closed).
 */

export type FeatureFlagValue = boolean | string | number;

export interface FeatureFlagContext {
  readonly userId?: string | undefined;
  readonly tenantId?: string | undefined;
  readonly environment?: string | undefined;
  readonly attributes?: Readonly<Record<string, unknown>>;
}

export interface FeatureFlagClient {
  /** Boolean flag. Defaults to `false` if the flag is undefined. */
  isEnabled(flag: string, context?: FeatureFlagContext): boolean;
  /** Variant flag. Returns `defaultValue` if the flag is undefined. */
  getVariant<T extends FeatureFlagValue>(
    flag: string,
    defaultValue: T,
    context?: FeatureFlagContext,
  ): T;
  /** All flags currently known to the client (snapshot). */
  snapshot(): Readonly<Record<string, FeatureFlagValue>>;
}

export interface StaticFeatureFlagOptions {
  readonly flags: Readonly<Record<string, FeatureFlagValue>>;
}

/**
 * Minimal in-memory adapter. Use during development or in unit tests.
 * Replace with a remote adapter once the backend feature service ships.
 */
export const createStaticFeatureFlagClient = (
  options: StaticFeatureFlagOptions,
): FeatureFlagClient => {
  const frozen = Object.freeze({ ...options.flags });

  return Object.freeze({
    isEnabled(flag: string): boolean {
      const value = frozen[flag];
      return value === true;
    },
    getVariant<T extends FeatureFlagValue>(flag: string, defaultValue: T): T {
      const value = frozen[flag];
      if (value === undefined) return defaultValue;
      if (typeof value !== typeof defaultValue) return defaultValue;
      return value as T;
    },
    snapshot(): Readonly<Record<string, FeatureFlagValue>> {
      return frozen;
    },
  });
};
