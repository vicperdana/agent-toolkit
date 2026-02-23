# AGENTS.md — Project Guidelines for AI Assistants

Guidelines for AI assistants (e.g., Codex, GitHub Copilot) contributing to this repo.

## Golden Rules

- Make small, correct, reviewable changes
- Keep diffs minimal; don't refactor unrelated code
- Prefer existing repo patterns over general best practices
- Don't add dependencies unless clearly necessary

## Architecture

This is a **full-stack monorepo** with two distinct stacks:

| Layer | Tech | Path | Package Manager |
|-------|------|------|-----------------|
| **Backend API** | ASP.NET Core 10 (Minimal API) | `src/Api/` | `dotnet` |
| **Shared Domain** | .NET 10 class library | `src/Shared/` | `dotnet` |
| **Frontend** | Next.js 16 + React 19 | `src/web/` | `bun` / `npm` |
| **Backend Tests** | xUnit | `tests/Api.Tests/` | `dotnet` |

## Backend (.NET) — Skills & Conventions

You have access to skills in `.github/skills/backend/` for project-specific best practices. **YOU MUST** use these skills before answering questions or making changes related to these topics:

- API endpoints & routing: `dotnet-api`
- Architecture & domain logic: `clean-architecture`
- Testing & TDD: `testing`

### Constitutional Principles (Backend)

1. **Shared-First** — Domain logic starts in `src/Shared/` (entities, interfaces, extensions). No business rules in the Api project without first being defined in Shared.
2. **Test-First** — Write tests before implementation. Red → Green → Refactor.
3. **Simplicity** — Use ASP.NET Core features directly. No repository pattern, mediator, or CQRS unless the requirement explicitly demands it.
4. **Anti-Abstraction** — One entity class per domain concept. No speculative features.

### Tech Stack (Backend)

- **Framework**: ASP.NET Core 10.0 (Minimal API)
- **Language**: C# 14
- **Testing**: xUnit
- **Package Management**: Central Package Management (`Directory.Packages.props`)

### Quality Bar (Backend)

```bash
dotnet build       # Zero warnings (TreatWarningsAsErrors enabled)
dotnet test        # All tests pass
dotnet format      # Code style per .editorconfig
```

## Frontend (React/Next.js) — Skills & Conventions

You have access to skills in `.github/skills/frontend/` for project-specific best practices. **YOU MUST** use these skills before answering questions or making changes related to these topics:

- React features: `react-best-practices`
- React components (composition): `vercel-composition-patterns`
- Custom UI components: `ui-components`
- Next.js features: `next-best-practices`
- State management: `zustand`
- New feature files: `web-project-conventions`

### Tech Stack (Frontend)

- **Framework**: Next.js 16 (App Router)
- **Runtime / package manager**: Bun (npm as fallback)
- **UI**: shadcn/ui components (built on Base UI primitives)
- **Styling**: Tailwind CSS 4
- **Linting/formatting**: oxlint / oxfmt
- **Type checking**: TypeScript 5
- **State**: Zustand (decoupled actions pattern)

### Quality Bar (Frontend)

```bash
cd src/web
bun run lint       # or: npm run lint
bun run build      # or: npm run build
```

## Full-Stack — Skills & Conventions

Cross-cutting skills in `.github/skills/fullstack/`:

- E2E testing: `playwright-cli`

### API Proxy (Dev)

The Next.js frontend proxies `/api/*` requests to the .NET backend at `http://localhost:5001`. This is configured in `src/web/next.config.ts`.

### Development Workflow

```bash
# Terminal 1: Start .NET API
dotnet run --project src/Api     # http://localhost:5001

# Terminal 2: Start Next.js frontend
cd src/web && bun dev            # http://localhost:3000
```

## Spec-Driven Development

Use the spec-driven workflow (Requirements → Design → Tasks) for new features:
- Templates in `.specs/templates/`
- Specs in `.specs/features/{NNN}-{feature-name}/`
- See `docs/sdd-workflow.md` for the full workflow guide

## Code Style

- **Backend**: Defined in `.editorconfig`. Use `var` when type is apparent. Interface names start with `I`.
- **Frontend**: oxlint + oxfmt. Use absolute `@/` imports. PascalCase for components, camelCase for functions, kebab-case for file names.

## Final Response Requirements

Include:

- What you changed (1–5 bullets)
- Why you changed it
- How you verified it (exact commands + results)
- Any follow-ups or risks
