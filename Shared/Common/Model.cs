using AzureStaticWebApp.Shared.Common.Services;

namespace AzureStaticWebApp.Shared.Common;

public abstract class Model
{
    protected Model() : this(new GuidService())
    {
    }

    protected Model(IGuid guid)
    {
        Id = guid.NewGuid;
    }

    public Guid Id { get; set; }
}