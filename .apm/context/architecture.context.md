---
description: Project architecture overview and monorepo layout
---

# Architecture

This is a **full-stack monorepo** with two distinct stacks:

| Layer | Tech | Path | Package Manager |
|-------|------|------|-----------------|
| **Backend API** | ASP.NET Core 10 (Minimal API) | `src/Api/` | `dotnet` |
| **Shared Domain** | .NET 10 class library | `src/Shared/` | `dotnet` |
| **Frontend** | Next.js 16 + React 19 | `src/web/` | `bun` / `npm` |
| **Backend Tests** | xUnit | `tests/Api.Tests/` | `dotnet` |
