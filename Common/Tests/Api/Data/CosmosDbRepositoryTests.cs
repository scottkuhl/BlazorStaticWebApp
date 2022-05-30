using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Shared.Requests;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace AzureStaticWebApp.Common.Tests.Api.Data;

[Collection("Integration")]
[CollectionDefinition("Integration", DisableParallelization = true)]
[Trait("Category", "Integration")]
public class CosmosDbRepositoryTests
{
    private const string CosmosDb_Account = "https://localhost:8081";
    private const string CosmosDb_Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
    private const string DatabaseName = "AzureStaticWebApp";
    private const string PartitionKey = "id";

    [Fact]
    public async Task AddAsync_ShouldAddData()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };

        // Act
        await cosmosDbRepository.AddAsync(testEntity, testEntity.id, new CancellationToken());

        // Assert
        var entityInRepo = await container.ReadItemAsync<TestEntity>(testEntity.id, new PartitionKey(testEntity.id));
        _ = entityInRepo.Should().NotBeNull();
        _ = entityInRepo.Resource.Should().NotBeNull();
        _ = entityInRepo.Resource.id.Should().Be("1");
    }

    [Fact]
    [SuppressMessage("AsyncUsage", "AsyncFixer02:Long-running or blocking operations inside an async method", Justification = "Testing for exception")]
    public async Task DeleteAsync_ShouldDeleteData()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));

        // Act
        await cosmosDbRepository.DeleteAsync(testEntity.id, testEntity.id, new CancellationToken());

        // Assert
        var exception = Record.ExceptionAsync(async () => await container.ReadItemAsync<TestEntity>(testEntity.id, new PartitionKey(testEntity.id)));
        _ = exception.Result.Should().NotBeNull();
        _ = exception.Result.Should().BeOfType<CosmosException>();
    }

    [Fact]
    public async Task GetAsync_ShouldReturnEntity_WhenEntityExists()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));

        // Act
        var entity = await cosmosDbRepository.GetAsync(testEntity.id, testEntity.id, new CancellationToken());

        // Assert
        _ = entity.Should().NotBeNull();
    }

    [Fact]
    public async Task InitializeCollection_ShouldCreateContainer()
    {
        // Arrange
        var container = await PrepContainerAsync();
        _ = await container.DeleteContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();

        // Act
        cosmosDbRepository.InitializeCollection(PartitionKey);

        // Assert
        var client = new CosmosClient(CosmosDb_Account, CosmosDb_Key);
        container = client.GetContainer(DatabaseName, "Test");
        _ = container.Should().NotBeNull();
    }

    [Fact]
    public async Task InitializeCollection_ShouldCreateContainerWithCompositeIndexes_WhenCompositeIndexesAreProvided()
    {
        // Arrange
        var container = await PrepContainerAsync();
        _ = await container.DeleteContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var compositeIndexes = new List<Collection<CompositePath>>
        {
            new Collection<CompositePath>
            {
                new CompositePath { Path = "/id", Order = CompositePathSortOrder.Descending },
                new CompositePath { Path = "/Name", Order = CompositePathSortOrder.Ascending }
            }
        };

        // Act
        cosmosDbRepository.InitializeCollection(PartitionKey, compositeIndexes: compositeIndexes);

        // Assert
        var client = new CosmosClient(CosmosDb_Account, CosmosDb_Key);
        container = client.GetContainer(DatabaseName, "Test");
        _ = container.Should().NotBeNull();
    }

    [Fact]
    public async Task InitializeCollection_ShouldCreateContainerWithUniqueKeyPaths_WhenUniqueKeyPathsAreProvided()
    {
        // Arrange
        var container = await PrepContainerAsync();
        _ = await container.DeleteContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var uniqueKeyPaths = new[] { "/Name" };

        // Act
        cosmosDbRepository.InitializeCollection(PartitionKey, uniqueKeyPaths);

        // Assert
        var client = new CosmosClient(CosmosDb_Account, CosmosDb_Key);
        container = client.GetContainer(DatabaseName, "Test");
        _ = container.Should().NotBeNull();
    }

    [Fact]
    public async Task ListAsync_ShouldNotReturnNonMatchingEntities_WhenWhereExpressionIsGiven()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1", Name = "No" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        var parameters = new Parameters();

        // Act
        var entities = await cosmosDbRepository.ListAsync(new Parameters(), new CancellationToken(), where: x => x.Name == "Yes");

        // Assert
        _ = entities.Count.Should().Be(0);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));

        // Act
        var entities = await cosmosDbRepository.ListAsync(new Parameters(), new CancellationToken());

        // Assert
        _ = entities.Count.Should().Be(1);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnAllEntities_WhenQueryIsGiven()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        var query = container.GetItemLinqQueryable<TestEntity>();
        _ = query.Select(x => new { x.id, x.Name });

        // Act
        var entities = await cosmosDbRepository.ListAsync(new Parameters(), new CancellationToken(), query: query);

        // Assert
        _ = entities.Count.Should().Be(1);
    }

    [Fact]
    public async Task ListAsync_ShouldReturnMatchingEntities_WhenWhereExpressionIsGiven()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1", Name = "Yes" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        var parameters = new Parameters();

        // Act
        var entities = await cosmosDbRepository.ListAsync(new Parameters(), new CancellationToken(), where: x => x.Name == "Yes");

        // Assert
        _ = entities.Count.Should().Be(1);
    }

    [Fact]
    public async Task ListVirtualAsync_ShouldNotReturnNonMatchingEntities_WhenWhereExpressionIsGiven()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1", Name = "No" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        var parameters = new Parameters();

        // Act
        var entities = await cosmosDbRepository.ListVirtualAsync(new VirtualParameters(), new CancellationToken(), where: x => x.Name == "Yes");

        // Assert
        _ = entities.Items.Count.Should().Be(0);
    }

    [Fact]
    public async Task ListVirtualAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));

        // Act
        var entities = await cosmosDbRepository.ListVirtualAsync(new VirtualParameters(), new CancellationToken());

        // Assert
        _ = entities.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task ListVirtualAsync_ShouldReturnAllEntities_WhenQueryIsGiven()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        var query = container.GetItemLinqQueryable<TestEntity>();
        _ = query.Select(x => new { x.id, x.Name });

        // Act
        var entities = await cosmosDbRepository.ListVirtualAsync(new VirtualParameters(), new CancellationToken(), query: query);

        // Assert
        _ = entities.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task ListVirtualAsync_ShouldReturnMatchingEntities_WhenWhereExpressionIsGiven()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1", Name = "Yes" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        var parameters = new Parameters();

        // Act
        var entities = await cosmosDbRepository.ListVirtualAsync(new VirtualParameters(), new CancellationToken(), where: x => x.Name == "Yes");

        // Assert
        _ = entities.Items.Count.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ShouldAddData()
    {
        // Arrange
        var container = await PrepContainerAsync();
        var cosmosDbRepository = StubCosmosDbRepository();
        var testEntity = new TestEntity { id = "1" };
        _ = await container.CreateItemAsync(testEntity, new PartitionKey(testEntity.id));
        testEntity.Name = "Changed";

        // Act
        await cosmosDbRepository.UpdateAsync(testEntity, testEntity.id, new CancellationToken());

        // Assert
        var entityInRepo = await container.ReadItemAsync<TestEntity>(testEntity.id, new PartitionKey(testEntity.id));
        _ = entityInRepo.Should().NotBeNull();
        _ = entityInRepo.Resource.Should().NotBeNull();
        _ = entityInRepo.Resource.Name.Should().Be("Changed");
    }

    [SuppressMessage("Roslynator", "RCS1075:Avoid empty catch clause that catches System.Exception.", Justification = "Forced deletion.")]
    private static async Task<Container> PrepContainerAsync()
    {
        var client = new CosmosClient(CosmosDb_Account, CosmosDb_Key);
        var database = await client.CreateDatabaseIfNotExistsAsync(DatabaseName);
        var properties = new ContainerProperties("Test", $"/{PartitionKey}");
        _ = await database.Database.CreateContainerIfNotExistsAsync(properties);
        var container = client.GetContainer(DatabaseName, "Test");
        try
        {
            _ = await container.DeleteItemAsync<TestEntity>("1", new PartitionKey("1"));
        }
        catch (Exception) { }

        return container;
    }

    private static CosmosDbRepository<TestEntity> StubCosmosDbRepository()
    {
        var settings = new Dictionary<string, string>
        {
            { nameof(CosmosDb_Account), CosmosDb_Account },
            { nameof(CosmosDb_Key), CosmosDb_Key }
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        return new CosmosDbRepository<TestEntity>(configuration);
    }

    private class TestEntity : CosmosEntity
    {
        public string Name { get; set; } = string.Empty;
    }
}