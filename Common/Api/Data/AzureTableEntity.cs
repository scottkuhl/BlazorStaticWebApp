using Azure;
using Azure.Data.Tables;

namespace AzureStaticWebApp.Common.Api.Data;

public class AzureTableEntity : ITableEntity
{
    public ETag ETag { get; set; }
    public string PartitionKey { get; set; } = string.Empty;
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; } = DateTime.Now;
}