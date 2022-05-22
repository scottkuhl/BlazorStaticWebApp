using AzureStaticWebApp.Common.Shared.Requests;

namespace AzureStaticWebApp.Common.Shared.Responses;

public class PagingResponse<T> where T : class
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public MetaData MetaData { get; set; } = new();
}