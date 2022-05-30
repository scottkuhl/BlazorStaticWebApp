using AzureStaticWebApp.Common.Api.Data;
using FluentAssertions;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Trait("Category", "Unit")]
public class CosmosEntityTests
{
    [Fact]
    public void Id_ShouldDefaultToEmpty_WhenInitialized()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        var actual = entity.id;

        // Assert
        _ = actual.Should().BeEmpty();
    }

    private class TestEntity : CosmosEntity
    { }
}