---
description: Frontend React/Next.js conventions, tech stack, and quality bar
applyTo: "src/web/**/*.{ts,tsx,js,jsx}"
---

# Frontend (React/Next.js) — Skills & Conventions

You have access to skills in `.github/skills/frontend/` for project-specific best practices. **YOU MUST** use these skills before answering questions or making changes related to these topics:

- React features: `react-best-practices`
- React components (composition): `vercel-composition-patterns`
- Custom UI components: `ui-components`
- Next.js features: `next-best-practices`
- State management: `zustand`
- New feature files: `web-project-conventions`

## Tech Stack

- **Framework**: Next.js 16 (App Router)
- **Runtime / package manager**: Bun (npm as fallback)
- **UI**: shadcn/ui components (built on Base UI primitives)
- **Styling**: Tailwind CSS 4
- **Linting/formatting**: oxlint / oxfmt
- **Type checking**: TypeScript 5
- **State**: Zustand (decoupled actions pattern)

## Quality Bar

```bash
cd src/web
bun run lint       # or: npm run lint
bun run build      # or: npm run build
```

## Code Style

oxlint + oxfmt. Use absolute `@/` imports. PascalCase for components, camelCase for functions, kebab-case for file names.
