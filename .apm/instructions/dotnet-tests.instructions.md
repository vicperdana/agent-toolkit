---
description: Testing conventions for .NET backend tests (xUnit, TDD)
applyTo: "tests/**/*.cs"
---

# Backend Testing Conventions

You have access to skills in `.github/skills/backend/` for project-specific best practices. **YOU MUST** use the `testing` skill before writing or modifying tests.

## Principles

1. **Test-First** — Write tests before implementation. Red → Green → Refactor.
2. **xUnit** — All backend tests use xUnit as the test framework.
3. **Arrange-Act-Assert** — Structure every test with clear AAA sections.

## Quality Bar

```bash
dotnet test        # All tests pass
```
