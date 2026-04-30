import { useTranslation } from "react-i18next";
import { Navbar, buttonStyles, cn } from "@mavrynt/ui";
import { resolveAppUrls } from "@mavrynt/config/app-urls";
import { RouterLink } from "../../lib/router/RouterLink.tsx";
import {
  featureFlags,
  ADMIN_REGISTER_ENABLED_FLAG,
} from "../../lib/feature-flags/index.ts";
import { useAdminAuth } from "../../lib/auth/useAdminAuth.ts";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";
import { ThemeToggle } from "./ThemeToggle.tsx";

const appUrls = resolveAppUrls();

export const AdminNav = () => {
  const { t } = useTranslation();
  const registerEnabled = featureFlags.isEnabled(ADMIN_REGISTER_ENABLED_FLAG);
  const { isAuthenticated, user, logout } = useAdminAuth();
  const identity = user?.name ?? user?.email;

  const links = (
    <>
      <li>
        <RouterLink to="/" variant="subtle" className="text-sm font-medium">
          {t("nav.home")}
        </RouterLink>
      </li>

      {!isAuthenticated && (
        <li>
          <RouterLink to="/login" variant="subtle" className="text-sm font-medium">
            {t("nav.login")}
          </RouterLink>
        </li>
      )}

      {registerEnabled && !isAuthenticated && (
        <li>
          <RouterLink to="/register" variant="subtle" className="text-sm font-medium">
            {t("nav.register")}
          </RouterLink>
        </li>
      )}

      {isAuthenticated && (
        <li>
          <RouterLink to="/dashboard" variant="subtle" className="text-sm font-medium">
            Dashboard
          </RouterLink>
        </li>
      )}
    </>
  );

  const actions = (
    <>
      <LanguageSwitcher />
      <ThemeToggle />

      {isAuthenticated && (
        <span className="text-xs text-fg-muted" aria-label="Logged in user">
          {identity}
          {user?.roles?.[0] ? ` (${user.roles[0]})` : ""}
        </span>
      )}

      <a
        href={appUrls.web}
        className={cn(buttonStyles({ variant: "ghost", size: "sm" }), "hidden md:inline-flex")}
      >
        {t("nav.backToWeb")}
      </a>

      <a
        href={appUrls.landing}
        className={cn(buttonStyles({ variant: "ghost", size: "sm" }), "hidden sm:inline-flex")}
      >
        {t("nav.backToSite")}
      </a>

      {isAuthenticated ? (
        <button
          type="button"
          aria-label="Logout"
          onClick={() => void logout()}
          className={cn(buttonStyles({ variant: "primary", size: "sm" }), "hidden sm:inline-flex")}
        >
          Logout
        </button>
      ) : (
        <RouterLink
          to="/login"
          variant="inline"
          className={cn(buttonStyles({ variant: "primary", size: "sm" }), "hidden sm:inline-flex")}
        >
          {t("nav.login")}
        </RouterLink>
      )}
    </>
  );

  return <Navbar links={links} actions={actions} ariaLabel={t("a11y.nav")} />;
};
