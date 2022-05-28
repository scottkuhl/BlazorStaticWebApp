using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

namespace AzureStaticWebApp.Common.Api.Data;

public interface IAzureTableStorageRepository<T> where T : AzureTableEntity
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken);

    Task DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken);

    Task<T> SaveAsync(T entity, CancellationToken cancellationToken);

    Task<T> UpdateAsync(T entity, Azure.ETag? etag, CancellationToken cancellationToken);
}

public class AzureTableStorageRepository<T> : IAzureTableStorageRepository<T> where T : AzureTableEntity
{
    protected readonly TableClient _tableClient;
    private readonly TableServiceClient _serviceClient;

    public AzureTableStorageRepository(IConfiguration configuration)
    {
        _serviceClient = new TableServiceClient(configuration["AzureStorage"]);

        _tableClient = _serviceClient.GetTableClient(GetContainerName());
        _tableClient.CreateIfNotExistsAsync().Wait();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        _ = await _tableClient.AddEntityAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public Task DeleteAsync(string partitionKey, string rowKey, CancellationToken cancellationToken)
    {
        return _tableClient.DeleteEntityAsync(partitionKey, rowKey, cancellationToken: cancellationToken);
    }

    public async Task<T> SaveAsync(T entity, CancellationToken cancellationToken)
    {
        _ = await _tableClient.UpsertEntityAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task<T> UpdateAsync(T entity, Azure.ETag? etag, CancellationToken cancellationToken)
    {
        var etagParam = etag ?? Azure.ETag.All;

        _ = await _tableClient.UpdateEntityAsync(entity, etagParam, cancellationToken: cancellationToken);
        return entity;
    }

    private static string GetContainerName()
    {
        return typeof(T).Name.Replace("Entity", "");
    }
}