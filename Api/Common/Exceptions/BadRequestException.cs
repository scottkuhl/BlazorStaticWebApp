using System.Diagnostics.CodeAnalysis;

namespace AzureStaticWebApp.Api.Common.Exceptions;

[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }

    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Block usage.")]
    private BadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    private BadRequestException()
    {
    }
}