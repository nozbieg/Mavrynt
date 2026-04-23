import { useTranslation } from "react-i18next";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { RouterLink } from "../lib/router/RouterLink.tsx";
import { Seo } from "../lib/seo/Seo.tsx";

/**
 * Admin HomePage — "you are signed out" landing.
 *
 * Deliberately sparse: operators shouldn't see marketing copy. Once
 * auth state wiring lands (Phase 4+), this route redirects signed-in
 * users to the operator dashboard.
 */
const HomePage = () => {
  const { t } = useTranslation();

  return (
    <>
      <Seo title={t("home.title")} description={t("home.subtitle")} />
      <Section spacing="lg" container="md">
        <Stack gap={6} align="start">
          <h1 className="font-display text-4xl font-semibold tracking-tight text-fg sm:text-5xl">
            {t("home.title")}
          </h1>
          <p className="max-w-2xl text-lg text-fg-muted">
            {t("home.subtitle")}
          </p>
          <RouterLink
            to="/login"
            variant="inline"
            className={cn(buttonStyles({ variant: "primary", size: "md" }))}
          >
            {t("home.primary")}
          </RouterLink>
        </Stack>
      </Section>
    </>
  );
};

export default HomePage;
