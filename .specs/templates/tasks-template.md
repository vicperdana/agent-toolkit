# [Feature Name] Implementation Tasks

## Task Overview

**Feature**: [Feature name from requirements.md]
**Total Tasks**: [X] tasks in [Y] phases
**References**: [`requirements.md`](requirements.md) · [`design.md`](design.md)

## Implementation Tasks

### Phase 1: Domain & Contracts

- [ ] **1.1** Create domain entity in `src/Shared/Entities/[Entity].cs`
  - **Deliverables**: Entity class with properties per design.md
  - **Requirements**: [Reference acceptance criteria]
  - **Dependencies**: None

- [ ] **1.2** Create service interface in `src/Shared/Interfaces/I[Service].cs`
  - **Deliverables**: Interface with method signatures per design.md
  - **Dependencies**: 1.1

- [ ] **1.3** Register service in `src/Shared/Extensions/ServiceCollectionExtensions.cs`
  - **Deliverables**: Add registration in `AddSharedServices()`
  - **Dependencies**: 1.2

### Phase 2: Tests (Write First)

- [ ] **2.1** Create backend unit tests in `tests/Api.Tests/UnitTests/[Service]Tests.cs`
  - **Deliverables**: Tests for all service methods; tests MUST FAIL initially
  - **Dependencies**: 1.2

### Phase 3: Backend Implementation

- [ ] **3.1** Implement service in `src/Shared/` or `src/Api/`
  - **Deliverables**: Service implementation that makes unit tests pass
  - **Dependencies**: 2.1

- [ ] **3.2** Create API endpoints in `src/Api/Endpoints/[Resource]Endpoints.cs`
  - **Deliverables**: Minimal API endpoints with OpenAPI metadata
  - **Dependencies**: 3.1

### Phase 4: Frontend Implementation

- [ ] **4.1** Create API client functions in `src/web/lib/api/`
  - **Deliverables**: Typed fetch functions for API endpoints
  - **Dependencies**: 3.2

- [ ] **4.2** Create feature page in `src/web/features/[feature]/`
  - **Deliverables**: React page component with data fetching
  - **Dependencies**: 4.1

- [ ] **4.3** Create/update App Router page in `src/web/app/`
  - **Deliverables**: Page route that renders the feature component
  - **Dependencies**: 4.2

### Phase 5: Validation & Polish

- [ ] **5.1** Verify all backend tests pass
  - **Deliverables**: `dotnet test` passes with all green
  - **Dependencies**: 3.2

- [ ] **5.2** Verify backend build has zero warnings
  - **Deliverables**: `dotnet build` compiles with no warnings
  - **Dependencies**: 5.1

- [ ] **5.3** Verify frontend builds cleanly
  - **Deliverables**: `npm run build` in `src/web/` succeeds
  - **Dependencies**: 4.3

## Task Guidelines

### Path Conventions
- **Entities**: `src/Shared/Entities/`
- **Interfaces**: `src/Shared/Interfaces/`
- **Extensions**: `src/Shared/Extensions/`
- **API Endpoints**: `src/Api/Endpoints/`
- **Unit Tests**: `tests/Api.Tests/UnitTests/`
- **Frontend Features**: `src/web/features/`
- **Frontend UI**: `src/web/components/ui/`
- **API Client**: `src/web/lib/api/`

---

**Task Status**: Not Started
**Current Phase**: Phase 1
**Progress**: 0/[X] tasks completed
