import { useTranslation } from "react-i18next";
import { PlaceholderPage } from "./_PlaceholderPage.tsx";

const ContactPage = () => {
  const { t } = useTranslation();
  return (
    <PlaceholderPage title={t("contact.title")} description={t("contact.subtitle")} />
  );
};

export default ContactPage;
