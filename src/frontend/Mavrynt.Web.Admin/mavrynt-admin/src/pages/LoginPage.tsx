import { useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import { Section } from "@mavrynt/ui";
import { AuthCard, LoginForm, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui";
import { RouterLink } from "../lib/router/RouterLink.tsx";
import { Seo } from "../lib/seo/Seo.tsx";
import {
  featureFlags,
  ADMIN_REGISTER_ENABLED_FLAG,
} from "../lib/feature-flags/index.ts";

/**
 * Admin LoginPage — identical presentation to `mavrynt-web`'s login
 * page (same `<AuthCard>` + `<LoginForm>` from `@mavrynt/auth-ui`).
 * The only differences are:
 *   - the AuthService adapter is the admin one (stamped with
 *     `roles: ["admin"]` in the console adapter), resolved by
 *     `<AuthServiceContext.Provider>` in `Providers.tsx`.
 *   - the "register" secondary link is hidden when the feature flag is
 *     off, so operators aren't led to a disabled page.
 */
const LoginPage = () => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const { t: tCommon } = useTranslation();
  const navigate = useNavigate();
  const registerEnabled = featureFlags.isEnabled(ADMIN_REGISTER_ENABLED_FLAG);

  return (
    <>
      <Seo
        title={t("login.title") ?? tCommon("nav.login")}
        description={t("login.subtitle")}
      />
      <Section spacing="lg" container="md">
        <AuthCard
          eyebrow={tCommon("app.name")}
          title={t("login.title")}
          subtitle={t("login.subtitle")}
          footer={
            <>
              {registerEnabled && (
                <span>
                  {t("login.links.noAccount")}{" "}
                  <RouterLink to="/register" variant="inline">
                    {t("login.links.register")}
                  </RouterLink>
                </span>
              )}
              <RouterLink to="/" variant="muted">
                {tCommon("nav.home")}
              </RouterLink>
            </>
          }
        >
          <LoginForm
            source="admin:login"
            onSuccess={(session) => {
              if (session.requiresPasswordChange) {
                void navigate("/change-password", { replace: true });
              } else {
                void navigate("/dashboard", { replace: true });
              }
            }}
          />
        </AuthCard>
      </Section>
    </>
  );
};

export default LoginPage;
