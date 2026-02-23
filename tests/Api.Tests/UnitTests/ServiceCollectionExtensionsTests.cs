using Microsoft.Extensions.DependencyInjection;
using Shared.Extensions;

namespace Api.Tests.UnitTests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSharedServices_ReturnsServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddSharedServices();

        // Assert
        Assert.Same(services, result);
    }
}
