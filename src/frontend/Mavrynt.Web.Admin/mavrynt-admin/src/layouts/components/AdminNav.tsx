import { useNavigate } from "react-router";
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
import { useAdminSession } from "../../lib/auth/AdminSessionProvider.tsx";

const appUrls = resolveAppUrls();

export const AdminNav = () => {
  const { t } = useTranslation();
  const { session, logout } = useAdminSession();
  const navigate = useNavigate();
  const registerEnabled = featureFlags.isEnabled(ADMIN_REGISTER_ENABLED_FLAG);

  const handleLogout = () => {
    logout();
    void navigate("/login", { replace: true });
  };

  const links = (
    <>
      <li>
        <RouterLink to="/" variant="subtle" className="text-sm font-medium">
          {t("nav.home")}
        </RouterLink>
      </li>
      {session ? (
        <li>
          <RouterLink
            to="/dashboard"
            variant="subtle"
            className="text-sm font-medium"
          >
            Dashboard
          </RouterLink>
        </li>
      ) : (
        <>
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
      )}
    </>
  );

  const actions = (
    <>
      <LanguageSwitcher />
      <ThemeToggle />
      {session ? (
        <>
          <span className="hidden text-sm text-fg-muted sm:block">
            {session.user.name ?? session.user.email}
          </span>
          <button
            type="button"
            onClick={handleLogout}
            className={cn(
              buttonStyles({ variant: "ghost", size: "sm" }),
              "hidden sm:inline-flex",
            )}
          >
            Logout
          </button>
        </>
      ) : (
        <>
          <a
            href={appUrls.web}
            className={cn(
              buttonStyles({ variant: "ghost", size: "sm" }),
              "hidden md:inline-flex",
            )}
          >
            {t("nav.backToWeb")}
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
      )}
    </>
  );

  return <Navbar links={links} actions={actions} ariaLabel={t("a11y.nav")} />;
};
