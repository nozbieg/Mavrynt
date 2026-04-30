import { describe, expect, it, vi, beforeEach } from "vitest";
import { createAdminHttpAuthService } from "./adminHttpAuthService.ts";
import { clearAdminSession, getAdminSession } from "./adminSession.ts";

describe("createAdminHttpAuthService", () => {
  beforeEach(() => {
    clearAdminSession();
    vi.restoreAllMocks();
  });

  it("maps backend AuthResultDto to AuthSession and stores it", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      json: async () => ({
        accessToken: "access-123",
        expiresAt: "2099-01-01T00:00:00Z",
        requiresPasswordChange: false,
        user: {
          id: "u-1",
          email: "admin@mavrynt.com",
          displayName: "Admin",
          role: "ADMIN",
        },
      }),
    } as Response);

    const session = await createAdminHttpAuthService().login({
      email: "admin@mavrynt.com",
      password: "Secret123!",
      locale: "en",
      source: "admin:login",
    });

    expect(session.token).toBe("access-123");
    expect(session.user.roles).toEqual(["admin"]);
    expect(getAdminSession()?.token).toBe("access-123");
  });
});
