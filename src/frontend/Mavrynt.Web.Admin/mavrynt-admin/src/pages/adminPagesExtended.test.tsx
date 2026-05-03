import { describe, expect, it, vi, beforeEach } from "vitest";
import { render, screen, waitFor, fireEvent } from "@testing-library/react";
import { MemoryRouter, Routes, Route } from "react-router";
import UsersPage from "./UsersPage.tsx";
import FeatureFlagsPage from "./FeatureFlagsPage.tsx";
import SmtpSettingsPage from "./SmtpSettingsPage.tsx";
import SettingsPage from "./SettingsPage.tsx";
import { saveAdminSession, clearAdminSession } from "../lib/auth/adminSession.ts";
import { AdminSessionProvider } from "../lib/auth/AdminSessionProvider.tsx";
import { RequireAdminAuth } from "../lib/auth/RequireAdminAuth.tsx";

const SESSION = {
  token: "tok",
  user: { id: "u-1", email: "admin@test.com", name: "Admin User", roles: ["admin"] as string[] },
} as const;

const wrapper = ({ children }: { children: React.ReactNode }) => (
  <AdminSessionProvider>{children}</AdminSessionProvider>
);

const renderInRouter = (ui: React.ReactNode) =>
  render(<MemoryRouter>{ui}</MemoryRouter>, { wrapper });

const FLAG = {
  id: "f-1",
  key: "test.flag",
  name: "Test Flag",
  description: "A test flag",
  isEnabled: true,
  createdAt: "2024-01-01T00:00:00Z",
};

const SMTP = {
  id: "s-1",
  providerName: "SendGrid",
  host: "smtp.sendgrid.net",
  port: 587,
  username: "apikey",
  senderEmail: "no-reply@test.com",
  senderName: "Mavrynt",
  useSsl: true,
  isEnabled: false,
  createdAt: "2024-01-01T00:00:00Z",
};

beforeEach(() => {
  clearAdminSession();
  vi.restoreAllMocks();
  saveAdminSession(SESSION);
});

// ---------------------------------------------------------------------------
// Route protection
// ---------------------------------------------------------------------------

describe("route protection", () => {
  it("redirects unauthenticated user from /users to /login", () => {
    clearAdminSession();
    render(
      <MemoryRouter initialEntries={["/users"]}>
        <Routes>
          <Route path="/login" element={<div>Login</div>} />
          <Route
            path="/users"
            element={
              <RequireAdminAuth>
                <UsersPage />
              </RequireAdminAuth>
            }
          />
        </Routes>
      </MemoryRouter>,
      { wrapper },
    );
    expect(screen.getByText("Login")).toBeInTheDocument();
  });
});

// ---------------------------------------------------------------------------
// SettingsPage
// ---------------------------------------------------------------------------

describe("SettingsPage", () => {
  it("renders section links", () => {
    renderInRouter(<SettingsPage />);
    expect(screen.getByRole("link", { name: /Manage SMTP settings/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Manage feature flags/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Manage users/i })).toBeInTheDocument();
  });

  it("shows session email when authenticated", () => {
    renderInRouter(<SettingsPage />);
    expect(screen.getByText("admin@test.com")).toBeInTheDocument();
  });

  it("does not crash when no session", () => {
    clearAdminSession();
    renderInRouter(<SettingsPage />);
    expect(screen.getByText(/SMTP Settings/i)).toBeInTheDocument();
  });
});

// ---------------------------------------------------------------------------
// UsersPage
// ---------------------------------------------------------------------------

