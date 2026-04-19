import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";

/**
 * HomePage — Phase 2 placeholder. Hero + CTA buttons anchor the layout
 * pipeline; Phase 3 replaces the body with real marketing sections
 * (LogoCloud, Features, HowItWorks, Pricing, Testimonials, FAQ, CTA).
 */
const HomePage = () => {
  const { t } = useTranslation();

  return (
    <>
      <Seo title={t("home.title")} description={t("home.subtitle")} />
      <Section spacing="lg" container="md">
        <Stack gap={6} align="start">
          <span className="rounded-full bg-bg-muted px-3 py-1 text-xs font-medium uppercase tracking-wide text-fg-muted">
            {t("home.eyebrow")}
          </span>
          <h1 className="font-display text-5xl font-semibold tracking-tight text-fg sm:text-6xl">
            {t("home.title")}
          </h1>
          <p className="max-w-2xl text-lg text-fg-muted">{t("home.subtitle")}</p>
          <Stack direction="row" gap={3} wrap>
            <RRLink
              to="/contact"
              className={cn(buttonStyles({ variant: "primary", size: "lg" }))}
            >
              {t("cta.getStarted")}
            </RRLink>
            <RRLink
              to="/features"
              className={cn(buttonStyles({ variant: "secondary", size: "lg" }))}
            >
              {t("cta.readDocs")}
            </RRLink>
          </Stack>
        </Stack>
      </Section>
    </>
  );
};

export default HomePage;
