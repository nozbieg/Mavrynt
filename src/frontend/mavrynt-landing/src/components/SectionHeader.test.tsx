import { describe, it, expect } from "vitest";
import { render, screen } from "@testing-library/react";
import { SectionHeader } from "./SectionHeader.tsx";

/**
 * SectionHeader — ensures the DRY header contract is honoured:
 *  - title always renders as an <h2> (landmark-quality heading for the
 *    section it labels),
 *  - eyebrow and subtitle render only when provided (no empty <span>/<p>),
 *  - `align="center"` applies the centering classes so callers don't
 *    have to reinvent them.
 */
describe("SectionHeader", () => {
  it("renders title as an <h2>", () => {
    render(<SectionHeader title="Pricing" />);
    expect(
      screen.getByRole("heading", { level: 2, name: "Pricing" }),
    ).toBeInTheDocument();
  });

  it("omits the eyebrow and subtitle when they are not provided", () => {
    const { container } = render(<SectionHeader title="Just a title" />);
    expect(container.querySelector("span")).toBeNull();
    expect(container.querySelector("p")).toBeNull();
  });

  it("renders the eyebrow and subtitle when provided", () => {
    render(
      <SectionHeader
        eyebrow="Mavrynt"
        title="Talk to us"
        subtitle="Reply within a day."
      />,
    );
    expect(screen.getByText("Mavrynt")).toBeInTheDocument();
    expect(screen.getByText("Reply within a day.")).toBeInTheDocument();
  });

  it("applies centering classes when align='center'", () => {
    const { container } = render(
      <SectionHeader title="Centered" align="center" />,
    );
    const wrapper = container.firstElementChild;
    expect(wrapper).toHaveClass("mx-auto");
    expect(wrapper).toHaveClass("items-center");
    expect(wrapper).toHaveClass("text-center");
  });

  it("does not apply centering classes by default", () => {
    const { container } = render(<SectionHeader title="Start aligned" />);
    const wrapper = container.firstElementChild;
    expect(wrapper).not.toHaveClass("items-center");
    expect(wrapper).not.toHaveClass("text-center");
  });
});
