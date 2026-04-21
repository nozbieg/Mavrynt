import { useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import { Section } from "@mavrynt/ui";
import { AuthCard, RegisterForm, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui";
import { RouterLink } from "../lib/router/RouterLink.tsx";
import { Seo } from "../lib/seo/Seo.tsx";

/**
 * RegisterPage — web-facing self-registration surface.
 *
 * Registration is enabled on web (disabled on admin per Phase 1
 * decision). On success we redirect to `/login` — an email verification
 * step will slot in between here and auto-login once the Users module
 * lands.
 */
const RegisterPage = () => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const { t: tCommon } = useTranslation();
  const navigate = useNavigate();

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
            source="web:register"
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
