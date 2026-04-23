import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";

/**
 * Infrastructure smoke test — proves Vitest, jsdom, Testing Library,
 * and the jest-dom matcher augmentation are all wired correctly.
 *
 * Keep this intentionally trivial: it exists to fail loudly if the
 * test harness itself regresses, not to exercise product code.
 */
describe("test infrastructure", () => {
  it("runs a trivial assertion", () => {
    expect(1 + 1).toBe(2);
  });

  it("renders a React element into jsdom and queries it", () => {
    render(<h1>Mavrynt</h1>);
    expect(
      screen.getByRole("heading", { level: 1, name: "Mavrynt" }),
    ).toBeInTheDocument();
  });
});
