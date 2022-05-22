using AzureStaticWebApp.Api.Common.Functions;
using AzureStaticWebApp.Api.Common.Validation;
using AzureStaticWebApp.Api.Data.Movies;
using AzureStaticWebApp.Common.Shared.Responses;
using AzureStaticWebApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureStaticWebApp.Api.Functions;

public class MovieFunctions : Function
{
    private readonly IMovieRepository _repository;

    public MovieFunctions(IHttpContextAccessor httpContextAccessor, ILogger<MovieFunctions> logger, IMovieRepository repository) : base(httpContextAccessor, logger)
    {
        _repository = repository;
    }

    [FunctionName("MovieCreate")]
    public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Function, "post", Route = "manage/movie")] HttpRequest req, CancellationToken cancellationToken)
    {
        var (Model, Validation) = await req.Validate<Movie>();
        if (!Validation.IsValid)
        {
            return new BadRequestObjectResult(new InvalidResults { ValidationResults = Validation.ValidationResults });
        }

        var movie = await _repository.CreateAsync(Model, cancellationToken);

        return new OkObjectResult(movie);
    }

    [FunctionName("MovieDelete")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "manage/movie")] HttpRequest req, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(req.Query["id"], out var id))
        {
            await _repository.DeleteAsync(id, cancellationToken);
            return new OkResult();
        }

        return new BadRequestResult();
    }

    [FunctionName("MovieGet")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "manage/movie")] HttpRequest req, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(req.Query["id"], out var id))
        {
            var movie = await _repository.GetAsync(id, cancellationToken);
            return new OkObjectResult(movie);
        }

        return new BadRequestResult();
    }

    [FunctionName("MovieList")]
    public async Task<IActionResult> List([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "moviespage")] HttpRequest req, CancellationToken cancellationToken)
    {
        var parameters = GetParametersFromQuery(req);
        _ = int.TryParse(req.Query["minYear"], out var minYear);
        _ = int.TryParse(req.Query["maxYear"], out var maxYear);

        return new OkObjectResult(await _repository.ListAsync(parameters, minYear, maxYear, cancellationToken));
    }

    [FunctionName("MovieListVirtual")]
    public async Task<IActionResult> ListVirtual([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "moviesvirtual")] HttpRequest req, CancellationToken cancellationToken)
    {
        var parameters = GetVirtualParametersFromQuery(req);
        _ = int.TryParse(req.Query["minYear"], out var minYear);
        _ = int.TryParse(req.Query["maxYear"], out var maxYear);

        return new OkObjectResult(await _repository.ListVirtualAsync(parameters, minYear, maxYear, cancellationToken));
    }

    [FunctionName("MovieUpdate")]
    public async Task<IActionResult> Update([HttpTrigger(AuthorizationLevel.Function, "put", Route = "manage/movie")] HttpRequest req, CancellationToken cancellationToken)
    {
        var (Model, Validation) = await req.Validate<Movie>();
        if (!Validation.IsValid)
        {
            return new BadRequestObjectResult(new InvalidResults { ValidationResults = Validation.ValidationResults });
        }

        var movie = await _repository.UpdateAsync(Model, cancellationToken);

        return new OkObjectResult(movie);
    }
}