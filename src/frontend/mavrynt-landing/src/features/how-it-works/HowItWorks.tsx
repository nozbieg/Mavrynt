import { useTranslation } from "react-i18next";
import { Section, Stack } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";
import { SectionHeader } from "../../components/SectionHeader.tsx";
import { howSteps } from "../../content/howItWorks.ts";

/**
 * HowItWorks — 3-step visual walkthrough. Uses ordered list semantics
 * so screen readers announce the step numbers the same way the visual
 * badge does.
 */
export const HowItWorks = () => {
  const { t } = useTranslation();

  return (
    <Section
      tone="subtle"
      spacing="lg"
      container="xl"
      aria-labelledby="how-title"
    >
      <Stack gap={10} align="start">
        <SectionHeader
          eyebrow="01 · 02 · 03"
          title={<span id="how-title">{t("how.title")}</span>}
          subtitle={t("how.subtitle")}
        />
        <ol className="m-0 grid w-full list-none grid-cols-1 gap-6 p-0 md:grid-cols-3">
          {howSteps.map((step, idx) => (
            <li
              key={step.id}
              className="relative flex flex-col gap-3 rounded-xl border border-border bg-bg p-6"
            >
              <div className="flex items-center gap-3">
                <span
                  aria-hidden="true"
                  className="inline-flex h-8 w-8 items-center justify-center rounded-full bg-primary text-sm font-semibold text-primary-fg"
                >
                  {idx + 1}
                </span>
                <Icon name={step.icon} size={20} className="text-primary" />
              </div>
              <h3 className="font-display text-lg font-semibold text-fg">
                {t(`how.steps.${step.id}.title`)}
              </h3>
              <p className="text-sm text-fg-muted">
                {t(`how.steps.${step.id}.desc`)}
              </p>
            </li>
          ))}
        </ol>
      </Stack>
    </Section>
  );
};
