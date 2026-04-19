import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";

const NotFoundPage = () => {
  const { t } = useTranslation();

  return (
    <>
      <Seo title={t("notFound.title")} description={t("notFound.subtitle")} noIndex />
      <Section spacing="lg" container="md">
        <Stack gap={4} align="start">
          <h1 className="font-display text-4xl font-semibold tracking-tight text-fg sm:text-5xl">
            404 — {t("notFound.title")}
          </h1>
          <p className="max-w-2xl text-lg text-fg-muted">{t("notFound.subtitle")}</p>
          <RRLink
            to="/"
            className={cn(buttonStyles({ variant: "primary", size: "md" }))}
          >
            {t("notFound.cta")}
          </RRLink>
        </Stack>
      </Section>
    </>
  );
};

export default NotFoundPage;
