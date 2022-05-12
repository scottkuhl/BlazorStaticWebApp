using AzureStaticWebApp.Client.Shared.Common.Services;
using AzureStaticWebApp.Shared.Common.Requests;
using AzureStaticWebApp.Shared.Common.Responses;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Responses.Movies;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using System.Text.Json;

namespace AzureStaticWebApp.Client.Shared.Services;

public interface IMovieHttpService
{
    Task CreateAsync(Movie movie, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    Task<Movie> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<PagingResponse<MovieListDto>> ListAsync(Parameters parameters, CancellationToken cancellationToken, int? minYear = null, int? maxYear = null);

    Task<VirtualResponse<MovieListDto>> ListAsync(VirtualParameters parameters, CancellationToken cancellationToken);

    Task UpdateAsync(Movie movie, CancellationToken cancellationToken);
}

public class MovieHttpService : HttpClientService, IMovieHttpService
{
    private readonly string _endPoint = "/api/movie";
    private readonly string _endPointAdmin = "/api/manage/movie";

    public MovieHttpService(HttpClient httpClient) : base(httpClient)
    {
    }

    public Task CreateAsync(Movie movie, CancellationToken cancellationToken)
    {
        return _httpRequestPolicy.ExecuteAsync(() => _httpClient.PostAsJsonAsync(_endPointAdmin, movie, cancellationToken));
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

        response.EnsureSuccessStatusCode();
        var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonSerializer.DeserializeAsync<Movie>(stream, _options, cancellationToken) ?? new();
    }

    public async Task<PagingResponse<MovieListDto>> ListAsync(Parameters parameters, CancellationToken cancellationToken, int? minYear = null, int? maxYear = null)
    {
        var queryStringParam = new Dictionary<string, string>
        {
            ["pageNumber"] = parameters.PageNumber.ToString(),
            ["pageSize"] = parameters.PageSize.ToString(),
            ["search"] = parameters.Search ?? string.Empty,
            ["orderBy"] = parameters.OrderBy ?? string.Empty,
            ["minYear"] = minYear.HasValue ? minYear.Value.ToString() : string.Empty,
            ["maxYear"] = minYear.HasValue ? minYear.Value.ToString() : string.Empty
        };

        using var response = await _httpRequestPolicy.ExecuteAsync(() => _httpClient.GetAsync(QueryHelpers.AddQueryString($"{_endPoint}spage", queryStringParam), cancellationToken));
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PagingResponse<MovieListDto>>(content, _options) ?? new PagingResponse<MovieListDto>();
    }

    public async Task<VirtualResponse<MovieListDto>> ListAsync(VirtualParameters parameters, CancellationToken cancellationToken)
    {
        var queryStringParam = new Dictionary<string, string>
        {
            ["pageSize"] = parameters.PageSize.ToString(),
            ["startIndex"] = parameters.StartIndex.ToString()
        };

        using var response = await _httpRequestPolicy.ExecuteAsync(() => _httpClient.GetAsync(QueryHelpers.AddQueryString($"{_endPoint}svirtual", queryStringParam), cancellationToken));
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<VirtualResponse<MovieListDto>>(content, _options) ?? new VirtualResponse<MovieListDto>();
    }

    public Task UpdateAsync(Movie movie, CancellationToken cancellationToken)
    {
        return _httpRequestPolicy.ExecuteAsync(() => _httpClient.PutAsJsonAsync(_endPointAdmin, movie, cancellationToken));
    }
}