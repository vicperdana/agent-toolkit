---
description: Full-stack cross-cutting conventions (E2E testing, API proxy)
applyTo: "**/*"
---

# Full-Stack Conventions

Cross-cutting skills in `.github/skills/fullstack/`:

- E2E testing: `playwright-cli`

## API Proxy (Dev)

The Next.js frontend proxies `/api/*` requests to the .NET backend at `http://localhost:5001`. This is configured in `src/web/next.config.ts`.
