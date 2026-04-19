import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";

/**
 * CtaBanner — full-width closer used at the bottom of most pages.
 * Single source of call-to-action copy (i18n `cta.banner.*`) so the
 * messaging stays consistent across Home / Features / Pricing.
 */
export interface CtaBannerProps {
  readonly primaryTo?: string;
  readonly secondaryTo?: string;
}

export const CtaBanner = ({
  primaryTo = "/contact",
  secondaryTo = "/features",
}: CtaBannerProps) => {
  const { t } = useTranslation();

  return (
    <Section spacing="lg" container="md" aria-labelledby="cta-title">
      <div className="relative overflow-hidden rounded-2xl border border-primary/30 bg-gradient-to-br from-primary/15 via-bg-subtle to-bg px-6 py-10 sm:px-10 sm:py-14">
        <Stack gap={4} align="start">
          <h2
            id="cta-title"
            className="font-display text-3xl font-semibold tracking-tight text-fg sm:text-4xl"
          >
            {t("cta.banner.title")}
          </h2>
          <p className="max-w-xl text-lg text-fg-muted">
            {t("cta.banner.subtitle")}
          </p>
          <Stack direction="row" gap={3} wrap>
            <RRLink
              to={primaryTo}
              className={cn(buttonStyles({ variant: "primary", size: "lg" }))}
            >
              {t("cta.banner.primary")}
            </RRLink>
            <RRLink
              to={secondaryTo}
              className={cn(buttonStyles({ variant: "secondary", size: "lg" }))}
            >
              {t("cta.banner.secondary")}
            </RRLink>
          </Stack>
        </Stack>
      </div>
    </Section>
  );
};
