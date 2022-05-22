using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Net.Http.Headers;

namespace AzureStaticWebApp.Common.Api.Cache;

#pragma warning disable CS0618 // Type or member is obsolete

public class FunctionResponseCacheAttribute : FunctionInvocationFilterAttribute
{
    private readonly ResponseCacheLocation _cacheLocation;
    private readonly int _duration;

    public FunctionResponseCacheAttribute(int durationInSeconds, ResponseCacheLocation cacheLocation)
    {
        _duration = durationInSeconds;
        _cacheLocation = cacheLocation;
    }

    public override Task OnExecutedAsync(FunctionExecutedContext executedContext, CancellationToken cancellationToken)
    {
        if (executedContext.Arguments.First().Value is not HttpRequest request)
        {
            throw new ApplicationException("HttpRequest is null. ModelBinding is not supported, please use HttpRequest as input parameter and deserialize using helper functions.");
        }

        request.HttpContext.Response.GetTypedHeaders().CacheControl = (executedContext.FunctionResult?.Exception == null ? _cacheLocation : ResponseCacheLocation.None) switch
        {
            ResponseCacheLocation.Any or ResponseCacheLocation.Client => new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.FromSeconds(_duration),
                NoStore = false,
                Public = true
            },
            ResponseCacheLocation.None => new CacheControlHeaderValue()
            {
                MaxAge = TimeSpan.Zero,
                NoStore = true
            },
            _ => throw new ArgumentOutOfRangeException(nameof(executedContext), "Invalid ResponseCacheLocation")
        };

        return base.OnExecutedAsync(executedContext, cancellationToken);
    }
}