namespace AzureStaticWebApp.Shared.Common.Requests;

public class VirtualParameters
{
    private string _search = string.Empty;
    public string? OrderBy { get; set; } = string.Empty;
    public int PageSize { get; set; } = 15;

    public string Search
    {
        get => _search;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _search = value.ToLower().Trim();
            }
        }
    }

    public int StartIndex { get; set; }
}