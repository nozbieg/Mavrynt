import { render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it, vi } from "vitest";

vi.mock("../../lib/auth/useAdminAuth", () => ({
  useAdminAuth: vi.fn(() => ({
    isAuthenticated: false,
    user: null,
    logout: vi.fn(),
    session: null,
    accessToken: null,
    refreshSession: vi.fn(),
    setSession: vi.fn(),
  })),
}));

import { useAdminAuth } from "../../lib/auth/useAdminAuth";
import { AdminNav } from "./AdminNav";

describe("AdminNav", () => {
  it("shows Login when unauthenticated", () => {
    render(
      <MemoryRouter>
        <AdminNav />
      </MemoryRouter>,
    );

    expect(screen.getAllByText(/login/i).length).toBeGreaterThan(0);
  });

  it("shows user and Logout when authenticated", () => {
    vi.mocked(useAdminAuth).mockReturnValue({
      isAuthenticated: true,
      user: { id: "1", email: "a@b.com", name: "Admin", roles: ["admin"] },
      logout: vi.fn(),
      session: null,
      accessToken: null,
      refreshSession: vi.fn(),
      setSession: vi.fn(),
    });

    render(
      <MemoryRouter>
        <AdminNav />
      </MemoryRouter>,
    );

    expect(screen.getByText(/Admin/)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /logout/i })).toBeInTheDocument();
  });
});
