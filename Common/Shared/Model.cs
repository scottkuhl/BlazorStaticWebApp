using AzureStaticWebApp.Common.Shared.Services;

namespace AzureStaticWebApp.Common.Shared;

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