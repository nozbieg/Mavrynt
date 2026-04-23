import { useTranslation } from "react-i18next";
import { Seo } from "../lib/seo/Seo.tsx";
import { Hero } from "../features/hero/Hero.tsx";
import { LogoCloud } from "../features/logo-cloud/LogoCloud.tsx";
import { FeatureGrid } from "../features/feature-grid/FeatureGrid.tsx";
import { HowItWorks } from "../features/how-it-works/HowItWorks.tsx";
import { Testimonials } from "../features/testimonials/Testimonials.tsx";
import { CtaBanner } from "../features/cta-banner/CtaBanner.tsx";

/**
 * HomePage — marketing landing. Pure composition: every section is a
 * self-contained feature, so reordering / A-B testing / trimming
 * requires editing only this file.
 */
const HomePage = () => {
  const { t } = useTranslation();

  return (
    <>
      <Seo title={t("home.title")} description={t("home.subtitle")} />
      <Hero />
      <LogoCloud />
      <FeatureGrid variant="compact" />
      <HowItWorks />
      <Testimonials />
      <CtaBanner />
    </>
  );
};

export default HomePage;
