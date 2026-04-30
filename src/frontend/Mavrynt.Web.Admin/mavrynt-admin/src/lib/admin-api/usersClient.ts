import { adminApiRequest } from "./adminApiClient";
export const usersClient = { assignRole: (token: string, userId: string, role: string) => adminApiRequest<void>(`/admin/users/${userId}/role`, token, { method: "PATCH", body: JSON.stringify({ role }) }) };
