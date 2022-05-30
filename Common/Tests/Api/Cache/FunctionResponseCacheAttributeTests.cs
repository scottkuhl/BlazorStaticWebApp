using AzureStaticWebApp.Common.Api.Cache;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.ObjectModel;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Cache;

#pragma warning disable CS0618 // Type or member is obsolete

[Trait("Category", "Unit")]
public class FunctionResponseCacheAttributeTests
{
    [Theory]
    [InlineData(ResponseCacheLocation.Any, "public, max-age=100")]
    [InlineData(ResponseCacheLocation.Client, "public, max-age=100")]
    [InlineData(ResponseCacheLocation.None, "no-store, max-age=0")]
    public async Task OnExecutedAsync_ShouldSetCacheControlHeaderValues_WhenResponseCacheLocationIsAny(ResponseCacheLocation cacheLocation, string expectedResult)
    {
        // Arrange
        var request = new DefaultHttpContext().Request;
        var functionResponseCacheAttribute = new FunctionResponseCacheAttribute(100, cacheLocation);
        var functionExecutedContext = StubFunctionExecutedContext(new Dictionary<string, object> { { "first", request } });

        // Act
        await functionResponseCacheAttribute.OnExecutedAsync(functionExecutedContext, new CancellationToken());

        // Assert
        var actualResult = request.HttpContext.Response.Headers.First(x => x.Key == "Cache-Control").Value;
        _ = actualResult.Should().Equal(expectedResult);
    }

    [Fact]
    public async Task OnExecutedAsync_ShouldThrowApplicationException_WhenFunctionExecutedContextIsNotHttpRequest()
    {
        // Arrange
        var functionResponseCacheAttribute = new FunctionResponseCacheAttribute(100, ResponseCacheLocation.Any);
        var functionExecutedContext = StubFunctionExecutedContext(new Dictionary<string, object> { { "first", "not an HTTP Request object" } });

        // Act
        var exception = await Record.ExceptionAsync(() => functionResponseCacheAttribute.OnExecutedAsync(functionExecutedContext, new CancellationToken()));

        // Assert
        _ = exception.Should().BeOfType<ApplicationException>();
        _ = exception.Message.Should().StartWith("HttpRequest is null.");
    }

    private static FunctionExecutedContext StubFunctionExecutedContext(Dictionary<string, object> arguments)
    {
        var roArguments = new ReadOnlyDictionary<string, object>(arguments);
        var properties = new Dictionary<string, object>();
        var logger = Substitute.For<ILogger>();
        var functionResult = new FunctionResult(true);

        return new FunctionExecutedContext(roArguments, properties, Guid.NewGuid(), "test", logger, functionResult);
    }
}