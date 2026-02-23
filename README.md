# Agent Toolkit

A full-stack starter template with **.NET API backend** and **Next.js frontend**, designed for use with GitHub Copilot agent skills.

## Tech Stack

| Layer | Technology | Path |
|-------|-----------|------|
| **Backend API** | ASP.NET Core 10.0 (Minimal API) | `src/Api/` |
| **Shared Domain** | .NET 10 class library (C# 14) | `src/Shared/` |
| **Frontend** | Next.js 16 + React 19 + Tailwind CSS 4 | `src/web/` |
| **Backend Tests** | xUnit | `tests/Api.Tests/` |

## Project Structure

```
/
├── src/
│   ├── Api/                    # ASP.NET Core Minimal API
│   │   ├── Endpoints/          # API endpoint groups
│   │   ├── Program.cs          # Application entry point
│   │   └── Properties/         # Launch settings
│   ├── Shared/                 # .NET shared library (domain logic)
│   │   ├── Entities/           # Domain models
│   │   ├── Interfaces/         # Service contracts
│   │   └── Extensions/         # DI registration & helpers
│   └── web/                    # Next.js frontend
│       ├── app/                # App Router pages
│       ├── components/         # UI components (shadcn/ui)
│       └── features/           # Feature modules
├── tests/
│   └── Api.Tests/              # .NET backend tests (xUnit)
├── .github/
│   ├── skills/
│   │   ├── backend/            # .NET-specific Copilot skills
│   │   ├── frontend/           # React/Next.js Copilot skills
│   │   └── fullstack/          # Cross-cutting Copilot skills
│   └── workflows/              # CI/CD pipelines
├── .specs/                     # Spec-driven development
│   ├── templates/              # Requirement/design/task templates
│   └── features/               # Feature specifications
├── AgentToolkit.sln            # .NET solution file
├── AGENTS.md                   # AI assistant guidelines
└── docs/                       # Project documentation
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/) (or [Bun](https://bun.sh/))

## Getting Started

### 1. Start the .NET API backend

```bash
dotnet build                         # Build entire .NET solution
dotnet run --project src/Api         # Start API at http://localhost:5001
```

### 2. Start the Next.js frontend

```bash
cd src/web
npm install                          # or: bun install
npm run dev                          # or: bun dev → http://localhost:3000
```

The frontend proxies `/api/*` requests to the .NET backend automatically.

### 3. Run tests

```bash
dotnet test                          # .NET backend tests
cd src/web && npm run lint           # Frontend linting
```

## Architecture

This project follows **Clean Architecture** principles:

- **Shared**: Domain entities, interfaces, and extensions (no external dependencies beyond Microsoft.Extensions.*)
- **Api**: ASP.NET Core Minimal API — depends on Shared
- **web**: Next.js frontend — communicates with Api via HTTP

The frontend proxies API calls through `next.config.ts` rewrites, so all `/api/*` requests are forwarded to the .NET backend during development.

## Copilot Agent Skills

This repo includes curated skill references that guide GitHub Copilot:

| Category | Skills |
|----------|--------|
| **Backend** | `dotnet-api`, `clean-architecture`, `testing` |
| **Frontend** | `react-best-practices`, `next-best-practices`, `ui-components`, `vercel-composition-patterns`, `web-project-conventions`, `zustand` |
| **Full-Stack** | `playwright-cli` |

See [AGENTS.md](AGENTS.md) for full guidelines.

## Spec-Driven Development

This project uses a structured workflow for feature development:

1. **Requirements** — Define what to build (user stories + EARS criteria)
2. **Design** — Define how to build it (architecture, components, tests)
3. **Tasks** — Break into numbered implementation steps

See [docs/sdd-workflow.md](docs/sdd-workflow.md) for the full guide.

## References

- [ASP.NET Core Minimal APIs](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Next.js App Router](https://nextjs.org/docs/app)
- [shadcn/ui](https://ui.shadcn.com/)
- [Clean Architecture](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
