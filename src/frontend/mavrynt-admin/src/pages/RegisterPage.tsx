import { useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import { Section } from "@mavrynt/ui";
import { AuthCard, RegisterForm, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui";
import { RouterLink } from "../lib/router/RouterLink.tsx";
import { Seo } from "../lib/seo/Seo.tsx";
import {
  featureFlags,
  ADMIN_REGISTER_ENABLED_FLAG,
} from "../lib/feature-flags/index.ts";

/**
 * Admin RegisterPage.
 *
 * Self-registration on the admin SPA is **invite-only by default**
 * (Phase 1 decision). The page is still mounted so the route exists
 * for parity with `mavrynt-web`, but when the feature flag is off we
 * pass `disabled` to `<RegisterForm>`, which swaps the inputs for a
 * friendly "registration disabled" banner.
 *
 * Admins who need to create accounts should use the user management
 * tools inside the signed-in area (shipping in a later phase).
 */
const RegisterPage = () => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const { t: tCommon } = useTranslation();
  const navigate = useNavigate();
  const registerEnabled = featureFlags.isEnabled(ADMIN_REGISTER_ENABLED_FLAG);

  return (
    <>
      <Seo
        title={t("register.title") ?? tCommon("nav.register")}
        description={t("register.subtitle")}
      />
      <Section spacing="lg" container="md">
        <AuthCard
          eyebrow={tCommon("app.name")}
          title={t("register.title")}
          subtitle={t("register.subtitle")}
          footer={
            <>
              <span>
                {t("register.links.haveAccount")}{" "}
                <RouterLink to="/login" variant="inline">
                  {t("register.links.login")}
                </RouterLink>
              </span>
              <RouterLink to="/" variant="muted">
                {tCommon("nav.home")}
              </RouterLink>
            </>
          }
        >
          <RegisterForm
            source="admin:register"
            disabled={!registerEnabled}
            onSuccess={() => {
              void navigate("/login", { replace: true });
            }}
          />
        </AuthCard>
      </Section>
    </>
  );
};

export default RegisterPage;
