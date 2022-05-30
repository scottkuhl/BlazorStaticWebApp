using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Api.Functions;
using AzureStaticWebApp.Common.Shared.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Functions;

[Trait("Category", "Unit")]
public class UploadFunctionTests
{
    [Fact]
    public async Task Run_ShouldReturnBadRequestResult_WhenEmptyFileIsAttachedToRequest()
    {
        // Arrange
        var azureBlobStorageService = Substitute.For<IAzureBlobStorageService>();
        var guid = Substitute.For<IGuid>();
        var function = new UploadFunction(azureBlobStorageService, guid);
        var request = new DefaultHttpContext().Request;
        var formFile = Substitute.For<IFormFile>();
        request.Form = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection { formFile });

        // Act
        var result = await function.Run(request, new CancellationToken());

        // Assert
        var badRequestResult = result as BadRequestResult;
        _ = badRequestResult.Should().NotBeNull();
    }

    [Fact]
    public async Task Run_ShouldReturnBadRequestResult_WhenNoFileIsAttachedToRequest()
    {
        // Arrange
        var azureBlobStorageService = Substitute.For<IAzureBlobStorageService>();
        var guid = Substitute.For<IGuid>();
        var function = new UploadFunction(azureBlobStorageService, guid);
        var request = new DefaultHttpContext().Request;

        // Act
        var result = await function.Run(request, new CancellationToken());

        // Assert
        var badRequestResult = result as BadRequestResult;
        _ = badRequestResult.Should().NotBeNull();
    }
}