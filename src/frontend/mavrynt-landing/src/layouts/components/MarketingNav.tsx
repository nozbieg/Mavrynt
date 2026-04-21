import { useTranslation } from "react-i18next";
import { Link as RRLink } from "react-router";
import { Navbar, buttonStyles, cn } from "@mavrynt/ui";
import { resolveAppUrls } from "@mavrynt/config/app-urls";
import { primaryNav } from "../../content/navigation.ts";
import { RouterLink } from "../../lib/router/RouterLink.tsx";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";
import { ThemeToggle } from "./ThemeToggle.tsx";

/**
 * Cross-app URLs — resolved from `@mavrynt/config`. Lets the marketing
 * site link straight to the Web SPA's `/login` and `/register` routes
 * without duplicating host/port config.
 */
const appUrls = resolveAppUrls();

/**
 * MarketingNav — composes the shared `<Navbar>` shell with
 * router-aware links, theme + language controls, and auth CTAs that
 * hop the visitor over to `mavrynt-web` (the auth-facing SPA per
 * ADR-010 — landing stays backend-independent).
 *
 * Kept small on purpose: all layout concerns live in `@mavrynt/ui`;
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
      <a
        href={`${appUrls.web}/login`}
        className={cn(
          buttonStyles({ variant: "ghost", size: "sm" }),
          "hidden sm:inline-flex",
        )}
      >
        {t("nav.login")}
      </a>
      <a
        href={`${appUrls.web}/register`}
        className={cn(
          buttonStyles({ variant: "primary", size: "sm" }),
          "hidden sm:inline-flex",
        )}
      >
        {t("nav.register")}
      </a>
      <RRLink
        to="/contact"
        className={cn(
          buttonStyles({ variant: "secondary", size: "sm" }),
          "hidden md:inline-flex",
        )}
      >
        {t("cta.getStarted")}
      </RRLink>
    </>
  );

  return <Navbar links={links} actions={actions} ariaLabel={t("a11y.nav")} />;
};
