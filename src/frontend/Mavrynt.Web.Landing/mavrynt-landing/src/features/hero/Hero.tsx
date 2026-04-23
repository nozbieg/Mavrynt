import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";

/**
 * Hero — top-of-page pitch. Anchors the whole marketing funnel, so it
 * owns its own Section (not passed in) and renders a decorative
 * background gradient using design tokens only.
 *
 * Kept free of motion libraries — CSS transitions in Tailwind are
 * enough for the Phase 3 scope.
 */
export const Hero = () => {
  const { t } = useTranslation();

  return (
    <Section
      spacing="lg"
      container="md"
      className="relative overflow-hidden"
      aria-labelledby="hero-title"
    >
      <div
        aria-hidden="true"
        className="pointer-events-none absolute inset-x-0 -top-24 -z-10 h-[480px] bg-gradient-to-b from-primary/10 via-bg to-transparent blur-3xl"
      />
      <Stack gap={6} align="start">
        <span className="inline-flex items-center gap-2 rounded-full bg-bg-muted px-3 py-1 text-xs font-medium uppercase tracking-wide text-fg-muted">
          <Icon name="sparkles" size={14} />
          {t("home.eyebrow")}
        </span>
        <h1
          id="hero-title"
          className="font-display text-5xl font-semibold tracking-tight text-fg sm:text-6xl"
        >
          {t("home.title")}
        </h1>
        <p className="max-w-2xl text-lg text-fg-muted">{t("home.subtitle")}</p>
        <Stack direction="row" gap={3} wrap>
          <RRLink
            to="/contact"
            className={cn(buttonStyles({ variant: "primary", size: "lg" }))}
          >
            {t("cta.getStarted")}
            <Icon name="arrow-right" size={18} className="ml-1" />
          </RRLink>
          <RRLink
            to="/features"
            className={cn(buttonStyles({ variant: "secondary", size: "lg" }))}
          >
            {t("cta.readDocs")}
          </RRLink>
        </Stack>
        <div className="mt-6 flex items-center gap-3 rounded-lg border border-border bg-bg-subtle/60 px-4 py-3 text-sm text-fg-muted">
          <Icon name="activity" size={18} className="text-primary" />
          <span>
            <strong className="font-semibold text-fg">
              {t("hero.stat.label")}:
            </strong>{" "}
            {t("hero.stat.value")}
          </span>
        </div>
      </Stack>
    </Section>
  );
};
