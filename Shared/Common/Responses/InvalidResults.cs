using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AzureStaticWebApp.Shared.Common.Responses;

public class InvalidResults
{
    public IEnumerable<ValidationResult> ValidationResults { get; set; } = default!;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}