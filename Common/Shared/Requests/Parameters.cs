namespace AzureStaticWebApp.Common.Shared.Requests;

public class Parameters
{
    public static readonly int[] PageSizeOptions = { 5, 10, 50, 100 };

    private int _pageSize = 10;
    private string _search = string.Empty;

    public string? OrderBy { get; set; } = string.Empty;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > PageSizeOptions.Max() ? PageSizeOptions.Max() : value;
    }

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
}