# .NET Minimal API Patterns

## Purpose

Guide for building Minimal API endpoints in ASP.NET Core. All API endpoints follow a consistent structure: one static class per resource, organized under `Endpoints/`, using `MapGroup` for route prefixes.

## Rules

1. **One endpoint class per resource** — Each resource (e.g., Products, Orders) gets its own static class in `src/Api/Endpoints/`.
2. **Use `MapGroup` with route prefixes** — Group related endpoints under a shared prefix (e.g., `/api/products`).
3. **Request/Response DTOs** — Never expose domain entities directly. Define DTOs in the endpoint class or a shared `Contracts/` folder.
4. **Dependency injection via parameters** — Inject services directly into handler method parameters, not constructors.
5. **OpenAPI metadata is required** — Every endpoint must have `WithName()`, `WithTags()`, and `Produces<T>()` metadata.
6. **Error handling via `Results.Problem()`** — Use RFC 7807 Problem Details for all error responses.
7. **CORS for frontend dev server** — Configure CORS policy to allow the frontend dev server origin during development.

## Endpoint Group Organization

```
src/Api/
├── Endpoints/
│   ├── ProductEndpoints.cs
│   ├── OrderEndpoints.cs
│   └── UserEndpoints.cs
├── Program.cs
└── ...
```

## MapGroup Pattern

```csharp
public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", GetAll)
            .WithName("GetAllProducts")
            .Produces<List<ProductResponse>>(200);

        group.MapGet("/{id:int}", GetById)
            .WithName("GetProductById")
            .Produces<ProductResponse>(200)
            .Produces(404);

        group.MapPost("/", Create)
            .WithName("CreateProduct")
            .Produces<ProductResponse>(201)
            .ProducesValidationProblem();
    }
}
```

## Request/Response DTOs

```csharp
// Define DTOs close to where they're used
public record CreateProductRequest(string Name, decimal Price, string? Description);
public record ProductResponse(int Id, string Name, decimal Price, string? Description);
```

**Rules:**
- Use `record` types for DTOs (immutability by default).
- Suffix request types with `Request`, response types with `Response`.
- Never return domain entities from endpoints.

## Dependency Injection in Handlers

Inject services as method parameters — Minimal APIs resolve them automatically:

```csharp
private static async Task<IResult> GetAll(
    IProductService productService,
    CancellationToken cancellationToken)
{
    var products = await productService.GetAllAsync(cancellationToken);
    return Results.Ok(products.Select(p => p.ToResponse()));
}

private static async Task<IResult> Create(
    CreateProductRequest request,
    IProductService productService,
    CancellationToken cancellationToken)
{
    var product = await productService.CreateAsync(request.Name, request.Price, cancellationToken);
    return Results.Created($"/api/products/{product.Id}", product.ToResponse());
}
```

**Anti-pattern — Do NOT use constructors or fields:**
```csharp
// ❌ Wrong: Minimal API endpoint classes are static
public class ProductEndpoints
{
    private readonly IProductService _service;
    public ProductEndpoints(IProductService service) => _service = service;
}
```

## OpenAPI Metadata

Every endpoint must include:

```csharp
group.MapPut("/{id:int}", Update)
    .WithName("UpdateProduct")          // Required: unique operation ID
    .WithTags("Products")               // Required: grouping in Swagger UI
    .WithDescription("Updates a product by ID")  // Optional but recommended
    .Produces<ProductResponse>(200)     // Required: success response type
    .Produces(404)                      // Required: document error responses
    .ProducesValidationProblem();       // Required: if accepting input
```

## Error Handling with Results.Problem()

Use `Results.Problem()` for RFC 7807 Problem Details:

```csharp
private static async Task<IResult> GetById(
    int id,
    IProductService productService,
    CancellationToken cancellationToken)
{
    var product = await productService.GetByIdAsync(id, cancellationToken);

    if (product is null)
    {
        return Results.Problem(
            title: "Product not found",
            detail: $"No product exists with ID {id}",
            statusCode: 404);
    }

    return Results.Ok(product.ToResponse());
}
```

**Validation errors:**
```csharp
private static IResult ValidateRequest(CreateProductRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Name))
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["Name"] = ["Product name is required"]
        });

    return null!; // valid
}
```

## CORS Configuration

In `Program.cs`, configure CORS for the frontend dev server:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // Next.js dev server
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Apply after routing, before endpoints
app.UseCors("Frontend");
```

## Registration in Program.cs

```csharp
var app = builder.Build();

app.MapProductEndpoints();
app.MapOrderEndpoints();
app.MapUserEndpoints();

app.Run();
```

## Checklist

- [ ] Endpoint class is `static` and lives in `Endpoints/`
- [ ] Uses `MapGroup` with `/api/{resource}` prefix
- [ ] All handlers use DI via method parameters
- [ ] Request/Response DTOs defined (no entity exposure)
- [ ] `WithName()`, `WithTags()`, `Produces<T>()` on every endpoint
- [ ] Errors return `Results.Problem()` with proper status codes
- [ ] CORS configured for frontend dev server
