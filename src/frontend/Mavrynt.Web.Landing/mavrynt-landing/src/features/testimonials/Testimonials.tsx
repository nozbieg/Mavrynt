import { useTranslation } from "react-i18next";
import { Section, Stack } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";
import { SectionHeader } from "../../components/SectionHeader.tsx";
import { testimonials } from "../../content/testimonials.ts";

/**
 * Testimonials — static 3-up quote grid. No carousel by design: Phase
 * 3 stays keyboard-accessible with zero extra JS. Carousel UX (if
 * needed) is a Phase 4+ concern.
 */
export const Testimonials = () => {
  const { t } = useTranslation();

  return (
    <Section
      tone="subtle"
      spacing="lg"
      container="xl"
      aria-labelledby="testimonials-title"
    >
      <Stack gap={10} align="start">
        <SectionHeader
          title={<span id="testimonials-title">{t("testimonials.title")}</span>}
          subtitle={t("testimonials.subtitle")}
        />
        <ul className="m-0 grid w-full list-none grid-cols-1 gap-6 p-0 md:grid-cols-3">
          {testimonials.map((item) => (
            <li
              key={item.id}
              className="flex h-full flex-col gap-4 rounded-xl border border-border bg-bg p-6"
            >
              <Icon name="quote" size={22} className="text-primary" />
              <blockquote className="flex-1 text-base text-fg">
                <p>{t(`testimonials.items.${item.id}.quote`)}</p>
              </blockquote>
              <footer className="flex items-center gap-3 border-t border-border pt-4">
                <span
                  aria-hidden="true"
                  className="inline-flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 text-sm font-semibold text-primary"
                >
                  {item.initials}
                </span>
                <div className="flex flex-col">
                  <cite className="text-sm font-semibold not-italic text-fg">
                    {t(`testimonials.items.${item.id}.author`)}
                  </cite>
                  <span className="text-xs text-fg-muted">
                    {t(`testimonials.items.${item.id}.role`)}
                  </span>
                </div>
              </footer>
            </li>
          ))}
        </ul>
      </Stack>
    </Section>
  );
};
