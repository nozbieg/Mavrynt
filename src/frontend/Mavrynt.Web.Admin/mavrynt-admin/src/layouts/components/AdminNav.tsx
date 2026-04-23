import { useTranslation } from "react-i18next";
import { Navbar, buttonStyles, cn } from "@mavrynt/ui";
import { resolveAppUrls } from "@mavrynt/config/app-urls";
import { RouterLink } from "../../lib/router/RouterLink.tsx";
import {
  featureFlags,
  ADMIN_REGISTER_ENABLED_FLAG,
} from "../../lib/feature-flags/index.ts";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";
import { ThemeToggle } from "./ThemeToggle.tsx";

/**
 * Cross-app URLs — single source of truth in `@mavrynt/config`.
 * Overridable at build time via `VITE_APP_URL_*` env vars; dev defaults
 * line up with the per-app Vite ports.
 */
const appUrls = resolveAppUrls();

/**
 * AdminNav — top navigation for the internal operator console.
 *
 * The "Register" link is hidden (not just the page disabled) when
 * `admin.register.enabled` is off so operators aren't led to a
 * dead-end surface. Escape hatches point to the user web SPA and
 * marketing site via the shared URL resolver.
 */
export const AdminNav = () => {
  const { t } = useTranslation();
  const registerEnabled = featureFlags.isEnabled(ADMIN_REGISTER_ENABLED_FLAG);

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
      {registerEnabled && (
        <li>
          <RouterLink
            to="/register"
            variant="subtle"
            className="text-sm font-medium"
          >
            {t("nav.register")}
          </RouterLink>
        </li>
      )}
    </>
  );

  const actions = (
    <>
      <LanguageSwitcher />
      <ThemeToggle />
      <a
        href={appUrls.web}
        className={cn(
          buttonStyles({ variant: "ghost", size: "sm" }),
          "hidden md:inline-flex",
        )}
      >
        {t("nav.backToWeb")}
      </a>
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
