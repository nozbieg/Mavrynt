import { useTranslation } from "react-i18next";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { RouterLink } from "../lib/router/RouterLink.tsx";
import { Seo } from "../lib/seo/Seo.tsx";

/**
 * HomePage — "you are signed out" landing for the Web SPA.
 *
 * Keeps content minimal on purpose: this page exists so unauthenticated
 * users hitting `/` have a clear next step. Post-login dashboards will
 * live here (or redirect away) once the Users module lands.
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
          <p className="max-w-2xl text-lg text-fg-muted">{t("home.subtitle")}</p>
          <Stack direction="row" gap={3} align="center" wrap>
            <RouterLink
              to="/login"
              variant="inline"
              className={cn(buttonStyles({ variant: "primary", size: "md" }))}
            >
              {t("home.primary")}
            </RouterLink>
            <RouterLink
              to="/register"
              variant="inline"
              className={cn(buttonStyles({ variant: "secondary", size: "md" }))}
            >
              {t("home.secondary")}
            </RouterLink>
          </Stack>
        </Stack>
      </Section>
    </>
  );
};

export default HomePage;
