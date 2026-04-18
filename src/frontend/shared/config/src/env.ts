/**
 * Typed environment loader.
 *
 * Vite exposes environment variables on `import.meta.env`. This module
 * provides a safe, schema-driven wrapper so each app declares the variables
 * it needs and gets type-safe, validated access.
 *
 * SOLID:
 * - Single Responsibility: just env validation, nothing else.
 * - Open/Closed: callers extend by passing their own descriptor map.
 * - Dependency Inversion: the raw source (`import.meta.env` or process.env)
 *   is injected, so this module is testable without Vite.
 */

export type EnvKind = "string" | "number" | "boolean" | "url";

export interface EnvVarDescriptor<T> {
  readonly kind: EnvKind;
  readonly required?: boolean;
  readonly default?: T;
  readonly description?: string;
}

export type EnvSchema = Record<string, EnvVarDescriptor<unknown>>;

export type EnvValue<D extends EnvVarDescriptor<unknown>> = D["kind"] extends
  | "string"
  | "url"
  ? string
  : D["kind"] extends "number"
    ? number
    : D["kind"] extends "boolean"
      ? boolean
      : never;

export type ParsedEnv<S extends EnvSchema> = {
  readonly [K in keyof S]: S[K] extends { required: false }
    ? EnvValue<S[K]> | undefined
    : EnvValue<S[K]>;
};

export class EnvValidationError extends Error {
  constructor(
    message: string,
    readonly key: string,
  ) {
    super(message);
    this.name = "EnvValidationError";
  }
}

const parseValue = (
  key: string,
  raw: string | undefined,
  descriptor: EnvVarDescriptor<unknown>,
): unknown => {
  if (raw === undefined || raw === "") {
    if (descriptor.default !== undefined) return descriptor.default;
    if (descriptor.required === false) return undefined;
    throw new EnvValidationError(
      `Required env variable "${key}" is missing`,
      key,
    );
  }

  switch (descriptor.kind) {
    case "string":
      return raw;
    case "number": {
      const n = Number(raw);
      if (Number.isNaN(n)) {
        throw new EnvValidationError(
          `Env variable "${key}" must be a number, got "${raw}"`,
          key,
        );
      }
      return n;
    }
    case "boolean":
      return raw === "true" || raw === "1";
    case "url":
      try {
        return new URL(raw).toString();
      } catch {
        throw new EnvValidationError(
          `Env variable "${key}" must be a valid URL, got "${raw}"`,
          key,
        );
      }
  }
};

/**
 * Validate and freeze the env according to a schema.
 *
 * @param schema  the variables this app needs (Vite convention: `VITE_*`).
 * @param source  raw key/value source. Defaults to `import.meta.env`.
 */
export const loadEnv = <S extends EnvSchema>(
  schema: S,
  source: Readonly<Record<string, string | undefined>> = (
    import.meta as ImportMeta
  ).env as unknown as Record<string, string | undefined>,
): ParsedEnv<S> => {
  const result: Record<string, unknown> = {};
  for (const key of Object.keys(schema)) {
    const descriptor = schema[key];
    if (!descriptor) continue;
    result[key] = parseValue(key, source[key], descriptor);
  }
  return Object.freeze(result) as ParsedEnv<S>;
};

/** Helper to declare a schema with full inference. */
export const defineEnvSchema = <S extends EnvSchema>(schema: S): S => schema;
