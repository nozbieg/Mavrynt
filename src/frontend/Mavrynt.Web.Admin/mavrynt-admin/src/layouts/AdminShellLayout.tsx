import { Outlet } from "react-router";
import { AdminSidebar } from "./components/AdminSidebar";

export const AdminShellLayout = () => (
  <div className="mx-auto w-full max-w-7xl p-4">
    <div className="flex flex-col gap-4 overflow-x-hidden md:flex-row">
      <AdminSidebar />
      <main className="min-w-0 flex-1">
        <Outlet />
      </main>
    </div>
  </div>
);
