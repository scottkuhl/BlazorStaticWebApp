using AzureStaticWebApp.Common.Api.Exceptions;
using AzureStaticWebApp.Common.Shared;
using FluentAssertions;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Exceptions;

[Trait("Category", "Unit")]
public class NotFoundExceptionTests
{
    [Fact]
    public void NotFoundException_ShouldReturnHumanReadableMessage_WhenIdGiven()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var exception = new NotFoundException<Model>(id);

        // Assert
        _ = exception.Message.Should().StartWith("The Model with id:");
    }
}