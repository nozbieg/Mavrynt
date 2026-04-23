import { useTranslation } from "react-i18next";
import { Seo } from "../lib/seo/Seo.tsx";
import { PricingMatrix } from "../features/pricing-matrix/PricingMatrix.tsx";
import { Faq } from "../features/faq/Faq.tsx";
import { CtaBanner } from "../features/cta-banner/CtaBanner.tsx";

const PricingPage = () => {
  const { t } = useTranslation();
  return (
    <>
      <Seo title={t("pricing.title")} description={t("pricing.subtitle")} />
      <PricingMatrix />
      <Faq />
      <CtaBanner />
    </>
  );
};

export default PricingPage;
