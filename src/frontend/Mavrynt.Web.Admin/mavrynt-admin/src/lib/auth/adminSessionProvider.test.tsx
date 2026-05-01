import { describe, expect, it, beforeEach, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { AdminSessionProvider, useAdminSession } from "./AdminSessionProvider.tsx";
import { saveAdminSession, clearAdminSession } from "./adminSession.ts";
import type { AuthSession } from "@mavrynt/auth-ui";

const mockSession: AuthSession = {
  token: "tok-1",
  user: { id: "u-1", email: "admin@test.com", roles: ["admin"] },
};

const SessionDisplay = () => {
  const { session, logout } = useAdminSession();
  return (
    <div>
      <span data-testid="email">{session?.user.email ?? "none"}</span>
      <button type="button" onClick={logout}>logout</button>
    </div>
  );
};

describe("AdminSessionProvider", () => {
  beforeEach(() => {
    clearAdminSession();
    vi.restoreAllMocks();
  });

  it("initializes with null session when localStorage is empty", () => {
    render(
      <AdminSessionProvider>
        <SessionDisplay />
      </AdminSessionProvider>,
    );
    expect(screen.getByTestId("email").textContent).toBe("none");
  });

  it("initializes with existing session from localStorage", () => {
    saveAdminSession(mockSession);
    render(
      <AdminSessionProvider>
        <SessionDisplay />
      </AdminSessionProvider>,
    );
    expect(screen.getByTestId("email").textContent).toBe("admin@test.com");
  });

  it("logout clears the session", () => {
    saveAdminSession(mockSession);
    render(
      <AdminSessionProvider>
        <SessionDisplay />
      </AdminSessionProvider>,
    );
    fireEvent.click(screen.getByRole("button", { name: "logout" }));
    expect(screen.getByTestId("email").textContent).toBe("none");
  });
});
