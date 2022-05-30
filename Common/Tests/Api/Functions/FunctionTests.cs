using AzureStaticWebApp.Common.Api.Functions;
using AzureStaticWebApp.Common.Shared.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Runtime.ExceptionServices;
using Xunit;

#pragma warning disable CS0618 // Type or member is obsolete

namespace AzureStaticWebApp.Common.Tests.Api.Functions;

[Trait("Category", "Unit")]
public class FunctionTests
{
    [Fact]
    public void GetParametersFromQuery_ShouldGetParameters_WhenPassedIntoQueryString()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.QueryString = new QueryString("?pageNumber=1&pageSize=2&search=3&orderBy=4");

        // Act
        var parameters = TestFunction.TestGetParametersFromQuery(context.Request);

        // Assert
        _ = parameters.Should().NotBeNull();
        _ = parameters.PageNumber.Should().Be(1);
        _ = parameters.PageSize.Should().Be(2);
        _ = parameters.Search.Should().Be("3");
        _ = parameters.OrderBy.Should().Be("4");
    }

    [Fact]
    public void GetVirtualParametersFromQuery_ShouldGetParameters_WhenPassedIntoQueryString()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.QueryString = new QueryString("?startIndex=1&pageSize=2&search=3&orderBy=4");

        // Act
        var parameters = TestFunction.TestGetVirtualParametersFromQuery(context.Request);

        // Assert
        _ = parameters.Should().NotBeNull();
        _ = parameters.StartIndex.Should().Be(1);
        _ = parameters.PageSize.Should().Be(2);
        _ = parameters.Search.Should().Be("3");
        _ = parameters.OrderBy.Should().Be("4");
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldLogMessage()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        _ = httpContextAccessor.HttpContext.Returns(context);
        var logger = Substitute.For<ILogger<Function>>();
        var function = new TestFunction(httpContextAccessor, logger);
        var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(new Exception());
        var functionExceptionContext = new FunctionExceptionContext(Guid.NewGuid(), "test", logger, exceptionDispatchInfo, new Dictionary<string, object>());

        // Act
        await function.OnExceptionAsync(functionExceptionContext, new CancellationToken());

        // Assert
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        logger.Received().Log(LogLevel.Error, Arg.Any<EventId>(), Arg.Is<object>(o => o.ToString().StartsWith("Something went wrong:")), Arg.Any<Exception>(), Arg.Any<Func<object, Exception, string>>());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    }

    [Fact]
    public async Task OnExceptionAsync_ShouldReturnErrorStatus()
    {
        // Arrange
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        _ = httpContextAccessor.HttpContext.Returns(context);
        var logger = Substitute.For<ILogger<Function>>();
        var function = new TestFunction(httpContextAccessor, logger);
        var exceptionDispatchInfo = ExceptionDispatchInfo.Capture(new Exception());
        var functionExceptionContext = new FunctionExceptionContext(Guid.NewGuid(), "test", logger, exceptionDispatchInfo, new Dictionary<string, object>());

        // Act
        await function.OnExceptionAsync(functionExceptionContext, new CancellationToken());

        // Assert
        _ = context.Response.StatusCode.Should().Be(500);
    }

    private class TestFunction : Function
    {
        public TestFunction(IHttpContextAccessor httpContextAccessor, ILogger<Function> logger) : base(httpContextAccessor, logger)
        { }

        public static Parameters TestGetParametersFromQuery(HttpRequest request)
        {
            return GetParametersFromQuery(request);
        }

        public static VirtualParameters TestGetVirtualParametersFromQuery(HttpRequest request)
        {
            return GetVirtualParametersFromQuery(request);
        }
    }
}