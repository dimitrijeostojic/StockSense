## Agent skills

### Issue tracker

Issues live in GitHub Issues at github.com/dimitrijeostojic/StockSense. See `docs/agents/issue-tracker.md`.

### Domain docs

Single-context layout: one `CONTEXT.md` + `docs/adr/` at repo root. See `docs/agents/domain.md`.


## Project structure
This app is split across two repos:
- Backend (this repo): StockSense
- Frontend: ../StockSenseUI2 (separate GitHub repo)
Features often span both. When implementing or reviewing, check whether
the frontend needs matching changes (API contracts, DTOs, endpoints).

## Git workflow
Never commit or push directly to main. Always create a feature branch
per issue, push it, and open a PR with `gh pr create`.