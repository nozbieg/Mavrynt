import { describe, it, expect } from "vitest";
import { render } from "@testing-library/react";
import { Icon } from "./Icon.tsx";

/**
 * Icon — structural tests only. We do not snapshot path data; the path
 * strings live in the component and are covered by the type-level
 * discriminated union (`IconName`). What we validate here:
 *  - the rendered element is an <svg> carrying the shared visual contract,
 *  - props (size, className, aria overrides) pass through cleanly,
 *  - icons default to decorative (`aria-hidden="true"`).
 */
describe("Icon", () => {
  it("renders a decorative svg with the default 24x24 viewbox", () => {
    const { container } = render(<Icon name="check" />);
    const svg = container.querySelector("svg");
    expect(svg).not.toBeNull();
    expect(svg).toHaveAttribute("width", "24");
    expect(svg).toHaveAttribute("height", "24");
    expect(svg).toHaveAttribute("viewBox", "0 0 24 24");
    expect(svg).toHaveAttribute("aria-hidden", "true");
    expect(svg).toHaveAttribute("stroke", "currentColor");
    expect(svg?.querySelectorAll("path").length).toBeGreaterThan(0);
  });

  it("honours the `size` prop for both width and height", () => {
    const { container } = render(<Icon name="mail" size={16} />);
    const svg = container.querySelector("svg");
    expect(svg).toHaveAttribute("width", "16");
    expect(svg).toHaveAttribute("height", "16");
  });

  it("forwards className so icons can be coloured/sized by callers", () => {
    const { container } = render(
      <Icon name="arrow-right" className="text-primary" />,
    );
    expect(container.querySelector("svg")).toHaveClass("text-primary");
  });

  it("allows overriding aria-hidden for meaningful icons", () => {
    const { container } = render(
      <Icon name="mail" aria-hidden="false" aria-label="Email" />,
    );
    const svg = container.querySelector("svg");
    expect(svg).toHaveAttribute("aria-hidden", "false");
    expect(svg).toHaveAttribute("aria-label", "Email");
  });
});
