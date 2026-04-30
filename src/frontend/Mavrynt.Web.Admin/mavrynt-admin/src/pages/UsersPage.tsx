import { useState } from "react";
import { usersClient } from "../lib/admin-api/usersClient";
import { useAdminAuth } from "../lib/auth/useAdminAuth";
import { AdminApiAuthError } from "../lib/admin-api/adminApiClient";

export default function UsersPage() {
  const { accessToken, logout } = useAdminAuth();
  const [userId, setUserId] = useState("");
  const [role, setRole] = useState("admin");
  const [message, setMessage] = useState("");

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-semibold">Users</h1>
      <p>User listing API is not available yet. You can assign role manually.</p>

      <form
        className="max-w-md space-y-2 rounded border p-3"
        onSubmit={async (event) => {
          event.preventDefault();
          if (!accessToken) return;

          try {
            await usersClient.assignRole(accessToken, userId, role);
            setMessage("Role updated successfully.");
          } catch (err) {
            if (err instanceof AdminApiAuthError) {
              await logout();
              return;
            }
            setMessage("Failed to update role.");
          }
        }}
      >
        <input
          className="w-full border p-2"
          placeholder="User ID"
          value={userId}
          onChange={(event) => setUserId(event.target.value)}
        />
        <input
          className="w-full border p-2"
          placeholder="Role"
          value={role}
          onChange={(event) => setRole(event.target.value)}
        />
        <button className="border px-3 py-2">Assign role</button>
      </form>

      {message && <p>{message}</p>}
    </div>
  );
}
