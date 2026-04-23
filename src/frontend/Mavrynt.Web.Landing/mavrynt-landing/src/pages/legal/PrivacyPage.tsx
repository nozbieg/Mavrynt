import { useTranslation } from "react-i18next";
import { PlaceholderPage } from "../_PlaceholderPage.tsx";

const PrivacyPage = () => {
  const { t } = useTranslation();
  return (
    <PlaceholderPage
      title={t("legal.privacy.title")}
      description={t("legal.privacy.subtitle")}
    />
  );
};

export default PrivacyPage;
