import type { ReactNode } from "react";
import { Section, Stack } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";

/**
 * Internal helper for Phase 2 placeholder pages. Phase 3 replaces
 * individual page bodies with real content — at that point this file
 * is expected to be deleted.
 *
 * Underscore prefix marks it as internal to the `pages/` folder.
 */
export interface PlaceholderPageProps {
  readonly title: string;
  readonly description: string;
  readonly children?: ReactNode;
  readonly noIndex?: boolean;
}

export const PlaceholderPage = ({
  title,
  description,
  children,
  noIndex = false,
}: PlaceholderPageProps) => (
  <>
    <Seo title={title} description={description} noIndex={noIndex} />
    <Section spacing="lg" container="md">
      <Stack gap={4} align="start">
        <h1 className="font-display text-4xl font-semibold tracking-tight text-fg sm:text-5xl">
          {title}
        </h1>
        <p className="max-w-2xl text-lg text-fg-muted">{description}</p>
        {children}
      </Stack>
    </Section>
  </>
);
