# ADR 0002: HasSeenOnboarding stored on ApplicationUser

**Status:** Accepted  
**Date:** 2026-09-24

## Context

The onboarding wizard must be shown to every user exactly once — on their first login. Three candidate locations for the completion flag were considered:

1. **`Tenant.HasSeenOnboarding`** — one flag per tenant, not per user.
2. **`ApplicationUser.HasSeenOnboarding`** — one flag per user.
3. **Browser localStorage** — keyed by tenant or user ID, frontend-only.
4. **Implicit heuristic** — infer from data (e.g. show wizard if tenant has zero suppliers).

## Decision

Store `HasSeenOnboarding bool` on `ApplicationUser`.

## Reasoning

**Against `Tenant`**: the wizard must appear for every user on their first login regardless of whether another user on the same tenant has already completed it. A tenant-level flag would suppress the wizard for all subsequent users on that tenant after the first admin completes it.

**Against localStorage**: survives browser clears is a hard requirement for a first-login gate. A user who clears their browser data would see the wizard again, which breaks the "exactly once" guarantee without any workaround short of a backend flag.

**Against implicit heuristic**: "zero suppliers" as a proxy is fragile — a supplier created via API, CSV import, or by another user would suppress the wizard for someone who has never seen it. The heuristic also cannot be reset or audited.

**For `ApplicationUser`**: correctly scopes to individual users, survives browser clears, is explicit and auditable, and aligns with the existing pattern of per-user state (`IsActive`, `CreatedAt`) on `ApplicationUser`.

## Consequences

- DB migration on `AuthDbContext` adds `HasSeenOnboarding bit NOT NULL DEFAULT 0` to `AspNetUsers`.
- `GET /api/tenant` response includes `hasSeenOnboarding` sourced from the current user via `UserManager.FindByIdAsync`.
- `POST /api/tenant/complete-onboarding` sets the flag for the current user and saves via `IAuthUnitOfWork`.
- All existing users have `HasSeenOnboarding = false` after migration; the wizard will appear for them on next login.
