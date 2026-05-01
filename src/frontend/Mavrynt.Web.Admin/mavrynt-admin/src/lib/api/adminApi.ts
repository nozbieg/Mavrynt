import { clearAdminSession, getAdminAccessToken } from "../auth/adminSession.ts";

export type AdminProfile = {
  id: string;
  email: string;
  displayName?: string;
  status: string;
  role: string;
  createdAt: string;
  updatedAt?: string;
  requiresPasswordChange: boolean;
};

export type UserDto = {
  id: string;
  email: string;
  displayName?: string;
  status: string;
  role: string;
  createdAt: string;
  updatedAt?: string;
  requiresPasswordChange: boolean;
};

export type FeatureFlagDto = {
  id: string;
  key: string;
  name: string;
  description?: string;
  isEnabled: boolean;
  createdAt: string;
  updatedAt?: string;
};

export type SmtpSettingsDto = {
  id: string;
  providerName: string;
  host: string;
  port: number;
  username: string;
  senderEmail: string;
  senderName: string;
  useSsl: boolean;
  isEnabled: boolean;
  createdAt: string;
  updatedAt?: string;
};

export class ApiError extends Error {
  readonly status: number;
  readonly body: unknown;
  constructor(status: number, body: unknown) {
    super(`HTTP ${String(status)}`);
    this.status = status;
    this.body = body;
  }
}

const BASE = "/admin-api";

async function apiFetch<T>(path: string, init?: RequestInit): Promise<T> {
  const token = getAdminAccessToken();
  const headers: Record<string, string> = {
    Accept: "application/json",
    ...(init?.body !== undefined ? { "Content-Type": "application/json" } : {}),
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
  };

  let response: Response;
  try {
    response = await fetch(`${BASE}${path}`, {
      ...init,
      headers,
      credentials: "include",
    });
  } catch (cause) {
    throw new ApiError(0, { message: "Network error" });
  }

  if (response.status === 401 || response.status === 403) {
    clearAdminSession();
    window.location.assign("/login");
    throw new ApiError(response.status, null);
  }

  if (!response.ok) {
    let body: unknown = null;
    try {
      body = await response.json();
    } catch {
      // ignore
    }
    throw new ApiError(response.status, body);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export const adminApi = {
  getCurrentAdmin: (): Promise<AdminProfile> =>
    apiFetch<AdminProfile>("/admin/me"),

  assignUserRole: (userId: string, role: string): Promise<UserDto> =>
    apiFetch<UserDto>(`/admin/users/${userId}/role`, {
      method: "PATCH",
      body: JSON.stringify({ role }),
    }),

  listFeatureFlags: (): Promise<FeatureFlagDto[]> =>
    apiFetch<FeatureFlagDto[]>("/admin/feature-flags/"),

  createFeatureFlag: (data: {
    key: string;
    name: string;
    description?: string;
    isEnabled: boolean;
  }): Promise<FeatureFlagDto> =>
    apiFetch<FeatureFlagDto>("/admin/feature-flags/", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  updateFeatureFlag: (
    key: string,
    data: { name: string; description?: string },
  ): Promise<FeatureFlagDto> =>
    apiFetch<FeatureFlagDto>(`/admin/feature-flags/${key}`, {
      method: "PATCH",
      body: JSON.stringify(data),
    }),

  toggleFeatureFlag: (key: string): Promise<FeatureFlagDto> =>
    apiFetch<FeatureFlagDto>(`/admin/feature-flags/${key}/toggle`, {
      method: "PATCH",
    }),

  listSmtpSettings: (): Promise<SmtpSettingsDto[]> =>
    apiFetch<SmtpSettingsDto[]>("/admin/notifications/smtp-settings/"),

  getSmtpSettings: (id: string): Promise<SmtpSettingsDto> =>
    apiFetch<SmtpSettingsDto>(`/admin/notifications/smtp-settings/${id}`),

  createSmtpSettings: (data: {
    providerName: string;
    host: string;
    port: number;
    username: string;
    password: string;
    senderEmail: string;
    senderName: string;
    useSsl: boolean;
    isEnabled: boolean;
  }): Promise<SmtpSettingsDto> =>
    apiFetch<SmtpSettingsDto>("/admin/notifications/smtp-settings/", {
      method: "POST",
      body: JSON.stringify(data),
    }),

  updateSmtpSettings: (
    id: string,
    data: {
      providerName: string;
      host: string;
      port: number;
      username: string;
      password?: string | undefined;
      senderEmail: string;
      senderName: string;
      useSsl: boolean;
    },
  ): Promise<SmtpSettingsDto> =>
    apiFetch<SmtpSettingsDto>(`/admin/notifications/smtp-settings/${id}`, {
      method: "PATCH",
      body: JSON.stringify(data),
    }),

  enableSmtpSettings: (id: string): Promise<SmtpSettingsDto> =>
    apiFetch<SmtpSettingsDto>(
      `/admin/notifications/smtp-settings/${id}/enable`,
      { method: "PATCH" },
    ),
};
