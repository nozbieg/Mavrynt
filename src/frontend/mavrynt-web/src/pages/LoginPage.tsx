import { useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import { Section } from "@mavrynt/ui";
import { AuthCard, LoginForm, AUTH_I18N_NAMESPACE } from "@mavrynt/auth-ui";
import { RouterLink } from "../lib/router/RouterLink.tsx";
import { Seo } from "../lib/seo/Seo.tsx";

/**
 * LoginPage — web-facing login surface.
 *
 * Pure composition: the actual form + state machine lives in
 * `@mavrynt/auth-ui` (`<LoginForm>` + `useLoginForm`); this page just
 * arranges the card, SEO, secondary links, and post-success navigation.
 *
 * Secondary action is injected as a React Router `<RouterLink>` so the
 * shared form package stays routing-agnostic (SOLID — Dependency
 * Inversion). Admin mounts the same form with its own router wiring.
 */
const LoginPage = () => {
  const { t } = useTranslation(AUTH_I18N_NAMESPACE);
  const { t: tCommon } = useTranslation();
  const navigate = useNavigate();

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
              <span>
                {t("login.links.noAccount")}{" "}
                <RouterLink to="/register" variant="inline">
                  {t("login.links.register")}
                </RouterLink>
              </span>
              <RouterLink to="/" variant="muted">
                {tCommon("nav.home")}
              </RouterLink>
            </>
          }
        >
          <LoginForm
            source="web:login"
            onSuccess={() => {
              // Users module wiring: swap for redirect to intended URL in Phase 4.
              void navigate("/", { replace: true });
            }}
          />
        </AuthCard>
      </Section>
    </>
  );
};

export default LoginPage;
