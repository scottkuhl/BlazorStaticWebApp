using Azure.Data.Tables;
using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Shared.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Collection("Integration")]
[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Category", "Integration")]
public class AzureTableStorageRepositoryTests
{
    private const string AzureStorage = "UseDevelopmentStorage=true";
    private const string TestValue = "Test";

    [Fact]
    public async Task AddAsync_ShouldAddData()
    {
        // Arrange
        var tableClient = await PrepTableClientAsync();
        var azureTableStorageRepository = StubAzureTableStorageRepository();

        // Act
        var entity = await azureTableStorageRepository.AddAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue }, new CancellationToken());

        // Assert
        _ = entity.Should().NotBeNull();
        var entityInRepo = await tableClient.GetEntityAsync<TestEntity>(TestValue, TestValue);
        _ = entityInRepo.Should().NotBeNull();
        _ = entityInRepo.Value.Id.Should().Be(1);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteData()
    {
        // Arrange
        var tableClient = await PrepTableClientAsync();
        var azureTableStorageRepository = StubAzureTableStorageRepository();
        _ = await tableClient.AddEntityAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue });

        // Act
        await azureTableStorageRepository.DeleteAsync(TestValue, TestValue, new CancellationToken());

        // Assert
        var exception = Record.ExceptionAsync(async () => await tableClient.GetEntityAsync<TestEntity>(TestValue, TestValue));
        _ = exception.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveAsync_ShouldAddData_WhenEntityDoesNotExist()
    {
        // Arrange
        var tableClient = await PrepTableClientAsync();
        var azureTableStorageRepository = StubAzureTableStorageRepository();

        // Act
        var entity = await azureTableStorageRepository.SaveAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue }, new CancellationToken());

        // Assert
        _ = entity.Should().NotBeNull();
        var entityInRepo = await tableClient.GetEntityAsync<TestEntity>(TestValue, TestValue);
        _ = entityInRepo.Should().NotBeNull();
        _ = entityInRepo.Value.Id.Should().Be(1);
    }

    [Fact]
    public async Task SaveAsync_ShouldUpdateData_WhenEntityDoesExist()
    {
        // Arrange
        var tableClient = await PrepTableClientAsync();
        var azureTableStorageRepository = StubAzureTableStorageRepository();
        _ = await tableClient.AddEntityAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue });

        // Act
        var entity = await azureTableStorageRepository.SaveAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue, Id = 2 }, new CancellationToken());

        // Assert
        _ = entity.Should().NotBeNull();
        var entityInRepo = await tableClient.GetEntityAsync<TestEntity>(TestValue, TestValue);
        _ = entityInRepo.Should().NotBeNull();
        _ = entityInRepo.Value.Id.Should().Be(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateData_WhenEntityDoesExist()
    {
        // Arrange
        var tableClient = await PrepTableClientAsync();
        var azureTableStorageRepository = StubAzureTableStorageRepository();
        _ = await tableClient.AddEntityAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue });

        // Act
        var entity = await azureTableStorageRepository.UpdateAsync(new TestEntity { PartitionKey = TestValue, RowKey = TestValue, Id = 2 }, null, new CancellationToken());

        // Assert
        _ = entity.Should().NotBeNull();
        var entityInRepo = await tableClient.GetEntityAsync<TestEntity>(TestValue, TestValue);
        _ = entityInRepo.Should().NotBeNull();
        _ = entityInRepo.Value.Id.Should().Be(2);
    }

    private static async Task<TableClient> PrepTableClientAsync()
    {
        var tableServiceClient = new TableServiceClient(AzureStorage);
        var tableClient = tableServiceClient.GetTableClient(TestValue);
        _ = await tableClient.DeleteEntityAsync(TestValue, TestValue);
        return tableClient;
    }

    private static AzureTableStorageRepository<TestEntity> StubAzureTableStorageRepository()
    {
        var settings = new Dictionary<string, string>
        {
            { nameof(AzureStorage), AzureStorage }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new AzureTableStorageRepository<TestEntity>(configuration);
    }

    private class TestEntity : AzureTableEntity
    {
        public TestEntity() : base(Substitute.For<IDateTime>())
        {
        }

        public int Id { get; set; } = 1;
    }
}