using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureStaticWebApp.Common.Api.Data;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Collection("Integration")]
[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Category", "Integration")]
public class AzureStorageServiceTests
{
    private const string AzureBlobContainer = "file-container";
    private const string AzureStorage = "UseDevelopmentStorage=true";
    private const string FileLocation = "Assets\\" + FileName;
    private const string FileName = "Test.txt";

    [Fact]
    public async Task DeleteFileAsync_ShouldDeleteFile_WhenFileExists()
    {
        // Arrange
        var blob = await PrepBlobClientAsync();
        var fileStream = File.OpenRead(FileLocation);
        _ = await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "text/plain" });
        var azureBlobStorageService = StubAzureBlobStorageService();

        // Act
        await azureBlobStorageService.DeleteFileAsync(FileName, new CancellationToken());

        // Assert
        _ = blob.Exists().Value.Should().Be(false);
    }

    [Fact]
    public async Task UploadFileAsync_ShouldUploadFile()
    {
        // Arrange
        var blob = await PrepBlobClientAsync();
        var fileStream = File.OpenRead(FileLocation);
        var azureBlobStorageService = StubAzureBlobStorageService();

        // Act
        var uri = await azureBlobStorageService.UploadFileAsync(fileStream, FileName, "text/plain", new CancellationToken());

        // Assert
        _ = uri.Should().NotBeEmpty();
        _ = blob.Exists().Value.Should().Be(true);
    }

    private static async Task<BlobClient> PrepBlobClientAsync()
    {
        var container = new BlobContainerClient(AzureStorage, AzureBlobContainer);
        var blob = container.GetBlobClient(FileName);
        _ = await blob.DeleteIfExistsAsync();
        return blob;
    }

    private static AzureBlobStorageService StubAzureBlobStorageService()
    {
        var settings = new Dictionary<string, string>
        {
            { nameof(AzureStorage), AzureStorage },
            { nameof(AzureBlobContainer), AzureBlobContainer }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new AzureBlobStorageService(configuration);
    }
}