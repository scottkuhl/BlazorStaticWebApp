using System.Diagnostics.CodeAnalysis;

namespace AzureStaticWebApp.Common.Api.Data;

public abstract class Entity
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Lowercase required by Cosmos DB.")]
    public string id { get; set; } = string.Empty;
}