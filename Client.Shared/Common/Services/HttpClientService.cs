using Polly;
using System.Text.Json;

namespace AzureStaticWebApp.Client.Shared.Common.Services;

public interface IHttpClientService
{
    Task<string> UploadImage(MultipartFormDataContent content, CancellationToken cancellationToken);
}

public class HttpClientService : IHttpClientService
{
    protected readonly HttpClient _httpClient;
    protected readonly IAsyncPolicy<HttpResponseMessage> _httpRequestPolicy;
    protected readonly JsonSerializerOptions _options;

    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _httpRequestPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2),
                onRetry: (response, retryDelay, retryCount, _) => Console.WriteLine($"Received: {response.Result.StatusCode}, retryCount: {retryCount}, delaying: {retryDelay.Seconds} seconds\n"));
    }

    public async Task<string> UploadImage(MultipartFormDataContent content, CancellationToken cancellationToken)
    {
        var postResult = await _httpRequestPolicy.ExecuteAsync(() => _httpClient.PostAsync("/api/manage/upload", content, cancellationToken));
        return await postResult.Content.ReadAsStringAsync(cancellationToken);
    }
}