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
import { useAdminSession } from "../lib/auth/AdminSessionProvider.tsx";

const LoginPage = () => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const { t: tCommon } = useTranslation();
  const navigate = useNavigate();
  const { syncSession } = useAdminSession();
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
              syncSession();
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
