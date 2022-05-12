using AzureStaticWebApp.Shared.Common;
using Humanizer;
using System.Diagnostics.CodeAnalysis;

namespace AzureStaticWebApp.Api.Common.Exceptions;

[Serializable]
public class NotFoundException<T> : Exception where T : Model
{
    public NotFoundException(Guid id) : base($"The {typeof(T).Name.Humanize()} with id: {id} doesn't exist.")
    {
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Block usage.")]
    private NotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    private NotFoundException()
    {
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Block usage.")]
    private NotFoundException(string? message) : base(message)
    {
    }
}