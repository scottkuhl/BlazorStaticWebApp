using System.ComponentModel.DataAnnotations;

namespace AzureStaticWebApp.Api.Common.Validation;

public class Validation<T>
{
    public bool IsValid { get; set; }
    public IEnumerable<ValidationResult> ValidationResults { get; set; } = default!;
    public T Value { get; set; } = default!;
}