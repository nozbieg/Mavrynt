import { describe, it, expect, vi } from "vitest";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { FormEvent } from "react";
import {
  AuthError,
  useLoginForm,
  type AuthSession,
} from "@mavrynt/auth-ui";
import { buildAuthHarness, createAuthServiceMock } from "./authHarness.tsx";

const submitEvent = (): FormEvent<HTMLFormElement> =>
  ({
    preventDefault: vi.fn(),
  }) as unknown as FormEvent<HTMLFormElement>;

/**
 * Unit coverage for `useLoginForm`.
 *
 * The hook is a small state machine:
 *   idle → (submit with bad values) → idle + errors
 *   idle → (submit with good values) → submitting → success | error
 *
 * We drive it with the auth harness so the real i18n resources are
 * loaded — that way, error copy assertions match what the user sees.
 */
describe("useLoginForm", () => {
  it("surfaces validation errors without invoking the auth service", async () => {
    const { Wrapper, auth } = await buildAuthHarness();

    const { result } = renderHook(
      () => useLoginForm({ source: "test:login" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    expect(result.current.status).toBe("idle");
    expect(result.current.errors.email).toBeDefined();
    expect(result.current.errors.password).toBeDefined();
    expect(auth.login).not.toHaveBeenCalled();
  });

  it("rejects an invalid email format", async () => {
    const { Wrapper } = await buildAuthHarness();
    const { result } = renderHook(
      () => useLoginForm({ source: "test:login" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "not-an-email");
      result.current.setField("password", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    expect(result.current.errors.email).toBeDefined();
    expect(result.current.errors.password).toBeUndefined();
  });

  it("enforces the configured password minimum length", async () => {
    const { Wrapper } = await buildAuthHarness();
    const { result } = renderHook(
      () => useLoginForm({ source: "test:login", passwordMinLength: 12 }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "a@b.co");
      result.current.setField("password", "short");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    expect(result.current.errors.password).toMatch(/at least 12/);
  });

  it("transitions idle → submitting → success and fires analytics + onSuccess", async () => {
    const session: AuthSession = {
      token: "tok-123",
      user: { id: "u-1", email: "ok@mavrynt.com", roles: [] },
    };
    const auth = createAuthServiceMock({
      login: async (): Promise<AuthSession> => Promise.resolve(session),
    });
    const onSuccess = vi.fn();
    const { Wrapper, analytics } = await buildAuthHarness({ auth });

    const { result } = renderHook(
      () => useLoginForm({ source: "web:login", onSuccess }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "ok@mavrynt.com");
      result.current.setField("password", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("success");
    });

    expect(auth.login).toHaveBeenCalledWith(
      expect.objectContaining({
        email: "ok@mavrynt.com",
        password: "hunter2hunter2",
        source: "web:login",
      }),
    );
    expect(onSuccess).toHaveBeenCalledWith(session);
    expect(analytics.track).toHaveBeenCalledWith("auth_login_attempt", {
      source: "web:login",
    });
    expect(analytics.track).toHaveBeenCalledWith("auth_login_success", {
      source: "web:login",
    });
  });

  it("captures AuthError.code on failure and fires error analytics", async () => {
    const auth = createAuthServiceMock({
      login: async (): Promise<AuthSession> =>
        Promise.reject(
          new AuthError("invalid_credentials", "nope"),
        ),
    });
    const { Wrapper, analytics } = await buildAuthHarness({ auth });

    const { result } = renderHook(
      () => useLoginForm({ source: "web:login" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "bad@mavrynt.com");
      result.current.setField("password", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });

    expect(result.current.errorCode).toBe("invalid_credentials");
    expect(analytics.track).toHaveBeenCalledWith("auth_login_error", {
      source: "web:login",
      code: "invalid_credentials",
    });
  });

  it("maps non-AuthError rejections to the generic 'server' code", async () => {
    const auth = createAuthServiceMock({
      login: async (): Promise<AuthSession> =>
        Promise.reject(new Error("boom")),
    });
    const { Wrapper } = await buildAuthHarness({ auth });

    const { result } = renderHook(
      () => useLoginForm({ source: "web:login" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "x@mavrynt.com");
      result.current.setField("password", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });

    expect(result.current.errorCode).toBe("server");
  });

  it("clears the error banner once the user types again", async () => {
    const auth = createAuthServiceMock({
      login: async (): Promise<AuthSession> =>
        Promise.reject(new AuthError("invalid_credentials", "nope")),
    });
    const { Wrapper } = await buildAuthHarness({ auth });

    const { result } = renderHook(
      () => useLoginForm({ source: "web:login" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "x@mavrynt.com");
      result.current.setField("password", "hunter2hunter2");
    });
    act(() => {
      result.current.handleSubmit(submitEvent());
    });

    await waitFor(() => {
      expect(result.current.status).toBe("error");
    });

    act(() => {
      result.current.setField("password", "hunter2hunter2!");
    });

    expect(result.current.status).toBe("idle");
    expect(result.current.errorCode).toBeUndefined();
  });

  it("reset() restores the initial idle state", async () => {
    const { Wrapper } = await buildAuthHarness();
    const { result } = renderHook(
      () => useLoginForm({ source: "web:login" }),
      { wrapper: Wrapper },
    );

    act(() => {
      result.current.setField("email", "x@mavrynt.com");
    });
    act(() => {
      result.current.reset();
    });

    expect(result.current.values).toEqual({ email: "", password: "" });
    expect(result.current.errors).toEqual({});
    expect(result.current.status).toBe("idle");
  });
});
