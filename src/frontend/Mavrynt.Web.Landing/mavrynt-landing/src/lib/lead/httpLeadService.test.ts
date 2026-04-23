import { describe, it, expect, vi, beforeEach } from "vitest";
import { createHttpLeadService } from "./httpLeadService.ts";
import { LeadSubmissionError, type LeadPayload } from "./types.ts";

/**
 * HTTP adapter — the production-shaped contract. These tests lock in
 * the status-code → error-code mapping because the form UI branches on
 * `LeadSubmissionError.code` to render precise copy without parsing
 * response bodies. Any future change to this mapping must update the
 * form copy in lockstep.
 */
describe("httpLeadService", () => {
  const endpoint = "https://example.test/leads";
  const payload: LeadPayload = {
    name: "Ada",
    email: "ada@example.com",
    message: "Hello, a sufficiently long message.",
    source: "landing:contact",
    locale: "en",
  };

  const makeResponse = (status: number, ok = status >= 200 && status < 300) =>
    ({ status, ok }) as Response;

  let fetchMock: ReturnType<typeof vi.fn<typeof fetch>>;

  beforeEach(() => {
    fetchMock = vi.fn<typeof fetch>();
    vi.stubGlobal("fetch", fetchMock);
  });

  it("resolves when the server returns 2xx", async () => {
    fetchMock.mockResolvedValueOnce(makeResponse(204));
    const service = createHttpLeadService({ endpoint });

    await expect(service.submit(payload)).resolves.toBeUndefined();
  });

  it("posts JSON with the expected headers and stringified body", async () => {
    fetchMock.mockResolvedValueOnce(makeResponse(200));
    const service = createHttpLeadService({
      endpoint,
      headers: { "X-Api-Key": "abc" },
    });

    await service.submit(payload);

    expect(fetchMock).toHaveBeenCalledTimes(1);
    const [url, init] = fetchMock.mock.calls[0] ?? [];
    expect(url).toBe(endpoint);
    expect(init?.method).toBe("POST");
    expect(init?.body).toBe(JSON.stringify(payload));
    expect(init?.headers).toMatchObject({
      "Content-Type": "application/json",
      Accept: "application/json",
      "X-Api-Key": "abc",
    });
    expect(init?.signal).toBeInstanceOf(AbortSignal);
  });

  it.each([
    [422, "validation"] as const,
    [400, "validation"] as const,
    [429, "rate_limited"] as const,
    [500, "server"] as const,
    [503, "server"] as const,
  ])("maps status %i to error code %s", async (status, code) => {
    fetchMock.mockResolvedValueOnce(makeResponse(status));
    const service = createHttpLeadService({ endpoint });

    await expect(service.submit(payload)).rejects.toMatchObject({
      name: "LeadSubmissionError",
      code,
    });
  });

  it("wraps a thrown fetch in a network LeadSubmissionError and preserves cause", async () => {
    const cause = new TypeError("fetch failed");
    fetchMock.mockRejectedValueOnce(cause);
    const service = createHttpLeadService({ endpoint });

    const err: unknown = await service.submit(payload).catch((e: unknown) => e);

    expect(err).toBeInstanceOf(LeadSubmissionError);
    expect(err).toMatchObject({ code: "network" });
    expect((err as LeadSubmissionError).cause).toBe(cause);
  });

  it("aborts after the configured timeout and surfaces a network error", async () => {
    vi.useFakeTimers();
    try {
      // Simulate the server never responding — we resolve only if the
      // signal is aborted, mirroring how real `fetch` rejects on abort.
      fetchMock.mockImplementationOnce(
        (_input, init) =>
          new Promise<Response>((_resolve, reject) => {
            init?.signal?.addEventListener("abort", () => {
              reject(new DOMException("aborted", "AbortError"));
            });
          }),
      );
      const service = createHttpLeadService({ endpoint, timeoutMs: 50 });

      const pending = service.submit(payload);
      await vi.advanceTimersByTimeAsync(60);
      await expect(pending).rejects.toMatchObject({
        name: "LeadSubmissionError",
        code: "network",
      });
    } finally {
      vi.useRealTimers();
    }
  });
});
