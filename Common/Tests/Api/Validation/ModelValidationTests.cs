using AzureStaticWebApp.Common.Api.Validation;
using AzureStaticWebApp.Common.Shared;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Text.Json;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Validation;

[Trait("Category", "Unit")]
public class ModelValidationTests
{
    [Fact]
    public void Validate_ShouldValidateModel_GivenSerializableClass()
    {
        // Act
        var request = new DefaultHttpContext().Request;
        var model = JsonSerializer.Serialize(new TestModel());
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(model));

        // Arrange
        var results = request.Validate<TestModel>();

        // Assert
        _ = results.Should().NotBeNull();
    }

    private class TestModel : Model
    { }
}