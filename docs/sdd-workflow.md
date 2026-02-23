# Spec-Driven Development Workflow

This project uses a **spec-driven development** workflow. Instead of jumping straight into code, features are planned through three structured phases with approval gates.

## Why Spec-Driven?

- **Reduces rework** — requirements and design are validated before any code is written
- **Better AI output** — structured context produces higher-quality generated code
- **Traceability** — every task links back to requirements, every design choice is documented
- **Incremental delivery** — each phase produces a reviewable artifact

## The 3-Phase Workflow

```
 ┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
 │ Requirements │ ──▶ │   Design    │ ──▶ │    Tasks    │ ──▶ │ Implement   │
 │   (WHAT)     │     │   (HOW)     │     │   (DO)      │     │   (CODE)    │
 └─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
    ▲ approve           ▲ approve           ▲ approve
```

Each phase requires **explicit approval** before proceeding to the next.

## Quick Start

### Step 1: Create Requirements

Describe your feature idea to the `@speckit-specify` agent:

```
@speckit-specify Add a product catalog API with browsable products
```

The agent will generate `.specs/features/{NNN}-{feature-name}/requirements.md`.

### Step 2: Create Design

After requirements are approved:

```
@plan Create a technical design for the product catalog feature.
Read requirements from .specs/features/001-product-catalog/requirements.md
and the design template from .specs/templates/design-template.md.
Save as .specs/features/001-product-catalog/design.md
```

### Step 3: Create Tasks

After design is approved:

```
@task Break the product catalog feature into implementation tasks.
Read requirements and design from .specs/features/001-product-catalog/
and the tasks template from .specs/templates/tasks-template.md.
Save as .specs/features/001-product-catalog/tasks.md
```

### Step 4: Implement

Work through tasks incrementally. Validate after each phase:

```bash
# Backend
dotnet build    # Zero warnings
dotnet test     # All tests pass
dotnet format --verify-no-changes

# Frontend
cd src/web
npm run lint    # No lint errors
npm run build   # Clean build
```

## Directory Structure

```
.specs/
├── templates/                          # Reusable templates (don't modify)
│   ├── requirements-template.md        # Phase 1: user stories + EARS criteria
│   ├── design-template.md              # Phase 2: architecture + components
│   └── tasks-template.md               # Phase 3: numbered implementation checklist
└── features/                           # Feature specifications
    └── 001-sample-feature/             # Example feature
        ├── requirements.md             # What to build
        ├── design.md                   # How to build it
        └── tasks.md                    # Step-by-step plan
```

## EARS Acceptance Criteria Format

| Pattern | When to Use | Example |
|---------|-------------|---------|
| **WHEN** X, **THEN SHALL** Y | Event-driven | WHEN user calls GET /api/products, THEN the system SHALL return all products |
| **IF** X, **THEN SHALL** Y | Conditional | IF no products exist, THEN the system SHALL return an empty array |
| **WHILE** X, **SHALL** Y | State-driven | WHILE data is loading, the system SHALL display a spinner |
