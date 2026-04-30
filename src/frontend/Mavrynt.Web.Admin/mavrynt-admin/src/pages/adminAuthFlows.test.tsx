import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter, Routes, Route } from "react-router";
import LoginPage from "./LoginPage.tsx";
import DashboardPage from "./DashboardPage.tsx";
import { saveAdminSession, clearAdminSession } from "../lib/auth/adminSession.ts";

vi.mock("@mavrynt/auth-ui", async () => {
  const actual = await vi.importActual<object>("@mavrynt/auth-ui");
  return {
    ...actual,
    LoginForm: ({ onSuccess }: { onSuccess: (session: { requiresPasswordChange?: boolean }) => void }) => (
      <div>
        <button onClick={() => onSuccess({ requiresPasswordChange: false })}>login-ok</button>
        <button onClick={() => onSuccess({ requiresPasswordChange: true })}>login-change</button>
      </div>
    ),
  };
});

describe("admin auth flows", () => {
  beforeEach(() => {
    clearAdminSession();
    vi.restoreAllMocks();
  });

  it("redirects successful login without password change to /dashboard", async () => {
    render(<MemoryRouter initialEntries={["/login"]}><Routes><Route path="/login" element={<LoginPage />} /><Route path="/dashboard" element={<div>Dashboard</div>} /></Routes></MemoryRouter>);
    screen.getByText("login-ok").click();
    await waitFor(() => expect(screen.getByText("Dashboard")).toBeInTheDocument());
  });

  it("redirects successful login with password change to /change-password", async () => {
    render(<MemoryRouter initialEntries={["/login"]}><Routes><Route path="/login" element={<LoginPage />} /><Route path="/change-password" element={<div>Change Password</div>} /></Routes></MemoryRouter>);
    screen.getByText("login-change").click();
    await waitFor(() => expect(screen.getByText("Change Password")).toBeInTheDocument());
  });

  it("dashboard sends bearer token and loads profile", async () => {
    saveAdminSession({ token: "jwt-1", user: { id: "1", email: "a@b.com", roles: ["admin"] } });
    const fetchMock = vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({ email: "a@b.com", displayName: "Admin", role: "admin", status: "Active" }),
    } as Response);

    render(<MemoryRouter><DashboardPage /></MemoryRouter>);

    await waitFor(() => expect(fetchMock).toHaveBeenCalled());
    const headers = (fetchMock.mock.calls[0]?.[1] as RequestInit).headers as Record<string, string>;
    expect(headers.Authorization).toBe("Bearer jwt-1");
  });
});
