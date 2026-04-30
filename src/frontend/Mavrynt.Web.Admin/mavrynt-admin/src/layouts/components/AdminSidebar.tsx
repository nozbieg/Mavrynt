import { NavLink } from "react-router";

const links = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/feature-flags", label: "Feature Flags" },
  { to: "/smtp-settings", label: "SMTP Settings" },
  { to: "/users", label: "Users" },
];

export const AdminSidebar = () => (
  <aside className="w-full shrink-0 md:w-64">
    <nav className="flex gap-2 rounded-lg border bg-surface p-3 md:flex-col">
      {links.map((link) => (
        <NavLink
          key={link.to}
          to={link.to}
          className={({ isActive }) =>
            `rounded px-3 py-2 ${isActive ? "bg-primary text-white" : "bg-transparent"}`
          }
        >
          {link.label}
        </NavLink>
      ))}
    </nav>
  </aside>
);
