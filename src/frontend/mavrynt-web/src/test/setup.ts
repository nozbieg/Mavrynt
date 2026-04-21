import "@testing-library/jest-dom/vitest";
import { afterEach } from "vitest";
import { cleanup } from "@testing-library/react";

/**
 * Global test setup for `mavrynt-web`.
 *
 * - Registers the `@testing-library/jest-dom` matchers with Vitest's
 *   expect so specs can use `toBeInTheDocument`, `toHaveAttribute`, etc.
 * - Calls `cleanup()` after each test to unmount React trees and reset
 *   the DOM — keeps tests isolated even when specs forget to tear down.
 *
 * Must stay dependency-free of app code: this file loads before every
 * test and should remain cheap.
 */
afterEach(() => {
  cleanup();
});
