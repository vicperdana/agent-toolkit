# .NET Testing Guide

## Purpose

Testing conventions for this project. We follow Test-Driven Development (TDD): write tests BEFORE implementation. All tests use xUnit with Arrange-Act-Assert pattern.

## Constitutional Principle

**Test-First** — Write tests before implementation. Unit tests in `UnitTests/`, integration tests in `IntegrationTests/`. Red → Green → Refactor.

## Project Structure

```
tests/
└── Api.Tests/
    ├── UnitTests/
    │   ├── Services/
    │   │   └── ProductServiceTests.cs
    │   └── Entities/
    │       └── ProductTests.cs
    ├── IntegrationTests/
    │   └── Endpoints/
    │       └── ProductEndpointTests.cs
    └── Api.Tests.csproj        # References both Api and Shared
```

## Test Naming Convention

Format: `MethodName_Scenario_ExpectedResult`

```csharp
public class ProductServiceTests
{
    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    [Fact]
    public async Task GetByIdAsync_NonExistentProduct_ReturnsNull()
    [Fact]
    public async Task CreateAsync_ValidInput_ReturnsNewProduct()
    [Fact]
    public async Task CreateAsync_NegativePrice_ThrowsArgumentException()
}
```

**Rules:**
- Method name first, then scenario, then expected result.
- Use underscores to separate the three parts.
- Be specific about the scenario and expectation.

## xUnit Conventions

### [Fact] — Single test case

```csharp
[Fact]
public async Task GetAllAsync_WhenProductsExist_ReturnsAllProducts()
{
    // Arrange
    var service = CreateService(withProducts: 3);

    // Act
    var result = await service.GetAllAsync();

    // Assert
    Assert.Equal(3, result.Count);
}
```

### [Theory] + [InlineData] — Parameterized tests

```csharp
[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(-100.50)]
public async Task CreateAsync_InvalidPrice_ThrowsArgumentException(decimal price)
{
    // Arrange
    var service = CreateService();

    // Act & Assert
    await Assert.ThrowsAsync<ArgumentException>(
        () => service.CreateAsync("Test Product", price));
}
```

### [Theory] + [MemberData] — Complex test data

```csharp
public static IEnumerable<object[]> InvalidProductNames =>
[
    [null!],
    [""],
    ["   "],
    [new string('x', 501)]  // exceeds max length
];

[Theory]
[MemberData(nameof(InvalidProductNames))]
public async Task CreateAsync_InvalidName_ThrowsArgumentException(string name)
{
    var service = CreateService();
    await Assert.ThrowsAsync<ArgumentException>(
        () => service.CreateAsync(name, 9.99m));
}
```

## Arrange-Act-Assert Pattern

Every test follows this structure. Use comments to mark each section:

```csharp
[Fact]
public void ApplyDiscount_ValidPercentage_ReducesPrice()
{
    // Arrange
    var product = new Product { Name = "Widget", Price = 100m };

    // Act
    product.ApplyDiscount(25);

    // Assert
    Assert.Equal(75m, product.Price);
}
```

**Rules:**
- Always include `// Arrange`, `// Act`, `// Assert` comments.
- One logical action in Act.
- Prefer specific assertions (`Assert.Equal`, `Assert.Contains`) over `Assert.True`.
- One concept per test — don't test multiple behaviors.

## Entity Tests (Unit)

Test domain logic on entities directly:

```csharp
public class ProductTests
{
    [Fact]
    public void ApplyDiscount_ZeroPercent_PriceUnchanged()
    {
        // Arrange
        var product = new Product { Name = "Widget", Price = 50m };

        // Act
        product.ApplyDiscount(0);

        // Assert
        Assert.Equal(50m, product.Price);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void ApplyDiscount_OutOfRange_ThrowsArgumentOutOfRangeException(decimal percentage)
    {
        // Arrange
        var product = new Product { Name = "Widget", Price = 50m };

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => product.ApplyDiscount(percentage));
    }
}
```

## Service Tests (Unit)

Test services with mocked dependencies:

```csharp
public class ProductServiceTests
{
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        // Setup shared test fixtures
        _sut = new ProductService();
    }

    [Fact]
    public async Task CreateAsync_ValidInput_SetsCreatedAtToUtcNow()
    {
        // Arrange
        var before = DateTime.UtcNow;

        // Act
        var product = await _sut.CreateAsync("Widget", 9.99m);

        // Assert
        Assert.InRange(product.CreatedAt, before, DateTime.UtcNow);
    }
}
```

**Naming convention for the system under test:** Use `_sut` (system under test) for the class being tested.

## Integration Tests (Endpoints)

Test the full HTTP pipeline using `WebApplicationFactory`:

```csharp
public class ProductEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_ReturnsOkWithProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponse>>();
        Assert.NotNull(products);
    }

    [Fact]
    public async Task GetProduct_NonExistentId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/products/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
```

## TDD Workflow

1. **Red** — Write a failing test for the desired behavior.
2. **Green** — Write the minimum code to make the test pass.
3. **Refactor** — Clean up while keeping tests green.

```bash
# Step 1: Write test, verify it fails
dotnet test --filter "ProductServiceTests.CreateAsync_ValidInput_ReturnsNewProduct"

# Step 2: Implement the minimum code
# Step 3: Verify test passes
dotnet test --filter "ProductServiceTests.CreateAsync_ValidInput_ReturnsNewProduct"

# Step 4: Run all tests to ensure nothing broke
dotnet test
```

## Build Validation Commands

Run from solution root before every commit:

```bash
# Build — must produce zero warnings
dotnet build --warnaserrors

# Test — all tests must pass
dotnet test

# Format — code must conform to .editorconfig
dotnet format --verify-no-changes
```

## Checklist

- [ ] Tests written BEFORE implementation (TDD)
- [ ] Test name follows `MethodName_Scenario_ExpectedResult`
- [ ] Each test has Arrange-Act-Assert comments
- [ ] Unit tests in `UnitTests/`, integration tests in `IntegrationTests/`
- [ ] `[Fact]` for single cases, `[Theory]` for parameterized
- [ ] `_sut` naming for system under test
- [ ] `dotnet build` — zero warnings
- [ ] `dotnet test` — all green
- [ ] `dotnet format --verify-no-changes` — clean formatting
