import { useTranslation } from "react-i18next";
import { Navbar, buttonStyles, cn } from "@mavrynt/ui";
import { resolveAppUrls } from "@mavrynt/config/app-urls";
import { RouterLink } from "../../lib/router/RouterLink.tsx";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";
import { ThemeToggle } from "./ThemeToggle.tsx";

/**
 * Cross-app URLs — single source of truth in `@mavrynt/config`.
 * Overridable at build time via `VITE_APP_URL_*` env vars; dev defaults
 * line up with the per-app Vite ports.
 */
const appUrls = resolveAppUrls();

/**
 * AppNav — top navigation for the authenticated Web SPA.
 *
 * Composes the shared `<Navbar>` shell with:
 *  - brand / home link
 *  - Login/Register router links
 *  - locale + theme controls
 *  - a "Back to mavrynt.com" escape hatch (always safe — landing is
 *    backend-independent per ADR-010)
 *  - a primary "Sign in" CTA (always visible — Web is auth-facing)
 */
export const AppNav = () => {
  const { t } = useTranslation();

  const links = (
    <>
      <li>
        <RouterLink to="/" variant="subtle" className="text-sm font-medium">
          {t("nav.home")}
        </RouterLink>
      </li>
      <li>
        <RouterLink
          to="/login"
          variant="subtle"
          className="text-sm font-medium"
        >
          {t("nav.login")}
        </RouterLink>
      </li>
      <li>
        <RouterLink
          to="/register"
          variant="subtle"
          className="text-sm font-medium"
        >
          {t("nav.register")}
        </RouterLink>
      </li>
    </>
  );

  const actions = (
    <>
      <LanguageSwitcher />
      <ThemeToggle />
      <a
        href={appUrls.landing}
        className={cn(
          buttonStyles({ variant: "ghost", size: "sm" }),
          "hidden sm:inline-flex",
        )}
      >
        {t("nav.backToSite")}
      </a>
      <RouterLink
        to="/login"
        variant="inline"
        className={cn(
          buttonStyles({ variant: "primary", size: "sm" }),
          "hidden sm:inline-flex",
        )}
      >
        {t("nav.login")}
      </RouterLink>
    </>
  );

  return <Navbar links={links} actions={actions} ariaLabel={t("a11y.nav")} />;
};
