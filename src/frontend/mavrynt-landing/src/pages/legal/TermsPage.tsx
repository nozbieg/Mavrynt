import { useTranslation } from "react-i18next";
import { PlaceholderPage } from "../_PlaceholderPage.tsx";

const TermsPage = () => {
  const { t } = useTranslation();
  return (
    <PlaceholderPage
      title={t("legal.terms.title")}
      description={t("legal.terms.subtitle")}
    />
  );
};

export default TermsPage;
