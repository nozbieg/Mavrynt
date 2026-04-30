import { adminApiRequest } from "./adminApiClient";
export type FeatureFlag = { key: string; name: string; description?: string; isEnabled: boolean };
export const featureFlagsClient = {
  list: (token: string) => adminApiRequest<FeatureFlag[]>("/admin/feature-flags", token),
  create: (token: string, payload: FeatureFlag) => adminApiRequest<FeatureFlag>("/admin/feature-flags", token, { method: "POST", body: JSON.stringify(payload) }),
  update: (token: string, key: string, payload: Pick<FeatureFlag,"name"|"description">) => adminApiRequest<FeatureFlag>(`/admin/feature-flags/${encodeURIComponent(key)}`, token, { method: "PATCH", body: JSON.stringify(payload) }),
  toggle: (token: string, key: string) => adminApiRequest<void>(`/admin/feature-flags/${encodeURIComponent(key)}/toggle`, token, { method: "PATCH" }),
};
