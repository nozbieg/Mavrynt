import { useTranslation } from "react-i18next";
import { Section, Stack } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";
import { SectionHeader } from "../../components/SectionHeader.tsx";
import { features } from "../../content/features.ts";

/**
 * FeatureGrid — capability overview. Descriptor-driven (see
 * `content/features.ts`) so adding a feature means adding one entry
 * there + the matching i18n key, nothing more.
 *
 * Renderable at two densities: `variant="compact"` (homepage teaser)
 * and default (dedicated features page).
 */
export interface FeatureGridProps {
  readonly variant?: "default" | "compact";
}

export const FeatureGrid = ({ variant = "default" }: FeatureGridProps) => {
  const { t } = useTranslation();

  return (
    <Section spacing="lg" container="xl" aria-labelledby="features-title">
      <Stack gap={10} align="start">
        <SectionHeader
          eyebrow={t("nav.features")}
          title={t("features.title")}
          subtitle={variant === "compact" ? undefined : t("features.subtitle")}
        />
        <ul
          id="features-title"
          className="m-0 grid w-full list-none grid-cols-1 gap-4 p-0 sm:grid-cols-2 lg:grid-cols-3"
        >
          {features.map((item) => (
            <li
              key={item.id}
              className="flex flex-col gap-3 rounded-xl border border-border bg-bg-subtle/60 p-6 transition-colors hover:border-primary/60"
            >
              <span className="inline-flex h-10 w-10 items-center justify-center rounded-lg bg-primary/10 text-primary">
                <Icon name={item.icon} size={20} />
              </span>
              <h3 className="font-display text-lg font-semibold text-fg">
                {t(`features.items.${item.id}.title`)}
              </h3>
              <p className="text-sm text-fg-muted">
                {t(`features.items.${item.id}.desc`)}
              </p>
            </li>
          ))}
        </ul>
      </Stack>
    </Section>
  );
};
