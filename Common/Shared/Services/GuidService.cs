namespace AzureStaticWebApp.Common.Shared.Services;

public interface IGuid
{
    Guid NewGuid { get; }
}

public class GuidService : IGuid
{
    public Guid NewGuid => Guid.NewGuid();
}