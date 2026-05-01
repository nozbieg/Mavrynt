import { Seo } from "../lib/seo/Seo.tsx";
import { AdminPageHeader } from "../components/AdminPageHeader.tsx";
import { AdminCard } from "../components/AdminCard.tsx";

// TODO: Backend has no GET /api/admin/users list endpoint.
// Only PATCH /{userId}/role exists. Add list endpoint when available.

const UsersPage = () => (
  <>
    <Seo title="Users — Mavrynt Admin" description="Manage user accounts" />
    <AdminPageHeader
      title="Users"
      description="Manage user accounts and roles."
    />
    <AdminCard>
      <p className="text-sm text-fg-muted">
        User listing is not available yet — the backend does not expose a list
        users endpoint at this time. To assign a role to a known user, use the
        API directly:{" "}
        <code className="rounded bg-bg-subtle px-1 py-0.5 text-xs font-mono">
          PATCH /api/admin/users/{"{userId}"}/role
        </code>
        .
      </p>
    </AdminCard>
  </>
);

export default UsersPage;
