using AzureStaticWebApp.Common.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AzureStaticWebApp.Common.Api.Validation;

public static class ModelValidation
{
    public static async Task<(T Model, Validation<T> Validation)> Validate<T>(this HttpRequest request)
    {
        var model = JsonSerializer.Deserialize<T>(await new StreamReader(request.Body).ReadToEndAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (model is null)
        {
            throw new BadRequestException($"{nameof(T)} is null");
        }

        var body = new Validation<T>
        {
            Value = model
        };

        var results = new List<ValidationResult>();
        body.IsValid = Validator.TryValidateObject(body.Value, new ValidationContext(body.Value, null, null), results, true);
        body.ValidationResults = results;

        return (model, body);
    }
}