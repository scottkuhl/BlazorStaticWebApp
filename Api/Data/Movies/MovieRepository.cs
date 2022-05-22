using AutoMapper;
using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Api.Exceptions;
using AzureStaticWebApp.Common.Shared.Requests;
using AzureStaticWebApp.Common.Shared.Responses;
using AzureStaticWebApp.Common.Shared.Services;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Responses.Movies;
using Bogus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;

namespace AzureStaticWebApp.Api.Data.Movies;

public interface IMovieRepository
{
    Task<Movie> CreateAsync(Movie movie, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Movie> GetAsync(Guid id, CancellationToken cancellationToken);

    void InitializeCollection();

    Task<PagingResponse<MovieListDto>> ListAsync(Parameters parameters, int minYear, int maxYear, CancellationToken cancellationToken);

    Task<VirtualResponse<MovieListDto>> ListVirtualAsync(VirtualParameters parameters, int minYear, int maxYear, CancellationToken cancellationToken);

    void SeedTestData();

    Task<Movie> UpdateAsync(Movie movie, CancellationToken cancellationToken);
}

public sealed class MovieRepository : CosmosDbRepository<MovieEntity>, IMovieRepository
{
    private readonly IAzureStorageService _azureStorageService;
    private readonly IGuid _guid;
    private readonly IMapper _mapper;

    public MovieRepository(IAzureStorageService azureStorageService, IMapper mapper, IGuid guid, IConfiguration configuration) : base(configuration)
    {
        _azureStorageService = azureStorageService;
        _guid = guid;
        _mapper = mapper;
    }

    public async Task<Movie> CreateAsync(Movie movie, CancellationToken cancellationToken)
    {
        movie.Id = _guid.NewGuid;

        var entity = _mapper.Map<MovieEntity>(movie);
        await AddAsync(entity, entity.id, cancellationToken);
        return movie;
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

    public void InitializeCollection()
    {
        InitializeCollection(nameof(MovieEntity.id), compositeIndexes: new List<Collection<CompositePath>>
        {
            new Collection<CompositePath>
            {
                new CompositePath { Path = $"/{nameof(Movie.Year)}", Order = CompositePathSortOrder.Descending },
                new CompositePath { Path = $"/{nameof(Movie.Title)}", Order = CompositePathSortOrder.Ascending }
            }
        });
    }

    public async Task<PagingResponse<MovieListDto>> ListAsync(Parameters parameters, int minYear, int maxYear, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            parameters.OrderBy = $"{nameof(Movie.Year)} desc, {nameof(Movie.Title)} ";
        }

        var query = _container.GetItemLinqQueryable<MovieEntity>();
        _ = query.Select(x => new { x.id, x.PosterImageUrl, x.Title, x.Year });

        var entities = await ListAsync(parameters, cancellationToken,
            where: x =>
                (minYear == 0 || x.Year >= minYear)
                && (maxYear == 0 || x.Year <= maxYear)
                && (string.IsNullOrWhiteSpace(parameters.Search) || x.Title.Contains(parameters.Search, StringComparison.InvariantCultureIgnoreCase) || x.Summary.Contains(parameters.Search, StringComparison.InvariantCultureIgnoreCase) || x.Year.ToString() == parameters.Search),
            query: query);

        var movies = _mapper.Map<IEnumerable<MovieListDto>>(entities);
        return new PagingResponse<MovieListDto> { Items = movies, MetaData = entities.MetaData };
    }

    public async Task<VirtualResponse<MovieListDto>> ListVirtualAsync(VirtualParameters parameters, int minYear, int maxYear, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(parameters.OrderBy))
        {
            parameters.OrderBy = $"{nameof(Movie.Year)} desc, {nameof(Movie.Title)} ";
        }

        var query = _container.GetItemLinqQueryable<MovieEntity>();
        _ = query.Select(x => new { x.id, x.PosterImageUrl, x.Title, x.Year });

        var entities = await ListVirtualAsync(parameters, cancellationToken,
            where: x =>
                (minYear == 0 || x.Year >= minYear)
                && (maxYear == 0 || x.Year <= maxYear)
                && (string.IsNullOrWhiteSpace(parameters.Search) || x.Title.Contains(parameters.Search, StringComparison.InvariantCultureIgnoreCase) || x.Summary.Contains(parameters.Search, StringComparison.InvariantCultureIgnoreCase) || x.Year.ToString() == parameters.Search),
            query: query);

        var movies = _mapper.Map<IEnumerable<MovieListDto>>(entities.Items);
        return new VirtualResponse<MovieListDto> { Items = movies.ToList(), TotalSize = entities.TotalSize };
    }

    public void SeedTestData()
    {
        if (!ListAsync(new Parameters(), 0, 0, default).Result.Items.Any())
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
                CreateAsync(model, default).Wait();
            }
        }
    }

    public async Task<Movie> UpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        var entity = await ExistsAsync(movie.Id, cancellationToken);

        _ = _mapper.Map(movie, entity);
        await UpdateAsync(entity, entity.id, cancellationToken);

        return movie;
    }

    private async Task<MovieEntity> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetAsync(id.ToString(), id.ToString(), cancellationToken);
        return entity ?? throw new NotFoundException<Movie>(id);
    }
}