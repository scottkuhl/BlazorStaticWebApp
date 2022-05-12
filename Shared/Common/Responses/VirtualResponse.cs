namespace AzureStaticWebApp.Shared.Common.Responses;

public class VirtualResponse<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalSize { get; set; }
}