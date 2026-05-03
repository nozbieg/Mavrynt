import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import FeatureFlagsPage from "./FeatureFlagsPage.tsx";
import SmtpSettingsPage from "./SmtpSettingsPage.tsx";
import UsersPage from "./UsersPage.tsx";
import { saveAdminSession, clearAdminSession } from "../lib/auth/adminSession.ts";
import { AdminSessionProvider } from "../lib/auth/AdminSessionProvider.tsx";

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <AdminSessionProvider>{children}</AdminSessionProvider>
);

beforeEach(() => {
  clearAdminSession();
  vi.restoreAllMocks();
  saveAdminSession({ token: "tok", user: { id: "1", email: "admin@test.com", roles: ["admin"] } });
});

describe("FeatureFlagsPage", () => {
  it("shows loading then list of flags", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [
        { id: "1", key: "test.flag", name: "Test Flag", isEnabled: true, createdAt: "2024-01-01T00:00:00Z" },
      ],
    } as Response);

    render(<MemoryRouter><FeatureFlagsPage /></MemoryRouter>, { wrapper });

    await waitFor(() => expect(screen.getByText("Test Flag")).toBeInTheDocument());
    expect(screen.getByText("Enabled")).toBeInTheDocument();
  });

  it("shows error state on fetch failure", async () => {
    vi.spyOn(globalThis, "fetch").mockRejectedValue(new Error("Network error"));
    render(<MemoryRouter><FeatureFlagsPage /></MemoryRouter>, { wrapper });
    await waitFor(() =>
      expect(screen.getByRole("alert")).toBeInTheDocument(),
    );
  });
});

describe("SmtpSettingsPage", () => {
  it("shows empty state when no settings", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    render(<MemoryRouter><SmtpSettingsPage /></MemoryRouter>, { wrapper });

    await waitFor(() =>
      expect(screen.getByText(/No SMTP configurations yet/i)).toBeInTheDocument(),
    );
  });

  it("shows SMTP settings list", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [
        {
          id: "s-1",
          providerName: "SendGrid",
          host: "smtp.sendgrid.net",
          port: 587,
          username: "apikey",
          senderEmail: "no-reply@test.com",
          senderName: "Mavrynt",
          useSsl: true,
          isEnabled: true,
          createdAt: "2024-01-01T00:00:00Z",
        },
      ],
    } as Response);

    render(<MemoryRouter><SmtpSettingsPage /></MemoryRouter>, { wrapper });

    await waitFor(() => expect(screen.getByText("SendGrid")).toBeInTheDocument());
    expect(screen.getByText("Active")).toBeInTheDocument();
  });
});

describe("UsersPage", () => {
  it("shows limitation note about missing list endpoint", () => {
    render(<MemoryRouter><UsersPage /></MemoryRouter>, { wrapper });
    expect(screen.getByText(/User listing is not available/i)).toBeInTheDocument();
  });

  it("renders assign role form", () => {
    render(<MemoryRouter><UsersPage /></MemoryRouter>, { wrapper });
    expect(screen.getByRole("button", { name: /Assign role/i })).toBeInTheDocument();
  });
});
