import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Navbar, buttonStyles, cn } from "@mavrynt/ui";
import { primaryNav } from "../../content/navigation.ts";
import { RouterLink } from "../../lib/router/RouterLink.tsx";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";
import { ThemeToggle } from "./ThemeToggle.tsx";

/**
 * MarketingNav — composes the shared `<Navbar>` shell with
 * router-aware links, theme + language controls, and a primary CTA.
 *
 * Kept small on purpose: all layout concerns live in `@mavrynt/ui`,
 * this file just slots the marketing-specific routing and copy.
 */
export const MarketingNav = () => {
  const { t } = useTranslation();

  const links = primaryNav.map((item) => (
    <li key={item.id}>
      <RouterLink to={item.to} variant="subtle" className="text-sm font-medium">
        {t(item.labelKey)}
      </RouterLink>
    </li>
  ));

  const actions = (
    <>
      <LanguageSwitcher />
      <ThemeToggle />
      <RRLink
        to="/contact"
        className={cn(
          buttonStyles({ variant: "primary", size: "sm" }),
          "hidden sm:inline-flex",
        )}
      >
        {t("cta.getStarted")}
      </RRLink>
    </>
  );

  return <Navbar links={links} actions={actions} />;
};
