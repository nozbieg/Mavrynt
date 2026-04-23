import { describe, it, expect, beforeEach } from "vitest";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ContactForm } from "./ContactForm.tsx";
import { LeadSubmissionError } from "../../lib/lead/index.ts";
import {
  buildHarness,
  createLeadServiceMock,
  type TestHarness,
} from "../../test/harness.tsx";

/**
 * ContactForm — accessibility-first integration tests.
 *
 * These assertions document the form's WCAG 2.1 AA contract:
 *   - every input has an accessible name via <label htmlFor>,
 *   - required fields carry `aria-required` via the `required` attr,
 *   - error messaging is programmatically associated with its input
 *     (aria-invalid + aria-describedby → error id),
 *   - the submit error banner is announced via role="alert",
 *   - the success banner is announced as a status update (role="status").
 *
 * Breaking any of these should be loud — they are what screen-reader
 * users depend on.
 */
describe("ContactForm (accessibility)", () => {
  let harness: TestHarness;

  beforeEach(async () => {
    harness = await buildHarness();
  });

  const renderForm = (h: TestHarness = harness): void => {
    render(<ContactForm source="test" />, { wrapper: h.Wrapper });
  };

  it("gives every field an accessible name via <label htmlFor>", () => {
    renderForm();
    expect(screen.getByLabelText(/name/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/company/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/message/i)).toBeInTheDocument();
  });

  it("marks required inputs and leaves optional ones unmarked", () => {
    renderForm();
    expect(screen.getByLabelText(/name/i)).toBeRequired();
    expect(screen.getByLabelText(/email/i)).toBeRequired();
    expect(screen.getByLabelText(/message/i)).toBeRequired();
    // `company` is optional — we must NOT mark it required.
    expect(screen.getByLabelText(/company/i)).not.toBeRequired();
  });

  it("applies autocomplete tokens so browsers/password managers help users", () => {
    renderForm();
    expect(screen.getByLabelText(/name/i)).toHaveAttribute(
      "autocomplete",
      "name",
    );
    expect(screen.getByLabelText(/email/i)).toHaveAttribute(
      "autocomplete",
      "email",
    );
    expect(screen.getByLabelText(/company/i)).toHaveAttribute(
      "autocomplete",
      "organization",
    );
  });

  it("wires aria-invalid + aria-describedby → error id after a failed submit", async () => {
    const user = userEvent.setup();
    renderForm();

    await user.click(screen.getByRole("button", { name: /send message/i }));

    const emailInput = screen.getByLabelText(/email/i);
    expect(emailInput).toHaveAttribute("aria-invalid", "true");
    const describedBy = emailInput.getAttribute("aria-describedby");
    expect(describedBy).not.toBeNull();
    // The referenced element exists and carries the error copy.
    const errorEl = document.getElementById(describedBy ?? "");
    expect(errorEl).not.toBeNull();
    expect(errorEl?.textContent).toMatch(/required/i);
  });

  it("clears aria-invalid as the user edits the field", async () => {
    const user = userEvent.setup();
    renderForm();

    await user.click(screen.getByRole("button", { name: /send message/i }));
    const nameInput = screen.getByLabelText(/name/i);
    expect(nameInput).toHaveAttribute("aria-invalid", "true");

    await user.type(nameInput, "Ada");
    expect(nameInput).toHaveAttribute("aria-invalid", "false");
  });

  it("renders the server-error banner with role='alert'", async () => {
    const user = userEvent.setup();
    const failing = createLeadServiceMock(async () => {
      throw new LeadSubmissionError("server", "boom");
    });
    const h = await buildHarness({ leadService: failing });

    renderForm(h);
    await user.type(screen.getByLabelText(/name/i), "Ada Lovelace");
    await user.type(screen.getByLabelText(/email/i), "ada@example.com");
    await user.type(
      screen.getByLabelText(/message/i),
      "A sufficiently long message for validation.",
    );
    await user.click(screen.getByRole("button", { name: /send message/i }));

    const alert = await screen.findByRole("alert");
    expect(alert).toBeInTheDocument();
    expect(alert.textContent).toMatch(/something went wrong/i);
    // The form advertises the error id via aria-describedby while in error state.
    const form = alert.closest("form");
    expect(form).toHaveAttribute("aria-describedby", alert.id);
  });

  it("renders the success banner with role='status'", async () => {
    const user = userEvent.setup();
    renderForm();
    await user.type(screen.getByLabelText(/name/i), "Ada Lovelace");
    await user.type(screen.getByLabelText(/email/i), "ada@example.com");
    await user.type(
      screen.getByLabelText(/message/i),
      "A sufficiently long message for validation.",
    );
    await user.click(screen.getByRole("button", { name: /send message/i }));

    await waitFor(() => {
      expect(screen.getByRole("status")).toBeInTheDocument();
    });
    // Form itself is gone — the banner replaces it.
    expect(screen.queryByRole("button", { name: /send message/i })).toBeNull();
  });

  it("uses noValidate on the form so JS validation owns the UX", () => {
    renderForm();
    // `noValidate` is reflected as the `novalidate` attribute in HTML.
    const form = screen.getByLabelText(/name/i).closest("form");
    expect(form).not.toBeNull();
    expect(form).toHaveAttribute("novalidate");
  });
});
