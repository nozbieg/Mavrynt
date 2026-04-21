import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import {
  AuthError,
  createConsoleAuthService,
  type LoginCredentials,
  type RegisterCredentials,
} from "@mavrynt/auth-ui";

/**
 * Unit coverage for the default development adapter.
 *
 * The console adapter is the fallback `AuthService` consuming apps get
 * when they don't inject an HTTP adapter via `AuthServiceContext`. It
 * has two deterministic test triggers on the email local-part so demos
 * and tests can exercise the error branches without a backend:
 *   - `fail+invalid@…` → AuthError("invalid_credentials")
 *   - `fail+taken@…`   → AuthError("email_taken") (register only)
 *
 * We silence `console.warn` per-spec because the adapter is chatty by
 * design (it documents every call), and a noisy test suite buries real
 * failures.
 */

const baseLogin: LoginCredentials = {
  email: "ok@mavrynt.com",
  password: "hunter2hunter2",
  locale: "en",
  source: "test:login",
};

const baseRegister: RegisterCredentials = {
  name: "Norbert",
  email: "ok@mavrynt.com",
  password: "hunter2hunter2",
  locale: "en",
  source: "test:register",
};

describe("createConsoleAuthService", () => {
  let warnSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    warnSpy = vi.spyOn(console, "warn").mockImplementation(() => {
      /* silenced */
    });
  });

  afterEach(() => {
    warnSpy.mockRestore();
  });

  it("resolves login with a mock session echoing the submitted email", async () => {
    const auth = createConsoleAuthService({ latencyMs: 0 });

    const session = await auth.login(baseLogin);

    expect(session.token).toMatch(/^mock-/);
    expect(session.user.email).toBe("ok@mavrynt.com");
    expect(session.user.roles).toEqual([]);
  });

  it("embeds the configured roles on the mock user (admin behaviour)", async () => {
    const auth = createConsoleAuthService({
      latencyMs: 0,
      roles: ["admin"],
    });

    const session = await auth.login(baseLogin);

    expect(session.user.roles).toEqual(["admin"]);
  });

  it("throws AuthError('invalid_credentials') on the fail+invalid trigger", async () => {
    const auth = createConsoleAuthService({ latencyMs: 0 });

    await expect(
      auth.login({ ...baseLogin, email: "fail+invalid@mavrynt.com" }),
    ).rejects.toMatchObject({
      name: "AuthError",
      code: "invalid_credentials",
    });
  });

  it("resolves register with a mock session using the supplied name", async () => {
    const auth = createConsoleAuthService({ latencyMs: 0 });

    const session = await auth.register(baseRegister);

    expect(session.user.name).toBe("Norbert");
    expect(session.user.email).toBe("ok@mavrynt.com");
  });

  it("throws AuthError('email_taken') on the fail+taken register trigger", async () => {
    const auth = createConsoleAuthService({ latencyMs: 0 });

    await expect(
      auth.register({ ...baseRegister, email: "fail+taken@mavrynt.com" }),
    ).rejects.toMatchObject({
      name: "AuthError",
      code: "email_taken",
    });
  });

  it("does not throw on invalid_credentials when called via register (login-only trigger)", async () => {
    const auth = createConsoleAuthService({ latencyMs: 0 });

    await expect(
      auth.register({ ...baseRegister, email: "fail+invalid@mavrynt.com" }),
    ).resolves.toMatchObject({ token: expect.stringMatching(/^mock-/) });
  });

  it("logout is idempotent and does not reject", async () => {
    const auth = createConsoleAuthService({ latencyMs: 0 });

    await expect(auth.logout()).resolves.toBeUndefined();
    await expect(auth.logout()).resolves.toBeUndefined();
  });

  it("AuthError preserves the original cause when provided", () => {
    const cause = new Error("underlying");
    const err = new AuthError("server", "boom", { cause });

    expect(err.code).toBe("server");
    expect(err.cause).toBe(cause);
  });
});
