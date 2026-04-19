import { useTranslation } from "react-i18next";
import { PlaceholderPage } from "./_PlaceholderPage.tsx";

const FeaturesPage = () => {
  const { t } = useTranslation();
  return (
    <PlaceholderPage title={t("features.title")} description={t("features.subtitle")} />
  );
};

export default FeaturesPage;
