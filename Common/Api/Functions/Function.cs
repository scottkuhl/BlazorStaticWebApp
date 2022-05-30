using AzureStaticWebApp.Common.Api.Exceptions;
using AzureStaticWebApp.Common.Shared;
using AzureStaticWebApp.Common.Shared.Requests;
using AzureStaticWebApp.Common.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureStaticWebApp.Common.Api.Functions;

#pragma warning disable CS0618 // Type or member is obsolete (IFunctionExceptionFilter is in preview)

public abstract class Function : IFunctionExceptionFilter
{
    private readonly HttpResponse _httpResponse;
    private readonly ILogger<Function> _logger;

    protected Function(IHttpContextAccessor httpContextAccessor, ILogger<Function> logger)
    {
        _httpResponse = httpContextAccessor.HttpContext.Response;
        _logger = logger;
    }

    public Task OnExceptionAsync(FunctionExceptionContext exceptionContext, CancellationToken cancellationToken)
    {
        _logger.LogError($"Something went wrong: {exceptionContext.Exception}");

        _httpResponse.ContentType = "application/json";
        SetStatusCode(exceptionContext.Exception.InnerException ?? new Exception());

        return _httpResponse.WriteAsync(new ErrorDetails
        {
            StatusCode = _httpResponse.StatusCode,
            Message = "Internal Server Error"
        }.ToString(), cancellationToken);
    }

    protected static Parameters GetParametersFromQuery(HttpRequest req)
    {
        var parameters = new Parameters();

        if (int.TryParse(req.Query["pageNumber"], out var pageNumber))
        {
            parameters.PageNumber = pageNumber;
        }

        if (int.TryParse(req.Query["pageSize"], out var pageSize))
        {
            parameters.PageSize = pageSize;
        }

        parameters.Search = req.Query["search"];

        parameters.OrderBy = req.Query["orderBy"];

        return parameters;
    }

    protected static VirtualParameters GetVirtualParametersFromQuery(HttpRequest req)
    {
        var parameters = new VirtualParameters();

        if (int.TryParse(req.Query["startIndex"], out var startIndex))
        {
            parameters.StartIndex = startIndex;
        }

        if (int.TryParse(req.Query["pageSize"], out var pageSize))
        {
            parameters.PageSize = pageSize;
        }

        parameters.Search = req.Query["search"];

        parameters.OrderBy = req.Query["orderBy"];

        return parameters;
    }

    private static string GetNameWithoutGeneric(string name)
    {
        var index = name.IndexOf('`');
        return index == -1 ? name : name[..index];
    }

    private void SetStatusCode(Exception exceptionThrown)
    {
        var inboundExceptionTypeName = GetNameWithoutGeneric(exceptionThrown.GetType().Name);

        var badRequestExceptionTypeName = GetNameWithoutGeneric(typeof(BadRequestException).Name);
        var notFoundExceptionTypeName = GetNameWithoutGeneric(typeof(NotFoundException<Model>).Name);

        _httpResponse.StatusCode = inboundExceptionTypeName == badRequestExceptionTypeName
            ? StatusCodes.Status400BadRequest
            : inboundExceptionTypeName == notFoundExceptionTypeName
                ? StatusCodes.Status404NotFound
                : StatusCodes.Status500InternalServerError;
    }
}