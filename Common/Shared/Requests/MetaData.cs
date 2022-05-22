namespace AzureStaticWebApp.Common.Shared.Requests;

public class MetaData
{
    public int CurrentPage { get; set; }
    public bool HasNext => CurrentPage < TotalPages;
    public bool HasPrevious => CurrentPage > 1;
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
}