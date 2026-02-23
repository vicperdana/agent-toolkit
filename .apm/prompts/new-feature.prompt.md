---
description: Spec-driven new feature workflow using project templates
mode: agent
input: [feature_name, feature_description]
---

# New Feature: ${input:feature_name}

${input:feature_description}

## Workflow

Follow the spec-driven development workflow (Requirements → Design → Tasks):

### Step 1: Requirements
Create a requirements spec using the template at `.specs/templates/requirements-template.md`.
Save to `.specs/features/{NNN}-${input:feature_name}/requirements.md`.

### Step 2: Design
Create a design doc using the template at `.specs/templates/design-template.md`.
Save to `.specs/features/{NNN}-${input:feature_name}/design.md`.

### Step 3: Tasks
Break down into implementable tasks using `.specs/templates/tasks-template.md`.
Save to `.specs/features/{NNN}-${input:feature_name}/tasks.md`.

### Step 4: Implement
Follow Test-First development:
1. Write failing tests (Red)
2. Implement minimal code to pass (Green)
3. Refactor for clarity (Refactor)

### Step 5: Verify
```bash
dotnet build       # Zero warnings
dotnet test        # All tests pass
cd src/web && bun run lint && bun run build
```

See `docs/sdd-workflow.md` for the full workflow guide.