describe("UsersPage", () => {
  it("shows limitation note and assign role form", () => {
    renderInRouter(<UsersPage />);
    expect(screen.getByText(/User listing is not available/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Assign role/i })).toBeInTheDocument();
  });

  it("shows validation error when user ID is empty", async () => {
    renderInRouter(<UsersPage />);
    fireEvent.click(screen.getByRole("button", { name: /Assign role/i }));
    await waitFor(() =>
      expect(screen.getByRole("alert")).toBeInTheDocument(),
    );
    expect(screen.getByText(/User ID is required/i)).toBeInTheDocument();
  });

  it("shows success after role assignment", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => ({
        id: "u-2",
        email: "jane@test.com",
        role: "admin",
        status: "Active",
        createdAt: "2024-01-01T00:00:00Z",
        requiresPasswordChange: false,
      }),
    } as Response);

    renderInRouter(<UsersPage />);

    fireEvent.change(screen.getByLabelText(/User ID/i), {
      target: { value: "some-uuid" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Assign role/i }));

    await waitFor(() =>
      expect(screen.getByRole("status")).toBeInTheDocument(),
    );
    expect(screen.getByText(/Role updated/i)).toBeInTheDocument();
    expect(screen.getByText("jane@test.com")).toBeInTheDocument();
  });

  it("shows error on 404", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: false,
      status: 404,
      json: async () => null,
    } as Response);

    renderInRouter(<UsersPage />);

    fireEvent.change(screen.getByLabelText(/User ID/i), {
      target: { value: "nonexistent-uuid" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Assign role/i }));

    await waitFor(() =>
      expect(screen.getByRole("alert")).toBeInTheDocument(),
    );
    expect(screen.getByText(/User not found/i)).toBeInTheDocument();
  });

  it("shows error on 400 (invalid role)", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: false,
      status: 400,
      json: async () => null,
    } as Response);

    renderInRouter(<UsersPage />);
    fireEvent.change(screen.getByLabelText(/User ID/i), { target: { value: "uid" } });
    fireEvent.click(screen.getByRole("button", { name: /Assign role/i }));

    await waitFor(() =>
      expect(screen.getByRole("alert")).toBeInTheDocument(),
    );
    expect(screen.getByText(/Invalid role value/i)).toBeInTheDocument();
  });
});

// ---------------------------------------------------------------------------
// FeatureFlagsPage
// ---------------------------------------------------------------------------

describe("FeatureFlagsPage", () => {
  it("renders list of flags", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [FLAG],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);

    await waitFor(() =>
      expect(screen.getByText("Test Flag")).toBeInTheDocument(),
    );
    expect(screen.getByText("test.flag")).toBeInTheDocument();
    expect(screen.getByText("Enabled")).toBeInTheDocument();
  });

  it("shows empty state when no flags", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);

    await waitFor(() =>
      expect(screen.getByText(/No feature flags found/i)).toBeInTheDocument(),
    );
  });

  it("shows New flag button and opens create form", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByRole("button", { name: /New flag/i }));

    fireEvent.click(screen.getByRole("button", { name: /New flag/i }));
    expect(screen.getByLabelText(/Create feature flag form/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Key/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Name/i)).toBeInTheDocument();
  });

  it("validates empty key on create", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByRole("button", { name: /New flag/i }));
    fireEvent.click(screen.getByRole("button", { name: /New flag/i }));

    fireEvent.click(screen.getByRole("button", { name: /Create flag/i }));

    await waitFor(() =>
      expect(screen.getByText(/Key is required/i)).toBeInTheDocument(),
    );
  });

  it("validates key format", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByRole("button", { name: /New flag/i }));
    fireEvent.click(screen.getByRole("button", { name: /New flag/i }));

    fireEvent.change(screen.getByLabelText(/Key/i), {
      target: { value: "Invalid Key With Spaces" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Create flag/i }));

    await waitFor(() =>
      expect(screen.getByText(/lowercase/i)).toBeInTheDocument(),
    );
  });

  it("validates empty name on create", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByRole("button", { name: /New flag/i }));
    fireEvent.click(screen.getByRole("button", { name: /New flag/i }));

    fireEvent.change(screen.getByLabelText(/Key/i), {
      target: { value: "valid.key" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Create flag/i }));

    await waitFor(() =>
      expect(screen.getByText(/Name is required/i)).toBeInTheDocument(),
    );
  });

  it("creates flag successfully and shows success message", async () => {
    const listMock = vi
      .spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [],
      } as Response)
      .mockResolvedValueOnce({
        ok: true,
        status: 201,
        json: async () => ({
          id: "f-new",
          key: "new.flag",
          name: "New Flag",
          isEnabled: false,
          createdAt: "2024-01-01T00:00:00Z",
        }),
      } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByRole("button", { name: /New flag/i }));
    fireEvent.click(screen.getByRole("button", { name: /New flag/i }));

    fireEvent.change(screen.getByLabelText(/Key/i), {
      target: { value: "new.flag" },
    });
    fireEvent.change(screen.getByLabelText(/Name/i), {
      target: { value: "New Flag" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Create flag/i }));

    await waitFor(() =>
      expect(screen.getByRole("status")).toBeInTheDocument(),
    );
    expect(screen.getByText(/Flag "New Flag" created/i)).toBeInTheDocument();
    expect(listMock).toHaveBeenCalledTimes(2);
  });

  it("opens edit form with existing data", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [FLAG],
    } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByText("Test Flag"));

    fireEvent.click(screen.getByRole("button", { name: /Edit Test Flag/i }));
    expect(screen.getByLabelText(/Edit feature flag form/i)).toBeInTheDocument();

    const nameInput = screen.getByLabelText(/Name/i) as HTMLInputElement;
    expect(nameInput.value).toBe("Test Flag");
  });

  it("edits flag successfully", async () => {
    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [FLAG],
      } as Response)
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => ({ ...FLAG, name: "Updated Flag" }),
      } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByText("Test Flag"));

    fireEvent.click(screen.getByRole("button", { name: /Edit Test Flag/i }));
    fireEvent.change(screen.getByLabelText(/Name/i), {
      target: { value: "Updated Flag" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Save changes/i }));

    await waitFor(() =>
      expect(screen.getByText(/Flag "Updated Flag" updated/i)).toBeInTheDocument(),
    );
  });

  it("shows toggle error in list row", async () => {
    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [FLAG],
      } as Response)
      .mockResolvedValueOnce({
        ok: false,
        status: 500,
        json: async () => null,
      } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByText("Test Flag"));

    fireEvent.click(
      screen.getByRole("button", { name: /Disable Test Flag/i }),
    );

    await waitFor(() =>
      expect(screen.getByRole("alert")).toBeInTheDocument(),
    );
  });

  it("shows 409 conflict error on duplicate key", async () => {
    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [],
      } as Response)
      .mockResolvedValueOnce({
        ok: false,
        status: 409,
        json: async () => null,
      } as Response);

    renderInRouter(<FeatureFlagsPage />);
    await waitFor(() => screen.getByRole("button", { name: /New flag/i }));
    fireEvent.click(screen.getByRole("button", { name: /New flag/i }));

    fireEvent.change(screen.getByLabelText(/Key/i), {
      target: { value: "existing.key" },
    });
    fireEvent.change(screen.getByLabelText(/Name/i), {
      target: { value: "Duplicate" },
    });
    fireEvent.click(screen.getByRole("button", { name: /Create flag/i }));

    await waitFor(() =>
      expect(
        screen.getByText(/A flag with this key already exists/i),
      ).toBeInTheDocument(),
    );
  });
});

