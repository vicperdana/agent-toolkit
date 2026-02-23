# [Feature Name] Design Document

## Overview

[Brief description of the feature and how it integrates with the existing Agent Toolkit architecture. Reference requirements from `requirements.md`.]

## Architecture

### Component Architecture

```
┌─────────────────────────────────────┐
│        Frontend (src/web/)          │
│  ┌──────────┐    ┌──────────────┐  │
│  │  Pages    │    │  Components  │  │
│  │  (app/)   │───▶│  (ui/blocks) │  │
│  └────┬─────┘    └──────────────┘  │
│       │ fetch /api/*                │
├───────┼─────────────────────────────┤
│       ▼                             │
│     Backend API (src/Api/)          │
│  ┌──────────┐    ┌──────────────┐  │
│  │ Endpoints │───▶│  Services    │  │
│  └────┬─────┘    └──────┬───────┘  │
├───────┼─────────────────┼───────────┤
│       ▼                 ▼           │
│     Shared (src/Shared/)            │
│  ┌──────────┐  ┌────────────────┐  │
│  │ Entities  │  │  Interfaces   │  │
│  └──────────┘  └────────────────┘  │
└─────────────────────────────────────┘
```

### Technology Choices

- **API Style**: [Minimal API endpoint groups]
- **Rendering**: [SSR / Client components — justify if interactive]
- **State**: [Zustand / React state / server state]
- **Data**: [In-memory / database — specify if needed]

## Backend Design

### Domain Entities (`src/Shared/Entities/`)

```csharp
namespace Shared.Entities;

public class [EntityName]
{
    public int Id { get; set; }
    // Define properties
}
```

### Service Interfaces (`src/Shared/Interfaces/`)

```csharp
namespace Shared.Interfaces;

public interface I[ServiceName]
{
    Task<IReadOnlyList<[Entity]>> GetAllAsync();
    Task<[Entity]?> GetByIdAsync(int id);
}
```

### API Endpoints (`src/Api/Endpoints/`)

```csharp
namespace Api.Endpoints;

public static class [Resource]Endpoints
{
    public static void Map[Resource]Endpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/[resource]").WithTags("[Resource]");

        group.MapGet("/", async (I[Service] service) =>
            Results.Ok(await service.GetAllAsync()));
    }
}
```

## Frontend Design

### Feature Module (`src/web/features/[feature]/`)

```
features/[feature]/
├── [feature]-page.tsx          # Page component
├── [feature]-store.ts          # Zustand store (if needed)
└── components/                 # Feature-specific components
```

### API Client (`src/web/lib/api/`)

```typescript
export async function get[Resources](): Promise<[Resource][]> {
  const res = await fetch("/api/[resource]");
  if (!res.ok) throw new Error("Failed to fetch");
  return res.json();
}
```

## Testing Strategy

### Backend Unit Tests (`tests/Api.Tests/UnitTests/`)

```csharp
public class [ServiceName]Tests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllItems()
    {
        // Arrange → Act → Assert
    }
}
```

### Frontend Tests (if applicable)

- Component rendering tests
- API client mock tests

---

**Requirements Traceability**: This design addresses requirements from `requirements.md`
**Document Status**: Draft
**Approval**: [ ] Approved — proceed to Tasks phase
