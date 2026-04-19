import { useTranslation } from "react-i18next";
import { Section, Stack } from "@mavrynt/ui";
import { Icon } from "../../components/Icon.tsx";
import { SectionHeader } from "../../components/SectionHeader.tsx";
import { faqIds } from "../../content/faq.ts";

/**
 * FAQ — accordion using native `<details>`/`<summary>`.
 *
 * Why native: zero-JS, keyboard-accessible by default, screen-readers
 * already announce expanded/collapsed state, and Tailwind can style
 * both states with `open:` variants. A custom ARIA accordion only
 * makes sense if product needs controlled single-open behaviour —
 * Phase 3 does not.
 */
export const Faq = () => {
  const { t } = useTranslation();

  return (
    <Section spacing="lg" container="md" aria-labelledby="faq-title">
      <Stack gap={8} align="start">
        <SectionHeader
          title={<span id="faq-title">{t("faq.title")}</span>}
          subtitle={t("faq.subtitle")}
        />
        <div className="flex w-full flex-col divide-y divide-border overflow-hidden rounded-xl border border-border bg-bg">
          {faqIds.map((id) => (
            <details
              key={id}
              className="group p-6 open:bg-bg-subtle/60"
              name="faq"
            >
              <summary className="flex cursor-pointer list-none items-center justify-between gap-4 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-focus-ring focus-visible:ring-offset-2 focus-visible:ring-offset-bg">
                {/*
                 * Wrapping the question in <h3> inside <summary> gives
                 * screen-reader users the ability to navigate between
                 * questions via the headings list, without changing
                 * keyboard activation (summary remains the toggle).
                 */}
                <h3 className="m-0 text-base font-medium text-fg">
                  {t(`faq.items.${id}.q`)}
                </h3>
                <Icon
                  name="chevron-down"
                  size={18}
                  className="shrink-0 text-fg-muted transition-transform duration-200 group-open:rotate-180 motion-reduce:transition-none"
                />
              </summary>
              <p className="mt-3 text-sm leading-relaxed text-fg-muted">
                {t(`faq.items.${id}.a`)}
              </p>
            </details>
          ))}
        </div>
      </Stack>
    </Section>
  );
};
