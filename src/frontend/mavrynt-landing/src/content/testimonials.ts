/**
 * Testimonial descriptor — rotates in the Testimonials section.
 * All copy (quote, author, role) lives under `testimonials.items.<id>`.
 */
export interface TestimonialItem {
  readonly id: "1" | "2" | "3";
  /** Initials shown in the avatar ring — kept here because it's presentation, not content. */
  readonly initials: string;
}

export const testimonials: ReadonlyArray<TestimonialItem> = [
  { id: "1", initials: "AK" },
  { id: "2", initials: "MN" },
  { id: "3", initials: "LS" },
];
