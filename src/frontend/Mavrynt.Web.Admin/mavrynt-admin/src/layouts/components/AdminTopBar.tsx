import { useNavigate } from "react-router";
import { cn, buttonStyles } from "@mavrynt/ui";
import { useAdminSession } from "../../lib/auth/AdminSessionProvider.tsx";
import { ThemeToggle } from "./ThemeToggle.tsx";
import { LanguageSwitcher } from "./LanguageSwitcher.tsx";

export const AdminTopBar = () => {
  const { session, logout } = useAdminSession();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    void navigate("/login", { replace: true });
  };

  return (
    <header className="flex h-14 shrink-0 items-center justify-between border-b border-border bg-bg px-6">
      <span className="text-sm font-semibold text-fg">Mavrynt Admin</span>
      <div className="flex items-center gap-2">
        <LanguageSwitcher />
        <ThemeToggle />
        {session ? (
          <>
            <span className="hidden text-sm text-fg-muted sm:block">
              {session.user.name ?? session.user.email}
              {session.user.roles?.[0] ? (
                <span className="ml-1 text-xs opacity-60">
                  ({session.user.roles[0]})
                </span>
              ) : null}
            </span>
            <button
              type="button"
              onClick={handleLogout}
              className={cn(buttonStyles({ variant: "ghost", size: "sm" }))}
            >
              Logout
            </button>
          </>
        ) : null}
      </div>
    </header>
  );
};
