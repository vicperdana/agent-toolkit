# Clean Architecture Guide

## Purpose

Enforce simplified Clean Architecture in this project. Domain logic lives in Shared, API concerns live in Api. No unnecessary abstractions.

## Constitutional Principles

1. **Shared-First** — All domain logic starts in `src/Shared/`. No business rules in the Api project without first being defined in Shared.
2. **Simplicity** — Maximum 3 projects: Api, Shared, Api.Tests. No speculative features.
3. **Anti-Abstraction** — Use ASP.NET Core features directly. No repository pattern, no mediator, no CQRS unless the requirement explicitly demands it.

## Project Structure

```
src/
├── Api/                    # ASP.NET Core Minimal API (entry point)
│   ├── Endpoints/          # One static class per resource
│   ├── Program.cs          # Host configuration, DI, middleware
│   └── Api.csproj          # References Shared only
├── Shared/                 # Domain library (no external deps)
│   ├── Entities/           # Domain entities
│   ├── Interfaces/         # Service contracts
│   ├── Extensions/         # Extension methods
│   ├── Services/           # Domain service implementations
│   └── Shared.csproj       # No deps beyond Microsoft.Extensions.*
tests/
└── Api.Tests/              # Unit + integration tests
    └── Api.Tests.csproj    # References both Api and Shared
```

## Shared Project Rules

The Shared project is the core of the application. It contains all domain logic.

### Allowed Dependencies
- `Microsoft.Extensions.DependencyInjection.Abstractions`
- `Microsoft.Extensions.Logging.Abstractions`
- `Microsoft.Extensions.Options`
- **Nothing else.** No EF Core, no HTTP clients, no ASP.NET Core packages.

### Entities

One class per domain concept. Keep entities simple:

```csharp
namespace Shared.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Domain logic belongs here
    public void ApplyDiscount(decimal percentage)
    {
        if (percentage is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage));

        Price -= Price * (percentage / 100m);
    }
}
```

**Rules:**
- One entity class per domain concept.
- Domain behavior belongs on the entity, not in a service.
- Use `required` for mandatory properties.
- No data annotations — validation happens in the service layer.

### Interfaces

All service contracts live in `Shared/Interfaces/`. Interface names start with `I`:

```csharp
namespace Shared.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(string name, decimal price, CancellationToken cancellationToken = default);
}
```

**Rules:**
- Interface names start with `I`.
- All async methods accept `CancellationToken`.
- Return `IReadOnlyList<T>` for collections, not `List<T>`.
- Return nullable `T?` for single-item lookups.

### Service Registration

Register all Shared services via a single extension method:

```csharp
namespace Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        return services;
    }
}
```

This is called from `Program.cs`:

```csharp
builder.Services.AddSharedServices();
```

## Api Project Rules

The Api project is a thin layer. It handles HTTP concerns only.

### Allowed in Api
- Endpoint definitions (route mapping, request/response DTOs)
- Middleware configuration
- DI container setup
- Authentication/Authorization configuration
- CORS, OpenAPI, logging configuration

### NOT Allowed in Api
- Business rules or domain logic
- Data validation beyond request format
- Direct database access (use Shared services)
- Entity definitions

### Program.cs Pattern

```csharp
var builder = WebApplication.CreateBuilder(args);

// Register Shared services
builder.Services.AddSharedServices();

// Register Api-specific services
builder.Services.AddOpenApi();
builder.Services.AddCors(/* ... */);

var app = builder.Build();

// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("Frontend");
app.MapOpenApi();

// Map endpoints
app.MapProductEndpoints();
app.MapOrderEndpoints();

app.Run();
```

## Anti-Patterns to Avoid

### ❌ Repository Pattern
```csharp
// Don't do this — use EF Core DbContext directly in services
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
}
```

### ❌ Mediator / CQRS
```csharp
// Don't do this — call services directly from endpoints
public record GetProductQuery(int Id) : IRequest<Product>;
```

### ❌ Business Logic in Api
```csharp
// Don't do this — domain logic belongs in Shared
app.MapPost("/api/products", (CreateProductRequest req) =>
{
    if (req.Price < 0) req = req with { Price = 0 }; // ❌ Business rule in endpoint
});
```

### ✅ Correct Approach
```csharp
// Endpoint delegates to Shared service
app.MapPost("/api/products", async (CreateProductRequest req, IProductService svc) =>
{
    var product = await svc.CreateAsync(req.Name, req.Price); // ✅ Logic in Shared
    return Results.Created($"/api/products/{product.Id}", product.ToResponse());
});
```

## Dependency Direction

```
Api → Shared     ✅ (Api depends on Shared)
Shared → Api     ❌ (Shared must NEVER reference Api)
Api.Tests → Both ✅ (Tests can reference everything)
```

## Checklist

- [ ] All domain entities live in `Shared/Entities/`
- [ ] All interfaces live in `Shared/Interfaces/` and start with `I`
- [ ] No external dependencies in Shared beyond `Microsoft.Extensions.*`
- [ ] Service registration via `AddSharedServices()` extension
- [ ] Api project contains only HTTP/infrastructure concerns
- [ ] No repository pattern, mediator, or CQRS
- [ ] One entity class per domain concept
- [ ] Dependency direction: Api → Shared (never reverse)
