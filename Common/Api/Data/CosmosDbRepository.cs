using AzureStaticWebApp.Common.Shared.Requests;
using AzureStaticWebApp.Common.Shared.Responses;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;

namespace AzureStaticWebApp.Common.Api.Data;

public interface ICosmosDbRepository<T> where T : CosmosEntity
{
    Task AddAsync(T entity, string partitionKey, CancellationToken cancellationToken);

    Task DeleteAsync(string id, string partitionKey, CancellationToken cancellationToken);

    Task<T?> GetAsync(string id, string partitionKey, CancellationToken cancellationToken);

    void InitializeCollection(string partitionKey, ICollection<string>? uniqueKeyPaths = null, ICollection<Collection<CompositePath>>? compositeIndexes = null);

    Task<PagedList<T>> ListAsync(Parameters parameters, CancellationToken cancellationToken, Expression<Func<T, bool>>? where = null, IOrderedQueryable<T>? query = null);

    Task UpdateAsync(T entity, string partitionKey, CancellationToken cancellationToken);
}

public class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : CosmosEntity
{
    protected readonly Container _container;
    private const string DatabaseName = "AzureStaticWebApp";
    private readonly string _account;
    private readonly string _key;

    public CosmosDbRepository(IConfiguration configuration)
    {
        _account = configuration["CosmosDb_Account"];
        _key = configuration["CosmosDb_Key"];

        var client = new CosmosClient(_account, _key);
        _container = client.GetContainer(DatabaseName, GetContainerName());
    }

    public Task AddAsync(T entity, string partitionKey, CancellationToken cancellationToken)
    {
        return _container.CreateItemAsync(entity, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string id, string partitionKey, CancellationToken cancellationToken)
    {
        return _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
    }

    public async Task<T?> GetAsync(string id, string partitionKey, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public void InitializeCollection(string partitionKey, ICollection<string>? uniqueKeyPaths = null, ICollection<Collection<CompositePath>>? compositeIndexes = null)
    {
        var client = new CosmosClient(_account, _key);

        var database = client.CreateDatabaseIfNotExistsAsync(DatabaseName).GetAwaiter().GetResult();
        var properties = new ContainerProperties(GetContainerName(), $"/{partitionKey}");

        if (uniqueKeyPaths != null)
        {
            var uniqueKeyPolicy = new UniqueKeyPolicy();
            foreach (var uniqueKeyPath in uniqueKeyPaths)
            {
                var uniqueKey = new UniqueKey();
                uniqueKey.Paths.Add(uniqueKeyPath);
                uniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
            }
            properties.UniqueKeyPolicy = uniqueKeyPolicy;
        }

        if (compositeIndexes != null)
        {
            foreach (var compositeIndex in compositeIndexes)
            {
                properties.IndexingPolicy.CompositeIndexes.Add(compositeIndex);
            }
        }

        _ = database.Database.CreateContainerIfNotExistsAsync(properties).GetAwaiter().GetResult();
    }

    public Task<PagedList<T>> ListAsync(Parameters parameters, CancellationToken cancellationToken, Expression<Func<T, bool>>? where = null, IOrderedQueryable<T>? query = null)
    {
        if (query == null)
        {
            query = _container.GetItemLinqQueryable<T>();
        }

        if (where is not null)
        {
            query = query.Where(where) as IOrderedQueryable<T>;
            if (query == null)
            {
                throw new InvalidOperationException("Invalid where clause.");
            }
        }

        return PagedList<T>.ToPagedListAsync(ApplySort(query, parameters.OrderBy), parameters.PageNumber, parameters.PageSize, cancellationToken);
    }

    public Task<VirtualResponse<T>> ListVirtualAsync(VirtualParameters parameters, CancellationToken cancellationToken, Expression<Func<T, bool>>? where = null, IOrderedQueryable<T>? query = null)
    {
        if (query == null)
        {
            query = _container.GetItemLinqQueryable<T>();
        }

        if (where is not null)
        {
            query = query.Where(where) as IOrderedQueryable<T>;
            if (query == null)
            {
                throw new InvalidOperationException("Invalid where clause.");
            }
        }

        return VirtualList<T>.ToVirtualListAsync(ApplySort(query, parameters.OrderBy), parameters.StartIndex, parameters.PageSize, cancellationToken);
    }

    public Task UpdateAsync(T entity, string partitionKey, CancellationToken cancellationToken)
    {
        return _container.UpsertItemAsync(entity, new PartitionKey(partitionKey), cancellationToken: cancellationToken);
    }

    private static IOrderedQueryable<T> ApplySort(IOrderedQueryable<T> entities, string? orderByQueryString = null)
    {
        if (string.IsNullOrWhiteSpace(orderByQueryString))
        {
            return entities;
        }

        var orderParams = orderByQueryString.Trim().Split(',');
        var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var isNext = false;

        foreach (var param in orderParams)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                continue;
            }

            var propertyFromQueryName = param.Trim().Split(" ")[0];
            var objectProperty = Array.Find(propertyInfos, pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

            if (objectProperty == null)
            {
                continue;
            }

            var sortingOrder = param.EndsWith(" desc", StringComparison.OrdinalIgnoreCase) ? "descending" : "ascending";

            entities = isNext ? entities.ThenBy($"{objectProperty.Name} {sortingOrder}") : entities.OrderBy($"{objectProperty.Name} {sortingOrder}");

            isNext = true;
        }

        return entities;
    }

    private static string GetContainerName()
    {
        return typeof(T).Name.Replace("Entity", "");
    }
}