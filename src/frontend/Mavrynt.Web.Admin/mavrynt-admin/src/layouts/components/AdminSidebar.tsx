import { NavLink } from "react-router";
import { cn } from "@mavrynt/ui";

const NAV_ITEMS = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/users", label: "Users" },
  { to: "/feature-flags", label: "Feature Flags" },
  { to: "/smtp-settings", label: "SMTP Settings" },
  { to: "/settings", label: "Settings" },
] as const;

export const AdminSidebar = () => (
  <nav className="flex flex-col gap-1 p-3" aria-label="Admin navigation">
    {NAV_ITEMS.map(({ to, label }) => (
      <NavLink
        key={to}
        to={to}
        className={({ isActive }) =>
          cn(
            "rounded-md px-3 py-2 text-sm font-medium transition-colors",
            isActive
              ? "bg-bg-subtle text-fg"
              : "text-fg-muted hover:bg-bg-subtle hover:text-fg",
          )
        }
      >
        {label}
      </NavLink>
    ))}
  </nav>
);
