import { describe, it, expect, vi } from "vitest";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { FormEvent } from "react";
import {
  AuthError,
  useRegisterForm,
  type AuthSession,
} from "@mavrynt/auth-ui";
import { buildAuthHarness, createAuthServiceMock } from "./authHarness.tsx";

const submitEvent = (): FormEvent<HTMLFormElement> =>
  ({
    preventDefault: vi.fn(),
  }) as unknown as FormEvent<HTMLFormElement>;

/**
 * Unit coverage for `useRegisterForm`.
 *
 * Mirrors `useLoginForm.test.tsx` — intentionally parallel so shared
 * behaviour (state machine, analytics, error mapping, reset) is
 * exercised identically for both hooks. Register-specific assertions
 * live in their own cases (confirm-password cross-field validation,
 * `email_taken` from the console adapter's trigger).
 */
describe("useRegisterForm", () => {
  it("requires every field before calling the auth service", async () => {
    const { Wrapper, auth } = await buildAuthHarness();

    const { result } = renderHook(
      () => useRegisterForm({ source: "test:register" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    expect(result.current.errors.name).toBeDefined();
    expect(result.current.errors.email).toBeDefined();
    expect(result.current.errors.password).toBeDefined();
    expect(result.current.errors.confirmPassword).toBeDefined();
    expect(auth.register).not.toHaveBeenCalled();
  });

  it("rejects mismatched confirmPassword", async () => {
    const { Wrapper } = await buildAuthHarness();
    const { result } = renderHook(
      () => useRegisterForm({ source: "test:register" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("name", "Norbert");
      result.current.setField("email", "a@b.co");
      result.current.setField("password", "hunter2hunter2");
      result.current.setField("confirmPassword", "something-else");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    expect(result.current.errors.confirmPassword).toMatch(/do not match/i);
  });

  it("calls register with trimmed name + email on valid input", async () => {
    const session: AuthSession = {
      token: "tok-123",
      user: { id: "u-1", email: "ok@mavrynt.com", roles: [] },
    };
    const auth = createAuthServiceMock({
      register: async (): Promise<AuthSession> => Promise.resolve(session),
    });
    const onSuccess = vi.fn();
    const { Wrapper, analytics } = await buildAuthHarness({ auth });

    const { result } = renderHook(
      () => useRegisterForm({ source: "web:register", onSuccess }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("name", "  Norbert  ");
      result.current.setField("email", "  ok@mavrynt.com  ");
      result.current.setField("password", "hunter2hunter2");
      result.current.setField("confirmPassword", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    expect(auth.register).toHaveBeenCalledWith(
      expect.objectContaining({
        name: "Norbert",
        email: "ok@mavrynt.com",
        password: "hunter2hunter2",
        source: "web:register",
      }),
    );
    expect(onSuccess).toHaveBeenCalledWith(session);
    expect(analytics.track).toHaveBeenCalledWith(
      "auth_register_success",
      { source: "web:register" },
    );
  });

  it("propagates email_taken from the backend as an AuthErrorCode", async () => {
    const auth = createAuthServiceMock({
      register: async (): Promise<AuthSession> =>
        Promise.reject(new AuthError("email_taken", "taken")),
    });
    const { Wrapper, analytics } = await buildAuthHarness({ auth });

    const { result } = renderHook(
      () => useRegisterForm({ source: "web:register" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("name", "Norbert");
      result.current.setField("email", "taken@mavrynt.com");
      result.current.setField("password", "hunter2hunter2");
      result.current.setField("confirmPassword", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });

    expect(result.current.errorCode).toBe("email_taken");
    expect(analytics.track).toHaveBeenCalledWith("auth_register_error", {
      source: "web:register",
      code: "email_taken",
    });
  });
});
