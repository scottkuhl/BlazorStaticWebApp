using AzureStaticWebApp.Shared.Common.Requests;
using Microsoft.Azure.Cosmos.Linq;

namespace AzureStaticWebApp.Api.Common.Data;

public class PagedList<T> : List<T>
{
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        MetaData = new MetaData
        {
            TotalCount = count,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        AddRange(items);
    }

    public MetaData MetaData { get; set; }

    public static async Task<PagedList<T>> ToPagedListAsync(IOrderedQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var items = new List<T>();
        var count = await source.CountAsync(cancellationToken);
        var query = source.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        using (var iterator = query.ToFeedIterator())
        {
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                items.AddRange(response);
            }
        }

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}