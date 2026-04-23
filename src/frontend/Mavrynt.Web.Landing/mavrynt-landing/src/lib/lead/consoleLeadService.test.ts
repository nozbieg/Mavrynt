import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { createConsoleLeadService } from "./consoleLeadService.ts";
import type { LeadPayload } from "./types.ts";

/**
 * Console adapter is the development default — the Liskov-compatible
 * fallback that lets the form function end-to-end without a backend.
 * These tests pin two behaviours:
 *   1. it resolves (never rejects) so the form flows into `success`,
 *   2. it logs the payload via `console.warn` (allowed by the shared
 *      ESLint config) so developers can see what would have been sent.
 */
describe("consoleLeadService", () => {
  const payload: LeadPayload = {
    name: "Ada Lovelace",
    email: "ada@example.com",
    message: "Hi, I would love to see a demo of Mavrynt.",
    source: "landing:contact",
    locale: "en",
  };

  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("resolves and logs the payload via console.warn", async () => {
    const warn = vi.spyOn(console, "warn").mockImplementation(() => undefined);
    const service = createConsoleLeadService();

    const promise = service.submit(payload);
    // Drain the simulated 250ms delay without making the test slow.
    await vi.runAllTimersAsync();
    await expect(promise).resolves.toBeUndefined();

    expect(warn).toHaveBeenCalledTimes(1);
    expect(warn).toHaveBeenCalledWith(
      "[LeadService:console] would submit",
      payload,
    );
  });
});
