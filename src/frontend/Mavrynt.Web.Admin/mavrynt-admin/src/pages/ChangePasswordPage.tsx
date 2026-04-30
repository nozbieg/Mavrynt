import { useState, type FormEvent } from "react";
import { useNavigate } from "react-router";
import { Section, buttonStyles, cn } from "@mavrynt/ui";
import { AuthCard, PasswordField } from "@mavrynt/auth-ui";
import { Seo } from "../lib/seo/Seo.tsx";

const CHANGE_PASSWORD_URL = "/admin-api/auth/change-password";

type Status = "idle" | "submitting" | "success" | "error";

/**
 * ChangePasswordPage — shown after first login when RequiresPasswordChange is true.
 * The user must set a new password before accessing any admin functionality.
 */
const ChangePasswordPage = () => {
  const navigate = useNavigate();

  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [status, setStatus] = useState<Status>("idle");
  const [errorMessage, setErrorMessage] = useState<string | undefined>(
    undefined,
  );

  const validate = (): string | undefined => {
    if (!currentPassword) return "Current password is required.";
    if (!newPassword) return "New password is required.";
    if (newPassword.length < 8)
      return "New password must be at least 8 characters.";
    if (newPassword !== confirmPassword) return "Passwords do not match.";
    return undefined;
  };

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (status === "submitting") return;

    const validationError = validate();
    if (validationError) {
      setErrorMessage(validationError);
      return;
    }

    setErrorMessage(undefined);
    setStatus("submitting");

    void (async () => {
      try {
        const response = await fetch(CHANGE_PASSWORD_URL, {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Accept: "application/json",
          },
          body: JSON.stringify({ currentPassword, newPassword }),
          credentials: "include",
        });

        if (!response.ok) {
          const body = (await response.json().catch(() => null)) as {
            message?: string;
          } | null;
          setErrorMessage(
            body?.message ??
              "Password change failed. Please check your current password.",
          );
          setStatus("error");
          return;
        }

        setStatus("success");
        void navigate("/login", { replace: true });
      } catch {
        setErrorMessage("Network error. Please try again.");
        setStatus("error");
      }
    })();
  };

  const submitting = status === "submitting";

  return (
    <>
      <Seo
        title="Change password"
        description="You must change your initial password before continuing."
      />
      <Section spacing="lg" container="md">
        <AuthCard
          eyebrow="Mavrynt Admin"
          title="Change your password"
          subtitle="Your account requires a password change before you can continue. This is a one-time step for first login."
        >
          <form
            noValidate
            onSubmit={handleSubmit}
            className="flex w-full flex-col gap-5"
          >
            <PasswordField
              label="Current password"
              placeholder="Enter your current password"
              autoComplete="current-password"
              required
              value={currentPassword}
              onChange={setCurrentPassword}
              showLabel="Show password"
              hideLabel="Hide password"
            />

            <PasswordField
              label="New password"
              placeholder="Enter your new password (min. 8 characters)"
              autoComplete="new-password"
              required
              value={newPassword}
              onChange={setNewPassword}
              showLabel="Show password"
              hideLabel="Hide password"
            />

            <PasswordField
              label="Confirm new password"
              placeholder="Repeat your new password"
              autoComplete="new-password"
              required
              value={confirmPassword}
              onChange={setConfirmPassword}
              showLabel="Show password"
              hideLabel="Hide password"
            />

            {(status === "error" || errorMessage) && (
              <div
                role="alert"
                className="flex items-start gap-3 rounded-lg border border-danger-500/30 bg-danger-500/10 p-4 text-sm text-fg"
              >
                <span aria-hidden="true" className="mt-0.5 text-danger-500">
                  {"!"}
                </span>
                <span>{errorMessage}</span>
              </div>
            )}

            {status === "success" && (
              <div
                role="status"
                className="rounded-lg border border-success-500/30 bg-success-500/10 p-4 text-sm text-fg"
              >
                Password changed successfully. Please sign in again.
              </div>
            )}

            <button
              type="submit"
              disabled={submitting}
              className={cn(
                buttonStyles({ variant: "primary", size: "md", fullWidth: true }),
              )}
            >
              {submitting ? "Changing password…" : "Change password"}
            </button>
          </form>
        </AuthCard>
      </Section>
    </>
  );
};

export default ChangePasswordPage;
