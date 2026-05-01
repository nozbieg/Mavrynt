import { Outlet } from "react-router";
import { AdminTopBar } from "./components/AdminTopBar.tsx";
import { AdminSidebar } from "./components/AdminSidebar.tsx";

export const AdminShell = () => (
  <div className="flex h-screen flex-col overflow-hidden bg-bg text-fg">
    {/* WCAG 2.2 — skip navigation link */}
    <a
      href="#main"
      className="sr-only focus:not-sr-only focus:absolute focus:left-4 focus:top-4 focus:z-50 focus:rounded-md focus:bg-bg focus:px-4 focus:py-2 focus:text-sm focus:font-medium focus:text-fg focus:shadow-lg focus:ring-2 focus:ring-focus-ring"
    >
      Skip to main content
    </a>

    <AdminTopBar />

    <div className="flex flex-1 overflow-hidden">
      <aside
        className="hidden w-60 shrink-0 overflow-y-auto border-r border-border md:flex md:flex-col"
        aria-label="Admin navigation"
      >
        <AdminSidebar />
      </aside>
      <main
        id="main"
        tabIndex={-1}
        className="flex-1 overflow-y-auto px-8 py-8 focus:outline-none"
      >
        <div className="mx-auto max-w-5xl">
          <Outlet />
        </div>
      </main>
    </div>
  </div>
);
