import { describe, expect, it, beforeEach, vi } from "vitest";
import { render, screen } from "@testing-library/react";
import { MemoryRouter, Routes, Route } from "react-router";
import { AdminSidebar } from "./components/AdminSidebar.tsx";
import { AdminTopBar } from "./components/AdminTopBar.tsx";
import { RequireAdminAuth } from "../lib/auth/RequireAdminAuth.tsx";
import { saveAdminSession, clearAdminSession } from "../lib/auth/adminSession.ts";
import { AdminSessionProvider } from "../lib/auth/AdminSessionProvider.tsx";

vi.mock("./components/ThemeToggle.tsx", () => ({
  ThemeToggle: () => null,
}));

vi.mock("./components/LanguageSwitcher.tsx", () => ({
  LanguageSwitcher: () => null,
}));

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <AdminSessionProvider>{children}</AdminSessionProvider>
);

describe("AdminSidebar", () => {
  it("renders all nav links", () => {
    render(
      <MemoryRouter>
        <AdminSidebar />
      </MemoryRouter>,
      { wrapper },
    );
    expect(screen.getByRole("link", { name: "Dashboard" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Users" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Feature Flags" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "SMTP Settings" })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Settings" })).toBeInTheDocument();
  });
});

describe("RequireAdminAuth", () => {
  beforeEach(() => clearAdminSession());

  it("redirects unauthenticated users to /login", () => {
    render(
      <MemoryRouter initialEntries={["/dashboard"]}>
        <Routes>
          <Route path="/login" element={<div>Login Page</div>} />
          <Route
            path="/dashboard"
            element={
              <RequireAdminAuth>
                <div>Protected</div>
              </RequireAdminAuth>
            }
          />
        </Routes>
      </MemoryRouter>,
      { wrapper },
    );
    expect(screen.getByText("Login Page")).toBeInTheDocument();
    expect(screen.queryByText("Protected")).not.toBeInTheDocument();
  });

  it("renders protected content for authenticated users", () => {
    saveAdminSession({ token: "tok", user: { id: "1", email: "admin@test.com", roles: ["admin"] } });
    render(
      <MemoryRouter initialEntries={["/dashboard"]}>
        <Routes>
          <Route path="/login" element={<div>Login Page</div>} />
          <Route
            path="/dashboard"
            element={
              <RequireAdminAuth>
                <div>Protected</div>
              </RequireAdminAuth>
            }
          />
        </Routes>
      </MemoryRouter>,
      { wrapper },
    );
    expect(screen.getByText("Protected")).toBeInTheDocument();
  });
});

describe("AdminTopBar", () => {
  beforeEach(() => clearAdminSession());

  it("shows logout when session is active", () => {
    saveAdminSession({ token: "tok", user: { id: "1", email: "admin@test.com", roles: ["admin"] } });
    render(
      <MemoryRouter>
        <AdminTopBar />
      </MemoryRouter>,
      { wrapper },
    );
    expect(screen.getByRole("button", { name: "Logout" })).toBeInTheDocument();
    expect(screen.getByText("admin@test.com")).toBeInTheDocument();
  });

  it("hides logout when no session", () => {
    render(
      <MemoryRouter>
        <AdminTopBar />
      </MemoryRouter>,
      { wrapper },
    );
    expect(screen.queryByRole("button", { name: "Logout" })).not.toBeInTheDocument();
  });
});
