export class AdminApiAuthError extends Error {}

export class AdminApiError extends Error {
  constructor(
    readonly status: number,
    message: string,
    readonly code?: string,
  ) {
    super(message);
  }
}

const parseErrorMessage = async (
  response: Response,
): Promise<{ code?: string; message: string }> => {
  try {
    const payload = (await response.json()) as {
      code?: string;
      message?: string;
      detail?: string;
      title?: string;
    };

    return {
      code: payload.code,
      message:
        payload.message ??
        payload.detail ??
        payload.title ??
        `Request failed (${response.status})`,
    };
  } catch {
    return { message: `Request failed (${response.status})` };
  }
};

export const adminApiRequest = async <T>(
  path: string,
  token: string | null,
  init?: RequestInit,
): Promise<T> => {
  const response = await fetch(`/admin-api${path}`, {
    ...init,
    headers: {
      Accept: "application/json",
      ...(init?.body ? { "Content-Type": "application/json" } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init?.headers,
    },
    credentials: "include",
  });

  if (response.status === 401 || response.status === 403) {
    throw new AdminApiAuthError("Unauthorized");
  }

  if (!response.ok) {
    const payload = await parseErrorMessage(response);
    throw new AdminApiError(response.status, payload.message, payload.code);
  }

  if (response.status === 204) return undefined as T;

  return (await response.json()) as T;
};
