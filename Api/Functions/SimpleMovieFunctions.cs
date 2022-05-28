using AzureStaticWebApp.Api.Data.SimpleMovies;
using AzureStaticWebApp.Common.Api.Functions;
using AzureStaticWebApp.Common.Api.Validation;
using AzureStaticWebApp.Common.Shared.Responses;
using AzureStaticWebApp.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureStaticWebApp.Api.Functions;

public class SimpleMovieFunctions : Function
{
    private readonly ISimpleMovieRepository _repository;

    public SimpleMovieFunctions(IHttpContextAccessor httpContextAccessor, ILogger<MovieFunctions> logger, ISimpleMovieRepository repository) : base(httpContextAccessor, logger)
    {
        _repository = repository;
    }

    [FunctionName("SimpleMovieDelete")]
    public async Task<IActionResult> Delete([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "manage/simplemovie")] HttpRequest req, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(req.Query["id"], out var id))
        {
            await _repository.DeleteAsync(id, cancellationToken);
            return new OkResult();
        }

        return new BadRequestResult();
    }

    [FunctionName("SimpleMovieGet")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Function, "get", Route = "manage/simplemovie")] HttpRequest req, CancellationToken cancellationToken)
    {
        if (Guid.TryParse(req.Query["id"], out var id))
        {
            var movie = await _repository.GetAsync(id, cancellationToken);
            return new OkObjectResult(movie);
        }

        return new BadRequestResult();
    }

    [FunctionName("SimpleMovieList")]
    public async Task<IActionResult> List([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "simplemoviespage")] HttpRequest req, CancellationToken cancellationToken)
    {
        var search = req.Query["search"];
        _ = int.TryParse(req.Query["minYear"], out var minYear);
        _ = int.TryParse(req.Query["maxYear"], out var maxYear);

        return new OkObjectResult(await _repository.ListAsync(search, minYear, maxYear, cancellationToken));
    }

    [FunctionName("SimpleMovieSave")]
    public async Task<IActionResult> Save([HttpTrigger(AuthorizationLevel.Function, "post", Route = "manage/simplemovie")] HttpRequest req, CancellationToken cancellationToken)
    {
        var (Model, Validation) = await req.Validate<Movie>();
        if (!Validation.IsValid)
        {
            return new BadRequestObjectResult(new InvalidResults { ValidationResults = Validation.ValidationResults });
        }

        var movie = await _repository.SaveAsync(Model, cancellationToken);

        return new OkObjectResult(movie);
    }
}