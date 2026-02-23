---
description: Structured code review workflow for pull requests
mode: agent
---

# Code Review

Review the code changes and provide feedback on the following areas:

## 1. Correctness
- Does the code do what it's supposed to do?
- Are there any logic errors or edge cases missed?

## 2. Architecture
- Does it follow Shared-First principles (domain logic in `src/Shared/`)?
- Are API endpoints following the patterns in `.github/skills/backend/dotnet-api/`?
- Do frontend components follow composition patterns?

## 3. Testing
- Are there tests for the changes?
- Do tests follow the Test-First approach?

## 4. Code Style
- Backend: `.editorconfig` compliance, `var` usage, `I` prefix for interfaces
- Frontend: `@/` imports, PascalCase components, kebab-case files

## 5. Quality Bar
Verify:
```bash
dotnet build       # Zero warnings
dotnet test        # All tests pass
cd src/web && bun run lint && bun run build
```

## Output Format
- What was changed (1–5 bullets)
- Issues found (critical → minor)
- Suggested improvements
