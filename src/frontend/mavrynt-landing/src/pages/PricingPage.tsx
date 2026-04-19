import { useTranslation } from "react-i18next";
import { PlaceholderPage } from "./_PlaceholderPage.tsx";

const PricingPage = () => {
  const { t } = useTranslation();
  return (
    <PlaceholderPage title={t("pricing.title")} description={t("pricing.subtitle")} />
  );
};

export default PricingPage;
