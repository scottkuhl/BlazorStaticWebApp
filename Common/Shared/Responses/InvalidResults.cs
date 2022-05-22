using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AzureStaticWebApp.Common.Shared.Responses;

public class InvalidResults
{
    public IEnumerable<ValidationResult> ValidationResults { get; set; } = default!;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}