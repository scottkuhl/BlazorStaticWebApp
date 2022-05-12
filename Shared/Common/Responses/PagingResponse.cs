using AzureStaticWebApp.Shared.Common.Requests;

namespace AzureStaticWebApp.Shared.Common.Responses;

public class PagingResponse<T> where T : class
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public MetaData MetaData { get; set; } = new();
}