import { adminApiRequest } from "./adminApiClient";
export type SmtpSettings = { id: string; providerName: string; host: string; port: number; username: string; senderEmail: string; senderName: string; useSsl: boolean; isEnabled: boolean };
export const smtpSettingsClient = {
  list: (token: string) => adminApiRequest<SmtpSettings[]>("/admin/notifications/smtp-settings", token),
  create: (token: string, payload: Record<string, unknown>) => adminApiRequest<SmtpSettings>("/admin/notifications/smtp-settings", token, { method: "POST", body: JSON.stringify(payload) }),
  update: (token: string, id: string, payload: Record<string, unknown>) => adminApiRequest<SmtpSettings>(`/admin/notifications/smtp-settings/${id}`, token, { method: "PATCH", body: JSON.stringify(payload) }),
  enable: (token: string, id: string) => adminApiRequest<void>(`/admin/notifications/smtp-settings/${id}/enable`, token, { method: "PATCH" }),
};
