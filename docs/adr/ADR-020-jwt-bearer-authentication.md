# ADR-020 — JWT Bearer Authentication Strategy

**Date:** 2026-04-28  
**Status:** Accepted

---

## Context

Mavrynt needs a stateless, cross-service authentication mechanism that works for both the user-facing API (`Mavrynt.Api`) and the admin API (`Mavrynt.AdminApp`). Both backends share the same Users module and must validate tokens issued by the same signing key.

Requirements:
- Stateless tokens (no server-side session store)
- Role-based authorization (`User`, `Admin`)
- Shared token validation logic between both backend services
- Short-lived access tokens with configurable expiry

## Decision

Use **JWT Bearer tokens** (HMAC-SHA-256 / HS256) issued by `Mavrynt.Api` and validated by both `Mavrynt.Api` and `Mavrynt.AdminApp` against a shared symmetric signing key.

Token claims:
- `sub` — user ID (GUID)
- `email` — user email address
- `http://schemas.microsoft.com/ws/2008/06/identity/claims/role` — `User` or `Admin`
- `name` — display name (optional)
- `jti` — unique token ID

Configuration shape (both services, `Jwt` section):

```json
{
  "Jwt": {
    "Secret": "<min-32-char-secret>",
    "Issuer": "Mavrynt.Api",
    "Audience": "Mavrynt",
    "ExpirationMinutes": 60
  }
}
```

`MapInboundClaims = false` is set on both services so `sub` is never remapped to Microsoft's legacy claim URI.

Authorization policies:
- `AdminOnly` — `RequireAuthenticatedUser` + `RequireRole("Admin")` — registered on both services

## Consequences

**Positive:**
- Simple, portable, no infrastructure dependency (no Redis, no DB session table)
- Tokens are self-contained and work across both backends without coordination
- Standard `Bearer` scheme is supported by all frontend clients

**Negative / Trade-offs:**
- Tokens cannot be revoked before expiry without an allowlist/denylist (deferred)
- Secret rotation requires redeployment of both services simultaneously
- HS256 symmetric key must be kept out of source control (env var / user secrets)

**Future work:**
- Refresh tokens (deferred to post-MVP)
- Token revocation / short-lived access + long-lived refresh pattern
- Migrate to RS256 asymmetric keys when services are split
