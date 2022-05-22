namespace AzureStaticWebApp.Common.Shared.Services;

public interface IDateTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}