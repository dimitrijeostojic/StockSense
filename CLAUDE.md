## Agent skills

### Issue tracker

GitHub Issues, split across TWO repos:
- Backend tickets and all specs: dimitrijeostojic/StockSense
- Frontend tickets: dimitrijeostojic/StockSenseUI
Every ticket goes to exactly one repo. Before creating any issue,
read `docs/agents/issue-tracker.md` ("Two-repo setup" section).
Every new issue is added to GitHub project 2.

### Domain docs

Single-context layout: one `CONTEXT.md` + `docs/adr/` at repo root. See `docs/agents/domain.md`.

## Project structure
This app is split across two repos:
- Backend (this repo): StockSense
- Frontend: ../StockSenseUI2 locally (GitHub: dimitrijeostojic/StockSenseUI)
When implementing or reviewing backend work, check whether the frontend
needs matching changes (API contracts, DTOs, endpoints), but do not
change ../StockSenseUI2 unless explicitly asked.

## Git workflow
Work on one branch per feature, not per issue: feature/<feature-name>.
If the branch exists, continue on it. Commit each issue separately on it.
Never commit to master. Never push and never open PRs — the developer
reviews locally and pushes manually.