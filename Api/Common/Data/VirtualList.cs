using AzureStaticWebApp.Common.Shared.Responses;
using Microsoft.Azure.Cosmos.Linq;

namespace AzureStaticWebApp.Api.Common.Data;

public class VirtualList<T> : List<T>
{
    public static async Task<VirtualResponse<T>> ToVirtualListAsync(IOrderedQueryable<T> source, int startIndex, int pageSize, CancellationToken cancellationToken)
    {
        var items = new List<T>();
        var count = await source.CountAsync(cancellationToken);
        var query = source.Skip(startIndex).Take(pageSize);

        using (var iterator = query.ToFeedIterator())
        {
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                items.AddRange(response);
            }
        }

        return new VirtualResponse<T> { Items = items, TotalSize = count };
    }
}