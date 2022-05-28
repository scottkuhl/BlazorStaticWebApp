using AutoMapper;
using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Api.Exceptions;
using AzureStaticWebApp.Common.Shared.Services;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Responses.Movies;
using Bogus;
using Microsoft.Extensions.Configuration;

namespace AzureStaticWebApp.Api.Data.SimpleMovies;

public interface ISimpleMovieRepository
{
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Movie> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<List<MovieListDto>> ListAsync(string search, int minYear, int maxYear, CancellationToken cancellationToken);

    Task<Movie> SaveAsync(Movie movie, CancellationToken cancellationToken);

    void SeedTestData();
}

public sealed class SimpleMovieRepository : AzureTableStorageRepository<SimpleMovieEntity>, ISimpleMovieRepository
{
    private readonly IAzureBlobStorageService _azureStorageService;
    private readonly IGuid _guid;
    private readonly IMapper _mapper;

    public SimpleMovieRepository(IAzureBlobStorageService azureStorageService, IMapper mapper, IGuid guid, IConfiguration configuration) : base(configuration)
    {
        _azureStorageService = azureStorageService;
        _guid = guid;
        _mapper = mapper;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await ExistsAsync(id, cancellationToken);
        await DeleteAsync(id.ToString(), id.ToString(), cancellationToken);
        await _azureStorageService.DeleteFileAsync(entity.PosterImageUrl, cancellationToken);
    }

    public async Task<Movie> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await ExistsAsync(id, cancellationToken);
        return _mapper.Map<Movie>(entity);
    }

    public async Task<List<MovieListDto>> ListAsync(string search, int minYear, int maxYear, CancellationToken cancellationToken)
    {
        string? filter = null;
        if (minYear > 0 && maxYear > 0)
        {
            filter = $"PartitionKey ge '{minYear}' and PartitionKey le '{maxYear}'";
        }
        else if (minYear > 0)
        {
            filter = $"PartitionKey ge '{minYear}'";
        }
        else if (maxYear > 0)
        {
            filter = $"PartitionKey le '{maxYear}'";
        }

        var results = new List<SimpleMovieEntity>();
        var query = _tableClient.QueryAsync<SimpleMovieEntity>(filter, 10, cancellationToken: cancellationToken);
        await foreach (var page in query.AsPages())
        {
            foreach (var value in page.Values)
            {
                results.Add(value);
            }
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            // NOTE: This could be done on the client or server.  Client = less processing on the server, Server = less bandwidth in the data transfer.
            results = results.Where(x => x.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase) || x.Summary.Contains(search, StringComparison.InvariantCultureIgnoreCase) || x.Year.ToString() == search).ToList();
        }

        // NOTE: Ordering should be done on the client side.

        var items = new List<MovieListDto>();
        foreach (var result in results)
        {
            items.Add(new MovieListDto(result.Id, result.PosterImageUrl, result.Title, result.Year));
        }

        return items;
    }

    public async Task<Movie> SaveAsync(Movie movie, CancellationToken cancellationToken)
    {
        if (movie.Id == Guid.Empty)
        {
            movie.Id = _guid.NewGuid;
        }

        var entity = _mapper.Map<SimpleMovieEntity>(movie);
        entity.PartitionKey = movie.Id.ToString();
        entity.RowKey = movie.Id.ToString();

        _ = await SaveAsync(entity, cancellationToken);
        return movie;
    }

    public void SeedTestData()
    {
        if (ListAsync(string.Empty, 0, 0, default).Result.Count == 0)
        {
            var testModels = new Faker<Movie>()
                .RuleFor(r => r.Summary, f => f.Lorem.Sentence())
                .RuleFor(r => r.Id, _ => Guid.NewGuid())
                .RuleFor(r => r.PosterImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(r => r.Year, f => f.Date.Past(yearsToGoBack: 100).Year)
                .RuleFor(r => r.Title, f => f.Company.CompanyName());

            for (var i = 1; i <= 100; i++)
            {
                var model = testModels.Generate();
                SaveAsync(model, default).Wait();
            }
        }
    }

    private async Task<SimpleMovieEntity> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = (await _tableClient.GetEntityAsync<SimpleMovieEntity>(id.ToString(), id.ToString(), cancellationToken: cancellationToken)).Value;
        return entity is null ? throw new NotFoundException<Movie>(id) : (SimpleMovieEntity)entity;
    }
}