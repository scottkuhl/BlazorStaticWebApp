using Azure;
using Azure.Data.Tables;
using AzureStaticWebApp.Common.Shared.Services;

namespace AzureStaticWebApp.Common.Api.Data;

public class AzureTableEntity : ITableEntity
{
    private readonly IDateTime _dateTime;

    public AzureTableEntity(IDateTime dateTime)
    {
        _dateTime = dateTime;

        Timestamp = _dateTime.UtcNow;
    }

    public ETag ETag { get; set; }
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
}