using System.Diagnostics.CodeAnalysis;

namespace AzureStaticWebApp.Api.Common.Data;

public abstract class Entity
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Lowercase required by Cosmos DB.")]
    public string id { get; set; } = string.Empty;
}