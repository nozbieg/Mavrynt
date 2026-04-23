import { expect, test } from "@playwright/test";

/**
 * Smoke coverage for the web auth UI.
 *
 * These journeys exercise the real `<Providers>` tree (router + i18n +
 * `AuthServiceContext` defaulting to the console adapter) against the
 * dev Vite server. We assert on accessible names / labels, not test-ids,
 * so a failure here is also a signal that the visible copy regressed.
 *
 * The console adapter has two deterministic test triggers:
 *   - `fail+invalid@…` → `invalid_credentials`
 *   - `fail+taken@…`   → `email_taken` (register only)
 *
 * Real backend coverage will replace these once the Users module ships.
 */

test.describe("Login page", () => {
  test("renders the sign-in surface and validates empty submit", async ({
    page,
  }) => {
    await page.goto("/login");

    await expect(
      page.getByRole("heading", { name: /sign in/i, level: 1 }),
    ).toBeVisible();

    await page.getByRole("button", { name: /^sign in$/i }).click();

    // Both fields should now expose their validation text via aria-describedby.
    await expect(page.getByLabel(/email/i)).toHaveAttribute(
      "aria-invalid",
      "true",
    );
    await expect(page.getByLabel(/^password$/i)).toHaveAttribute(
      "aria-invalid",
      "true",
    );
  });

  test("shows the invalid_credentials error for the fail+invalid trigger", async ({
    page,
  }) => {
    await page.goto("/login");

    await page.getByLabel(/email/i).fill("fail+invalid@mavrynt.com");
    await page.getByLabel(/^password$/i).fill("hunter2hunter2");
    await page.getByRole("button", { name: /^sign in$/i }).click();

    const alert = page.getByRole("alert");
    await expect(alert).toBeVisible();
    await expect(alert).toContainText(/email or password is incorrect/i);
  });

  test("navigates home on a successful console-adapter login", async ({
    page,
  }) => {
    await page.goto("/login");

    await page.getByLabel(/email/i).fill("ok@mavrynt.com");
    await page.getByLabel(/^password$/i).fill("hunter2hunter2");
    await page.getByRole("button", { name: /^sign in$/i }).click();

    await expect(page).toHaveURL(/\/$/);
    await expect(
      page.getByRole("heading", { level: 1 }).first(),
    ).toBeVisible();
  });
});

test.describe("Register page", () => {
  test("rejects mismatched password confirmation client-side", async ({
    page,
  }) => {
    await page.goto("/register");

    await expect(
      page.getByRole("heading", { name: /create your account/i, level: 1 }),
    ).toBeVisible();

    await page.getByLabel(/full name/i).fill("Norbert");
    await page.getByLabel(/email/i).fill("new@mavrynt.com");
    await page.getByLabel(/^password$/i).fill("hunter2hunter2");
    await page.getByLabel(/confirm password/i).fill("different-value");
    await page.getByRole("button", { name: /create account/i }).click();

    await expect(page.getByLabel(/confirm password/i)).toHaveAttribute(
      "aria-invalid",
      "true",
    );
  });

  test("surfaces email_taken from the console adapter trigger", async ({
    page,
  }) => {
    await page.goto("/register");

    await page.getByLabel(/full name/i).fill("Norbert");
    await page.getByLabel(/email/i).fill("fail+taken@mavrynt.com");
    await page.getByLabel(/^password$/i).fill("hunter2hunter2");
    await page.getByLabel(/confirm password/i).fill("hunter2hunter2");
    await page.getByRole("button", { name: /create account/i }).click();

    const alert = page.getByRole("alert");
    await expect(alert).toBeVisible();
    await expect(alert).toContainText(/already (exists|registered)/i);
  });

  test("redirects to /login on successful console registration", async ({
    page,
  }) => {
    await page.goto("/register");

    await page.getByLabel(/full name/i).fill("Norbert");
    await page.getByLabel(/email/i).fill("brand-new@mavrynt.com");
    await page.getByLabel(/^password$/i).fill("hunter2hunter2");
    await page.getByLabel(/confirm password/i).fill("hunter2hunter2");
    await page.getByRole("button", { name: /create account/i }).click();

    await expect(page).toHaveURL(/\/login$/);
  });
});