// ---------------------------------------------------------------------------
// SmtpSettingsPage
// ---------------------------------------------------------------------------

describe("SmtpSettingsPage", () => {
  it("renders list of SMTP configs", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [SMTP],
    } as Response);

    renderInRouter(<SmtpSettingsPage />);

    await waitFor(() =>
      expect(screen.getByText("SendGrid")).toBeInTheDocument(),
    );
    expect(screen.getByText("Inactive")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Activate/i })).toBeInTheDocument();
  });

  it("shows empty state when no configs", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<SmtpSettingsPage />);

    await waitFor(() =>
      expect(
        screen.getByText(/No SMTP configurations yet/i),
      ).toBeInTheDocument(),
    );
  });

  it("validates required fields on create", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() =>
      screen.getByRole("button", { name: /Add configuration/i }),
    );
    fireEvent.click(screen.getByRole("button", { name: /Add configuration/i }));

    fireEvent.click(screen.getByRole("button", { name: /^Save$/i }));

    await waitFor(() => {
      const alerts = screen.getAllByRole("alert");
      expect(alerts.length).toBeGreaterThan(0);
    });
    expect(screen.getByText(/Provider name is required/i)).toBeInTheDocument();
  });

  it("validates port range", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() =>
      screen.getByRole("button", { name: /Add configuration/i }),
    );
    fireEvent.click(screen.getByRole("button", { name: /Add configuration/i }));

    fireEvent.change(screen.getByLabelText(/Provider name/i), {
      target: { value: "Test" },
    });
    fireEvent.change(screen.getByLabelText(/Host/i), {
      target: { value: "smtp.test.com" },
    });
    fireEvent.change(screen.getByLabelText(/^Port/i), {
      target: { value: "99999" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Save$/i }));

    await waitFor(() =>
      expect(
        screen.getByText(/Port must be a number between 1 and 65535/i),
      ).toBeInTheDocument(),
    );
  });

  it("validates email format", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [],
    } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() =>
      screen.getByRole("button", { name: /Add configuration/i }),
    );
    fireEvent.click(screen.getByRole("button", { name: /Add configuration/i }));

    fireEvent.change(screen.getByLabelText(/Provider name/i), {
      target: { value: "Test" },
    });
    fireEvent.change(screen.getByLabelText(/Host/i), {
      target: { value: "smtp.test.com" },
    });
    fireEvent.change(screen.getByLabelText(/^Port/i), {
      target: { value: "587" },
    });
    fireEvent.change(screen.getByLabelText(/Username/i), {
      target: { value: "user" },
    });
    fireEvent.change(screen.getByLabelText(/^Password/i), {
      target: { value: "pass" },
    });
    fireEvent.change(screen.getByLabelText(/Sender email/i), {
      target: { value: "not-an-email" },
    });
    fireEvent.change(screen.getByLabelText(/Sender name/i), {
      target: { value: "Test" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Save$/i }));

    await waitFor(() =>
      expect(
        screen.getByText(/Enter a valid email address/i),
      ).toBeInTheDocument(),
    );
  });

  it("creates SMTP config and shows success", async () => {
    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [],
      } as Response)
      .mockResolvedValueOnce({
        ok: true,
        status: 201,
        json: async () => ({
          ...SMTP,
          id: "s-new",
          providerName: "Mailgun",
          isEnabled: false,
        }),
      } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() =>
      screen.getByRole("button", { name: /Add configuration/i }),
    );
    fireEvent.click(screen.getByRole("button", { name: /Add configuration/i }));

    fireEvent.change(screen.getByLabelText(/Provider name/i), {
      target: { value: "Mailgun" },
    });
    fireEvent.change(screen.getByLabelText(/Host/i), {
      target: { value: "smtp.mailgun.org" },
    });
    fireEvent.change(screen.getByLabelText(/^Port/i), {
      target: { value: "587" },
    });
    fireEvent.change(screen.getByLabelText(/Username/i), {
      target: { value: "postmaster@mg.test.com" },
    });
    fireEvent.change(screen.getByLabelText(/^Password/i), {
      target: { value: "secret" },
    });
    fireEvent.change(screen.getByLabelText(/Sender email/i), {
      target: { value: "no-reply@test.com" },
    });
    fireEvent.change(screen.getByLabelText(/Sender name/i), {
      target: { value: "Test App" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Save$/i }));

    await waitFor(() =>
      expect(screen.getByRole("status")).toBeInTheDocument(),
    );
    expect(
      screen.getByText(/SMTP configuration "Mailgun" created/i),
    ).toBeInTheDocument();
  });

  it("opens edit form without showing password", async () => {
    vi.spyOn(globalThis, "fetch").mockResolvedValue({
      ok: true,
      status: 200,
      json: async () => [SMTP],
    } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() => screen.getByText("SendGrid"));

    fireEvent.click(screen.getByRole("button", { name: /^Edit$/i }));

    const passwordInput = screen.getByLabelText(
      /Password \(leave blank to keep current\)/i,
    ) as HTMLInputElement;
    expect(passwordInput.value).toBe("");
    expect(passwordInput.type).toBe("password");
  });

  it("shows success after activation", async () => {
    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [SMTP],
      } as Response)
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => ({ ...SMTP, isEnabled: true }),
      } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() =>
      screen.getByRole("button", { name: /Activate/i }),
    );

    fireEvent.click(screen.getByRole("button", { name: /Activate/i }));

    await waitFor(() =>
      expect(screen.getByRole("status")).toBeInTheDocument(),
    );
    expect(
      screen.getByText(/SMTP configuration "SendGrid" activated/i),
    ).toBeInTheDocument();
  });

  it("shows API error when save fails", async () => {
    vi.spyOn(globalThis, "fetch")
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: async () => [],
      } as Response)
      .mockResolvedValueOnce({
        ok: false,
        status: 500,
        json: async () => null,
      } as Response);

    renderInRouter(<SmtpSettingsPage />);
    await waitFor(() =>
      screen.getByRole("button", { name: /Add configuration/i }),
    );
    fireEvent.click(screen.getByRole("button", { name: /Add configuration/i }));

    fireEvent.change(screen.getByLabelText(/Provider name/i), {
      target: { value: "Test" },
    });
    fireEvent.change(screen.getByLabelText(/Host/i), {
      target: { value: "smtp.test.com" },
    });
    fireEvent.change(screen.getByLabelText(/^Port/i), {
      target: { value: "587" },
    });
    fireEvent.change(screen.getByLabelText(/Username/i), {
      target: { value: "user" },
    });
    fireEvent.change(screen.getByLabelText(/^Password/i), {
      target: { value: "pass" },
    });
    fireEvent.change(screen.getByLabelText(/Sender email/i), {
      target: { value: "x@test.com" },
    });
    fireEvent.change(screen.getByLabelText(/Sender name/i), {
      target: { value: "X" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Save$/i }));

    await waitFor(() =>
      expect(screen.getByRole("alert")).toBeInTheDocument(),
    );
    expect(screen.getByText(/Request failed \(HTTP 500\)/i)).toBeInTheDocument();
  });
});
