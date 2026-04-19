import { expect, test, type Locator, type Page } from "@playwright/test";

/**
 * Journey 2 — Pricing → FAQ expand.
 *
 * The FAQ accordion on the Pricing page uses native `<details>` +
 * `<summary>` with `name="faq"`, giving us an exclusive accordion for
 * free (opening one closes the others). That wiring is easy to break
 * accidentally, so the smoke test asserts both:
 *   - opening a question reveals its answer,
 *   - opening a second question collapses the first.
 *
 * Implementation notes:
 *   - We locate <details> by the heading text it contains — more
 *     resilient than css classes and more explicit than nth-of-type.
 *   - We assert open/closed state via the `open` DOM property
 *     (`toHaveJSProperty`) because it is the canonical source of truth
 *     for <details>; the `open` HTML attribute is a boolean and can
 *     be awkward to match with `toHaveAttribute`.
 */

const faqItem = (page: Page, questionPattern: RegExp): Locator =>
  page.locator("details").filter({
    has: page.getByRole("heading", { level: 3, name: questionPattern }),
  });

test.describe("Pricing → FAQ", () => {
  test("expands one question and collapses the previous one", async ({
    page,
  }) => {
    await page.goto("/pricing");

    // Pricing heading anchors the page (and proves the PricingMatrix chunk
    // loaded — catches a regression in route-level code-splitting).
    await expect(page.getByRole("heading", { name: /^cennik$/i })).toBeVisible();

    // FAQ section landmark (Section has aria-labelledby → "faq-title").
    const faqRegion = page.getByRole("region", { name: /najczęściej zadawane/i });
    await expect(faqRegion).toBeVisible();

    const q1 = faqItem(page, /czy mavrynt jest open source/i);
    const q2 = faqItem(page, /dlaczego modularny monolit/i);

    await expect(q1).toBeVisible();
    await expect(q2).toBeVisible();

    // Both start closed.
    await expect(q1).toHaveJSProperty("open", false);
    await expect(q2).toHaveJSProperty("open", false);

    // Open Q1 by clicking its heading — <summary> forwards the click to
    // toggle the surrounding <details>.
    await q1.getByRole("heading", { level: 3 }).click();
    await expect(q1).toHaveJSProperty("open", true);
    await expect(
      q1.getByText(/open source na licencji mit/i),
    ).toBeVisible();

    // Opening Q2 closes Q1 (exclusive accordion via `name="faq"`).
    await q2.getByRole("heading", { level: 3 }).click();
    await expect(q2).toHaveJSProperty("open", true);
    await expect(q1).toHaveJSProperty("open", false);
  });
});
