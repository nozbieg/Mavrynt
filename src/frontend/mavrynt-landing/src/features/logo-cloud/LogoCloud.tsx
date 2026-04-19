import { useTranslation } from "react-i18next";
import { Section } from "@mavrynt/ui";
import { logos } from "../../content/logos.ts";

/**
 * LogoCloud — social-proof strip. Uses text wordmarks while real
 * partner assets aren't available; swap to SVGs later without
 * changing the layout.
 */
export const LogoCloud = () => {
  const { t } = useTranslation();

  return (
    <Section tone="subtle" spacing="sm" container="xl" aria-labelledby="logos-title">
      <p
        id="logos-title"
        className="mb-6 text-center text-xs font-semibold uppercase tracking-[0.2em] text-fg-muted"
      >
        {t("logos.title")}
      </p>
      <ul className="m-0 grid list-none grid-cols-2 gap-x-8 gap-y-6 p-0 sm:grid-cols-3 md:grid-cols-6">
        {logos.map((logo) => (
          <li
            key={logo.id}
            className="flex items-center justify-center text-base font-semibold tracking-tight text-fg-muted opacity-80 transition-opacity hover:opacity-100"
          >
            {logo.label}
          </li>
        ))}
      </ul>
    </Section>
  );
};
