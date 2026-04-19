import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Section, Stack, buttonStyles, cn } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";
import { SectionHeader } from "../../components/SectionHeader.tsx";
import { pricingTiers, type PricingTier } from "../../content/pricing.ts";

/**
 * PricingMatrix — three-tier card layout. Highlighted tier scales up
 * on desktop and shows a "popular" ribbon; all copy is i18n-driven,
 * so localised price units (zł / $) live in the translation files.
 *
 * Card content is generated from `PricingTier.featureCount`, so adding
 * a feature row means incrementing the count + adding the matching
 * `pricing.tiers.<id>.features.<n>` key.
 */
export const PricingMatrix = () => {
  const { t } = useTranslation();

  return (
    <Section spacing="lg" container="xl" aria-labelledby="pricing-title">
      <Stack gap={10} align="start">
        <SectionHeader
          eyebrow={t("nav.pricing")}
          title={<span id="pricing-title">{t("pricing.title")}</span>}
          subtitle={t("pricing.subtitle")}
          align="center"
        />
        <ul className="m-0 grid w-full list-none grid-cols-1 gap-6 p-0 lg:grid-cols-3 lg:items-stretch">
          {pricingTiers.map((tier) => (
            <li key={tier.id} className="flex">
              <PricingCard tier={tier} />
            </li>
          ))}
        </ul>
      </Stack>
    </Section>
  );
};

interface PricingCardProps {
  readonly tier: PricingTier;
}

const PricingCard = ({ tier }: PricingCardProps) => {
  const { t } = useTranslation();
  const featureIds = Array.from({ length: tier.featureCount }, (_, i) =>
    (i + 1).toString(),
  );

  return (
    <article
      className={cn(
        "relative flex w-full flex-col gap-6 rounded-2xl border p-6",
        tier.highlight
          ? "border-primary bg-primary/5 shadow-lg"
          : "border-border bg-bg-subtle/60",
      )}
      aria-labelledby={`pricing-${tier.id}-name`}
    >
      {tier.highlight === true && (
        <span className="absolute -top-3 right-6 rounded-full bg-primary px-3 py-1 text-xs font-semibold text-primary-fg shadow-sm">
          {t("pricing.popular")}
        </span>
      )}
      <header className="flex flex-col gap-2">
        <h3
          id={`pricing-${tier.id}-name`}
          className="font-display text-xl font-semibold text-fg"
        >
          {t(`pricing.tiers.${tier.id}.name`)}
        </h3>
        <div className="flex items-baseline gap-2">
          <span className="font-display text-4xl font-semibold text-fg">
            {t(`pricing.tiers.${tier.id}.price`)}
          </span>
          <span className="text-sm text-fg-muted">
            {t(`pricing.tiers.${tier.id}.period`)}
          </span>
        </div>
        <p className="text-sm text-fg-muted">
          {t(`pricing.tiers.${tier.id}.desc`)}
        </p>
      </header>
      <ul className="m-0 flex flex-1 list-none flex-col gap-3 p-0 text-sm">
        {featureIds.map((id) => (
          <li key={id} className="flex items-start gap-2 text-fg">
            <Icon name="check" size={18} className="mt-0.5 text-primary" />
            <span>{t(`pricing.tiers.${tier.id}.features.${id}`)}</span>
          </li>
        ))}
      </ul>
      <RRLink
        to={tier.ctaTo}
        className={cn(
          buttonStyles({ variant: tier.ctaVariant, size: "md", fullWidth: true }),
        )}
      >
        {t(`pricing.tiers.${tier.id}.cta`)}
      </RRLink>
    </article>
  );
};
