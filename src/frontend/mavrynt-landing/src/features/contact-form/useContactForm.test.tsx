import { describe, it, expect, beforeEach, vi } from "vitest";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { FormEvent } from "react";
import { useContactForm, type ContactFormValues } from "./useContactForm.ts";
import { LeadSubmissionError } from "../../lib/lead/index.ts";
import {
  buildHarness,
  createLeadServiceMock,
  createAnalyticsMock,
  type TestHarness,
} from "../../test/harness.tsx";

/**
 * useContactForm — the single source of truth for contact-form state.
 *
 * These tests verify the state machine's observable contract:
 *   - initial state is `idle` with empty values and no errors,
 *   - validation runs on submit and short-circuits when invalid,
 *   - successful submissions trim inputs, emit analytics, and transition
 *     `idle → submitting → success`,
 *   - failures transition to `error` with the analytics `code` pulled
 *     from `LeadSubmissionError`,
 *   - edits clear per-field errors (reduces visual noise),
 *   - `reset()` returns the hook to its initial state,
 *   - a second submit while `submitting` is a no-op (re-entrancy guard).
 *
 * The hook is composed of i18n + analytics + leadService, so we drive
 * it through the real i18n instance and mock contexts (harness.tsx).
 */

const SOURCE = "landing:contact";

const VALID_INPUT: ContactFormValues = {
  name: "Ada Lovelace",
  email: "ada@example.com",
  company: "Analytical Engines",
  message: "We would love to evaluate Mavrynt for our platform rewrite.",
};

/** Helper: drive `setField` for each key of a values object. */
const fillAll = (
  setField: (f: keyof ContactFormValues, v: string) => void,
  values: ContactFormValues,
): void => {
  (Object.keys(values) as Array<keyof ContactFormValues>).forEach((key) => {
    setField(key, values[key]);
  });
};

const fakeFormEvent = () =>
  ({ preventDefault: vi.fn() }) as unknown as FormEvent<HTMLFormElement>;

describe("useContactForm", () => {
  let harness: TestHarness;

  beforeEach(async () => {
    harness = await buildHarness();
  });

  it("starts in the idle state with empty values", () => {
    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: harness.Wrapper,
    });
    expect(result.current.status).toBe("idle");
    expect(result.current.errors).toEqual({});
    expect(result.current.values).toEqual({
      name: "",
      email: "",
      company: "",
      message: "",
    });
  });

  it("reports required/email/minLength validation errors and does not submit", () => {
    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: harness.Wrapper,
    });

    act(() => {
      result.current.setField("email", "not-an-email");
      result.current.setField("message", "short");
    });

    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });

    expect(result.current.status).toBe("idle");
    expect(result.current.errors.name).toBeDefined();
    expect(result.current.errors.email).toBeDefined();
    expect(result.current.errors.message).toBeDefined();
    // Analytics must NOT fire on a failed client-side validation.
    expect(harness.analytics.track).not.toHaveBeenCalled();
    expect(harness.leadService.submit).not.toHaveBeenCalled();
  });

  it("clears a field's error as the user types into it", () => {
    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: harness.Wrapper,
    });
    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });
    expect(result.current.errors.name).toBeDefined();

    act(() => {
      result.current.setField("name", "A");
    });
    expect(result.current.errors.name).toBeUndefined();
  });

  it("transitions idle → submitting → success, trims values, and emits analytics", async () => {
    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: harness.Wrapper,
    });

    act(() => {
      fillAll(result.current.setField, {
        ...VALID_INPUT,
        name: `  ${VALID_INPUT.name}  `,
        email: `  ${VALID_INPUT.email}  `,
      });
    });

    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });

    // Synchronous transition to `submitting`.
    expect(result.current.status).toBe("submitting");
    expect(harness.analytics.track).toHaveBeenCalledWith(
      "lead_submit_attempt",
      { source: SOURCE },
    );

    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    expect(harness.leadService.submit).toHaveBeenCalledTimes(1);
    expect(harness.leadService.submit).toHaveBeenCalledWith({
      name: VALID_INPUT.name,
      email: VALID_INPUT.email,
      company: VALID_INPUT.company,
      message: VALID_INPUT.message,
      source: SOURCE,
      locale: "en",
    });
    expect(harness.analytics.track).toHaveBeenCalledWith(
      "lead_submit_success",
      { source: SOURCE },
    );
  });

  it("omits `company` from the payload when the field is blank", async () => {
    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: harness.Wrapper,
    });

    act(() => {
      fillAll(result.current.setField, { ...VALID_INPUT, company: "   " });
    });
    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    const submitMock = vi.mocked(harness.leadService.submit);
    const submitted = submitMock.mock.calls[0]?.[0];
    expect(submitted).toBeDefined();
    expect(submitted).not.toHaveProperty("company");
  });

  it("maps a LeadSubmissionError to the analytics `code` and status=error", async () => {
    const leadService = createLeadServiceMock(async () => {
      throw new LeadSubmissionError("rate_limited", "Slow down.");
    });
    const analytics = createAnalyticsMock();
    const localHarness = await buildHarness({ leadService, analytics });

    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: localHarness.Wrapper,
    });
    act(() => {
      fillAll(result.current.setField, VALID_INPUT);
    });
    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });
    expect(analytics.track).toHaveBeenCalledWith("lead_submit_error", {
      source: SOURCE,
      code: "rate_limited",
    });
  });

  it("falls back to `server` code when a non-LeadSubmissionError is thrown", async () => {
    const leadService = createLeadServiceMock(async () => {
      throw new Error("boom");
    });
    const analytics = createAnalyticsMock();
    const localHarness = await buildHarness({ leadService, analytics });

    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: localHarness.Wrapper,
    });
    act(() => {
      fillAll(result.current.setField, VALID_INPUT);
    });
    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });
    expect(analytics.track).toHaveBeenCalledWith("lead_submit_error", {
      source: SOURCE,
      code: "server",
    });
  });

  it("ignores re-entrant submits while already submitting", async () => {
    // Never-resolving submit keeps us stuck in `submitting`.
    const leadService = createLeadServiceMock(
      () => new Promise<void>(() => undefined),
    );
    const localHarness = await buildHarness({ leadService });

    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: localHarness.Wrapper,
    });
    act(() => {
      fillAll(result.current.setField, VALID_INPUT);
    });
    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });
    expect(result.current.status).toBe("submitting");

    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });
    expect(leadService.submit).toHaveBeenCalledTimes(1);
  });

  it("reset() returns to idle with cleared values and errors", async () => {
    const { result } = renderHook(() => useContactForm(SOURCE), {
      wrapper: harness.Wrapper,
    });
    act(() => {
      fillAll(result.current.setField, VALID_INPUT);
    });
    act(() => {
      result.current.handleSubmit(fakeFormEvent());
    });
    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    act(() => {
      result.current.reset();
    });
    expect(result.current.status).toBe("idle");
    expect(result.current.errors).toEqual({});
    expect(result.current.values).toEqual({
      name: "",
      email: "",
      company: "",
      message: "",
    });
  });
});
