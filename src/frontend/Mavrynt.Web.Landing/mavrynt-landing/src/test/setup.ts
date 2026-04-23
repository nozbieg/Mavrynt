import "@testing-library/jest-dom/vitest";
import { afterEach } from "vitest";
import { cleanup } from "@testing-library/react";

/**
 * Global test setup for `mavrynt-landing`.
 *
 * - Registers the `@testing-library/jest-dom` matchers (`toBeInTheDocument`,
 *   `toHaveAttribute`, etc.) with Vitest's expect.
 * - Unmounts React trees and clears the DOM after every test, so tests
 *   stay isolated even when they omit an explicit cleanup call.
 *
 * Keep this file dependency-free of app code — it runs before every test
 * and must remain cheap.
 */
afterEach(() => {
  cleanup();
});
