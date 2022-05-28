using AzureStaticWebApp.Common.Client.Services;
using AzureStaticWebApp.Common.Shared.Requests;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Responses.Movies;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text.Json;

namespace AzureStaticWebApp.Client.Shared.Services;

public interface ISimpleMovieHttpService
{
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Movie> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<List<MovieListDto>> ListAsync(Parameters parameters, CancellationToken cancellationToken, int? minYear = null, int? maxYear = null);

    Task SaveAsync(Movie movie, CancellationToken cancellationToken);
}

public class SimpleMovieHttpService : HttpClientService, ISimpleMovieHttpService
{
    private readonly string _endPoint = "/api/simplemovie";
    private readonly string _endPointAdmin = "/api/manage/simplemovie";

    public SimpleMovieHttpService(HttpClient httpClient) : base(httpClient)
    {
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var address = $"{_endPointAdmin}?id={id}";
        return _httpRequestPolicy.ExecuteAsync(() => _httpClient.DeleteAsync(address, cancellationToken));
    }

    public async Task<Movie> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var uri = $"{_endPointAdmin}?id={id}";

        using var response = await _httpRequestPolicy.ExecuteAsync(() => _httpClient.GetAsync(uri, cancellationToken));

        _ = response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<Movie>(stream, _options, cancellationToken) ?? new();
    }

    public async Task<List<MovieListDto>> ListAsync(Parameters parameters, CancellationToken cancellationToken, int? minYear = null, int? maxYear = null)
    {
        var queryStringParam = new Dictionary<string, string>
        {
            ["search"] = parameters.Search ?? string.Empty,
            ["minYear"] = minYear.HasValue ? minYear.Value.ToString() : string.Empty,
            ["maxYear"] = minYear.HasValue ? minYear.Value.ToString() : string.Empty
        };

        using var response = await _httpRequestPolicy.ExecuteAsync(() => _httpClient.GetAsync(QueryHelpers.AddQueryString($"{_endPoint}spage", queryStringParam), cancellationToken));
        var content = await response.Content.ReadAsStringAsync();
        var results = JsonSerializer.Deserialize<List<MovieListDto>>(content, _options) ?? new List<MovieListDto>();

        if (parameters.OrderBy is not null)
        {
            switch (parameters.OrderBy)
            {
                case nameof(MovieListDto.Title):
                    results = results.OrderBy(x => x.Title).ToList();
                    break;

                case nameof(MovieListDto.Year):
                    results = results.OrderBy(x => x.Year).ToList();
                    break;
            }
        }
        else
        {
            results = results.OrderByDescending(x => x.Year).ThenBy(x => x.Title).ToList();
        }

        return results ?? new List<MovieListDto>();
    }

    public Task SaveAsync(Movie movie, CancellationToken cancellationToken)
    {
        return _httpRequestPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync(_endPointAdmin, movie, cancellationToken));
    }
}