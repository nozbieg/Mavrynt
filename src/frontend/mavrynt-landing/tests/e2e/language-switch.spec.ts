import { expect, test } from "@playwright/test";

/**
 * Journey 3 — Language switch (PL ↔ EN).
 *
 * Mavrynt-landing is a bilingual marketing site (PL default, EN secondary).
 * A locale swap must:
 *   1. swap the visible copy (the hero title here is a strong signal —
 *      it's distinct in both languages and anchored at the top of Home),
 *   2. persist via `persistLocale` so the next visit respects the choice,
 *   3. sync `<html lang>` so screen readers pick the right pronunciation
 *      dictionary (WCAG 3.1.1 / 3.1.2 — the whole point of the hook).
 *
 * A regression in any of these is a major accessibility + localisation
 * bug, hence the smoke-test coverage.
 */

// The landing page renders two <LanguageSwitcher>s (nav + footer). Both
// wrap a native <select>; we target the one inside the navigation
// landmark to avoid ambiguity.
const navLocaleSelect = (page: import("@playwright/test").Page) =>
  page.getByRole("navigation", { name: /nawigacja główna|primary navigation/i })
    .locator("select");

test.describe("Language switcher", () => {
  test("switches the site between Polish and English", async ({ page }) => {
    await page.goto("/");

    // --- Start in Polish (the default locale). -------------------------
    await expect(page.locator("html")).toHaveAttribute("lang", "pl");
    // Hero h1 is the loudest content signal — pick the start of the PL string.
    const plHero = /dostarczaj szybciej/i;
    const enHero = /ship faster with a modular monolith/i;

    await expect(page.getByRole("heading", { level: 1 })).toContainText(plHero);

    // --- Switch to English. -------------------------------------------
    await navLocaleSelect(page).selectOption("en");

    // `<html lang>` is synced by the useLocale effect.
    await expect(page.locator("html")).toHaveAttribute("lang", "en");

    // Copy swaps — wait for the re-render to settle.
    await expect(page.getByRole("heading", { level: 1 })).toContainText(enHero);
    // Nav labels flip too — "Kontakt" → "Contact".
    await expect(
      page.getByRole("link", { name: /^contact$/i }).first(),
    ).toBeVisible();

    // --- Switch back to Polish to confirm the round-trip works. -------
    await navLocaleSelect(page).selectOption("pl");
    await expect(page.locator("html")).toHaveAttribute("lang", "pl");
    await expect(page.getByRole("heading", { level: 1 })).toContainText(plHero);
  });
});
