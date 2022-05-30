using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Shared.Services;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Trait("Category", "Unit")]
public class AzureTableEntityTests
{
    private readonly AzureTableEntity _entity;

    public AzureTableEntityTests()
    {
        var dateTime = Substitute.For<IDateTime>();
        _ = dateTime.UtcNow.Returns(new DateTime(2022, 1, 1));
        _entity = new AzureTableEntity(dateTime);
    }

    [Fact]
    public void PartitionKey_ShouldDefaultToEmpty_WhenInitialized()
    {
        // Act
        var actual = _entity.PartitionKey;

        // Assert
        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void RowKey_ShouldDefaultToEmpty_WhenInitialized()
    {
        // Act
        var actual = _entity.RowKey;

        // Assert
        _ = actual.Should().BeEmpty();
    }

    [Fact]
    public void Timestamp_ShouldDefaultToEmpty_WhenInitialized()
    {
        // Act
        var actual = _entity.Timestamp;

        // Assert
        _ = actual.Should().Be(new DateTime(2022, 1, 1));
    }
}