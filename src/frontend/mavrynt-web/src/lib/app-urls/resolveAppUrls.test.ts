import { describe, it, expect } from "vitest";
import {
  resolveAppUrls,
  resolveAppUrl,
  DEFAULT_APP_URLS,
} from "@mavrynt/config/app-urls";

/**
 * Unit coverage for the cross-app URL resolver.
 *
 * We host the test in `mavrynt-web` (not in `@mavrynt/config`) because
 * the shared config package is source-shipped and has no test runner of
 * its own — the three SPAs each have Vitest, so any of them can exercise
 * the helper with zero duplication of toolchain. Web is the primary
 * consumer of `resolveAppUrls()` at runtime, so it owns the coverage.
 *
 * The helper takes an env-source argument (Dependency Inversion), so
 * tests don't need to touch `process.env` / `import.meta.env`.
 */
describe("resolveAppUrls", () => {
  it("falls back to the documented dev defaults when no env is provided", () => {
    const urls = resolveAppUrls({});

    expect(urls).toEqual({
      landing: DEFAULT_APP_URLS.landing,
      web: DEFAULT_APP_URLS.web,
      admin: DEFAULT_APP_URLS.admin,
    });
  });

  it("uses the canonical VITE_APP_URL_* keys when they are set", () => {
    const urls = resolveAppUrls({
      VITE_APP_URL_LANDING: "https://mavrynt.com",
      VITE_APP_URL_WEB: "https://app.mavrynt.com",
      VITE_APP_URL_ADMIN: "https://admin.mavrynt.com",
    });

    expect(urls).toEqual({
      landing: "https://mavrynt.com",
      web: "https://app.mavrynt.com",
      admin: "https://admin.mavrynt.com",
    });
  });

  it("honours legacy VITE_MARKETING_URL / VITE_WEB_URL / VITE_ADMIN_URL aliases", () => {
    const urls = resolveAppUrls({
      VITE_MARKETING_URL: "https://legacy-marketing.example.com",
      VITE_WEB_URL: "https://legacy-web.example.com",
      VITE_ADMIN_URL: "https://legacy-admin.example.com",
    });

    expect(urls.landing).toBe("https://legacy-marketing.example.com");
    expect(urls.web).toBe("https://legacy-web.example.com");
    expect(urls.admin).toBe("https://legacy-admin.example.com");
  });

  it("prefers the canonical key over its legacy alias", () => {
    const urls = resolveAppUrls({
      VITE_APP_URL_LANDING: "https://new.example.com",
      VITE_MARKETING_URL: "https://legacy.example.com",
    });

    expect(urls.landing).toBe("https://new.example.com");
  });

  it("treats empty-string env values as absent (falls through to next source)", () => {
    const urls = resolveAppUrls({
      VITE_APP_URL_LANDING: "",
      VITE_MARKETING_URL: "https://legacy.example.com",
      VITE_APP_URL_WEB: "",
      VITE_APP_URL_ADMIN: "",
    });

    expect(urls.landing).toBe("https://legacy.example.com");
    expect(urls.web).toBe(DEFAULT_APP_URLS.web);
    expect(urls.admin).toBe(DEFAULT_APP_URLS.admin);
  });

  it("normalises trailing slashes so callers can safely append paths", () => {
    const urls = resolveAppUrls({
      VITE_APP_URL_WEB: "https://app.mavrynt.com/",
      VITE_APP_URL_ADMIN: "https://admin.mavrynt.com///",
    });

    expect(urls.web).toBe("https://app.mavrynt.com");
    expect(urls.admin).toBe("https://admin.mavrynt.com");
  });

  it("returns a frozen object (callers must not mutate)", () => {
    const urls = resolveAppUrls({});

    expect(Object.isFrozen(urls)).toBe(true);
  });

  it("resolveAppUrl shorthand returns the matching entry", () => {
    const env = {
      VITE_APP_URL_WEB: "https://app.mavrynt.com",
    } as const;

    expect(resolveAppUrl("web", env)).toBe("https://app.mavrynt.com");
    expect(resolveAppUrl("landing", env)).toBe(DEFAULT_APP_URLS.landing);
  });
});
