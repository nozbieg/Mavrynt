import { useTranslation } from "react-i18next";
import { Seo } from "../lib/seo/Seo.tsx";
import { FeatureGrid } from "../features/feature-grid/FeatureGrid.tsx";
import { HowItWorks } from "../features/how-it-works/HowItWorks.tsx";
import { CtaBanner } from "../features/cta-banner/CtaBanner.tsx";

const FeaturesPage = () => {
  const { t } = useTranslation();
  return (
    <>
      <Seo title={t("features.title")} description={t("features.subtitle")} />
      <FeatureGrid />
      <HowItWorks />
      <CtaBanner />
    </>
  );
};

export default FeaturesPage;
