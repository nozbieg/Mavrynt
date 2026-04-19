import { expect, test } from "@playwright/test";

/**
 * Journey 1 — Home → Contact happy path.
 *
 * The visitor lands on Home, follows the "Contact" nav link, fills out
 * the form, and submits it. The default `LeadService` in dev is the
 * console adapter (see `src/app/Providers.tsx` + `createConsoleLeadService`),
 * which resolves after a short simulated latency. That's exactly what we
 * want for a smoke test: real components, real router, real i18n, and a
 * deterministic-enough adapter — no network flake.
 *
 * We assert on accessible names (roles/labels) so the test doubles as a
 * light a11y smoke: if a label or role regresses, this test fails.
 */
test.describe("Home → Contact happy path", () => {
  test("submits a lead and shows the success banner", async ({ page }) => {
    // Default site locale is Polish (PL); assertions mirror pl/common.json.
    await page.goto("/");

    // Shell is present (nav landmark, main content).
    await expect(
      page.getByRole("navigation", { name: /nawigacja główna/i }),
    ).toBeVisible();
    await expect(page.getByRole("main")).toBeVisible();

    // Navigate via the primary nav — do NOT hard-code the URL, because
    // we want to exercise the <RouterLink> / react-router pipeline.
    await page.getByRole("link", { name: /^kontakt$/i }).first().click();
    await expect(page).toHaveURL(/\/contact$/);

    // Fill the form using accessible labels; no test-ids — if the labels
    // change, copy decks and screen-readers break too, so a failure here
    // is a real signal.
    await page.getByLabel(/imię i nazwisko/i).fill("Ada Lovelace");
    await page.getByLabel(/email służbowy/i).fill("ada@example.com");
    await page
      .getByLabel(/wiadomość/i)
      .fill(
        "Chcielibyśmy porozmawiać o wdrożeniu Mavrynt w naszym zespole platformowym.",
      );

    // Submit and wait for the success banner. role="status" + polite live
    // region is how screen-reader users learn the submission succeeded.
    await page.getByRole("button", { name: /wyślij wiadomość/i }).click();
    const status = page.getByRole("status");
    await expect(status).toBeVisible();
    await expect(status).toContainText(/wiadomość otrzymana/i);

    // The form itself unmounts on success — sanity check that the submit
    // button is gone so we know we're not in a stuck "submitting" state.
    await expect(
      page.getByRole("button", { name: /wyślij wiadomość/i }),
    ).toHaveCount(0);
  });
});
