import { useTranslation } from "react-i18next";
import { Section, Stack } from "@mavrynt/ui";
import { Seo } from "../lib/seo/Seo.tsx";
import { SectionHeader } from "../components/SectionHeader.tsx";
import { ContactForm } from "../features/contact-form/ContactForm.tsx";
import { siteConfig } from "../content/site.ts";
import { Icon } from "../components/Icon.tsx";

/**
 * ContactPage — two-column layout on desktop, stacked on mobile.
 * Left: pitch + direct-contact fallback. Right: the form itself,
 * rendered via the `ContactForm` feature (which owns all form state).
 */
const ContactPage = () => {
  const { t } = useTranslation();

  return (
    <>
      <Seo title={t("contact.title")} description={t("contact.subtitle")} />
      <Section spacing="lg" container="xl">
        <div className="grid grid-cols-1 gap-12 lg:grid-cols-[1fr_1.2fr]">
          <Stack gap={6} align="start">
            <SectionHeader
              eyebrow={t("nav.contact")}
              title={t("contact.title")}
              subtitle={t("contact.subtitle")}
            />
            <a
              href={`mailto:${siteConfig.contactEmail}`}
              className="inline-flex items-center gap-2 text-sm font-medium text-primary hover:underline"
            >
              <Icon name="mail" size={16} />
              {siteConfig.contactEmail}
            </a>
          </Stack>
          <div className="rounded-2xl border border-border bg-bg-subtle/60 p-6 sm:p-8">
            <ContactForm source="landing:contact" />
          </div>
        </div>
      </Section>
    </>
  );
};

export default ContactPage;
