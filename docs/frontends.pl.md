# Mavrynt — Topologia frontendów

Ten dokument opisuje, jak trzy jednostronicowe aplikacje Mavrynt łączą się ze sobą, jak są hostowane w trybie deweloperskim, jak rozmawiają z backendem oraz jak rozwiązywane są zmienne środowiskowe. To operacyjne uzupełnienie `ADR-010` (frontendy oddzielone od backendu), `ADR-015` (niezależność landingu) i `ADR-016` (rozwiązywanie URL-i między aplikacjami).

## Trzy SPA

| SPA | Pakiet | Rola | Host backendu | Domyślny dev URL |
| --- | --- | --- | --- | --- |
| Landing marketingowy | `mavrynt-landing` | Publiczne drzwi frontowe; marketing, kontakt, strony legalne | Brak (niezależny od backendu — patrz `ADR-010`) | `http://localhost:5173` |
| Aplikacja web | `mavrynt-web` | Workspace użytkownika — logowanie, rejestracja, powierzchnie produktowe | `Mavrynt.Api` | `http://localhost:5174` |
| Konsola administracyjna | `mavrynt-admin` | Wewnętrzna konsola operatora | `Mavrynt.AdminApp` | `http://localhost:5175` |

Landing celowo nie ma powierzchni auth. Oba CTA `/login` i `/register` na stronie marketingowej są linkami zewnętrznymi do `mavrynt-web`, rozwiązywanymi przez `@mavrynt/config/app-urls`. Kierunek odwrotny: `mavrynt-web` i `mavrynt-admin` linkują z powrotem do landingu przez ten sam resolver (stopka „powrót do mavrynt.com" + awaryjne linki w navie).

## Rozwiązywanie URL-i między aplikacjami

Wszystkie linki między SPA przechodzą przez jeden pomocnik eksportowany z `@mavrynt/config`:

```ts
import { resolveAppUrls } from "@mavrynt/config/app-urls";

const appUrls = resolveAppUrls();
// appUrls.landing, appUrls.web, appUrls.admin
```

### Matryca zmiennych środowiskowych

Klucze kanoniczne (preferowane, ustawiane per wdrożenie):

| Zmienna | Dotyczy | Przykład |
| --- | --- | --- |
| `VITE_APP_URL_LANDING` | Strona marketingowa | `https://mavrynt.com` |
| `VITE_APP_URL_WEB` | SPA użytkownika | `https://app.mavrynt.com` |
| `VITE_APP_URL_ADMIN` | Konsola administracyjna | `https://admin.mavrynt.com` |

Legacy aliasy (nadal honorowane; wycofać, gdy to możliwe):

| Legacy zmienna | Mapuje na |
| --- | --- |
| `VITE_MARKETING_URL` | `landing` |
| `VITE_WEB_URL` | `web` |
| `VITE_ADMIN_URL` | `admin` |

Gdy żadna zmienna nie jest ustawiona, `resolveAppUrls()` korzysta z fallbacków na developerskie porty wymienione wyżej. Końcowe slashe są normalizowane, dzięki czemu wywołujący mogą bezpiecznie pisać `` `${appUrls.web}/login` ``. Zwrócony obiekt jest zamrożony — nie mutować.

## Proxy dev Vite

`mavrynt-web` i `mavrynt-admin` proxują `/api/*` do swoich hostów backendu podczas developmentu, dzięki czemu SPA wywołują relatywne URL-e (`fetch("/api/auth/login")`) bez CORS i bez wiązania env. Proxy jest zdefiniowane raz na aplikację w `vite.config.ts` i współdzielone między `server` i `preview`.

| SPA | Upstream | Domyślny cel | Override env |
| --- | --- | --- | --- |
| `mavrynt-web` | `/api/*` | `http://localhost:5000` | `VITE_API_PROXY_TARGET` |
| `mavrynt-admin` | `/api/*` | `http://localhost:5001` | `VITE_ADMIN_API_PROXY_TARGET` |

`mavrynt-landing` nie ma proxy backendu — pozostaje niezależny od backendu (`ADR-010` / `ADR-015`). Dane, których potrzebuje (np. wysyłka leada), idą przez port (`LeadService`), którego domyślny adapter to logger konsolowy w dev, a w prod zamieniany na adapter HTTP w `Providers.tsx`.

## Piramida testów per SPA

| SPA | Unit / integracyjne | E2E smoke |
| --- | --- | --- |
| `mavrynt-landing` | Vitest + jsdom + Testing Library | Playwright (Chromium) — home/contact, pricing/FAQ, zmiana języka |
| `mavrynt-web` | Vitest + jsdom + Testing Library | Playwright (Chromium) — login + register (adapter konsolowy) |
| `mavrynt-admin` | Vitest + jsdom + Testing Library | Jeszcze nie — do dodania, gdy trasy będą miały realny stan |

Każda SPA uruchamia własne testy jednostkowe przez `npm run test`. Zestawy Playwrighta żyją pod `tests/e2e/` i są wyłączone z globa include Vitesta, dzięki czemu runy unit pozostają hermetyczne.

## Dodawanie czwartej SPA (checklist)

1. Postaw szkielet pod `src/frontend/<name>/`, odzwierciedlając strukturę `mavrynt-web`.
2. Dopisz go do tablicy workspaces w rootowym `package.json`.
3. Zarejestruj nową pozycję w `AppId` w `@mavrynt/config/src/app-urls.ts`, rozszerz `DEFAULT_APP_URLS`, `PRIMARY_ENV_KEYS` i `LEGACY_ENV_KEYS`.
4. Zaktualizuj ten dokument oraz `ADR-016`.
5. Dodaj jego dev port do tabeli wyżej.
6. Zarejestruj go w `Mavrynt.AppHost` dla spójnej orkiestracji lokalnej.

Trzymanie się tej listy jest tańsze niż odkrycie później, że URL jest wbity w kodzie.
