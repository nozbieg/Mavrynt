import "@testing-library/jest-dom/vitest";
import { afterEach } from "vitest";
import { cleanup } from "@testing-library/react";

/**
 * Global test setup for `mavrynt-admin`.
 *
 * Registers jest-dom matchers with Vitest's expect and unmounts React
 * trees after each test so specs stay isolated. Keep dependency-free
 * of app code — this file runs before every test.
 */
afterEach(() => {
  cleanup();
});
