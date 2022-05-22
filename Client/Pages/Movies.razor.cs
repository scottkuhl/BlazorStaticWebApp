using AzureStaticWebApp.Client.Shared.Common.Services;
using AzureStaticWebApp.Client.Shared.Services;
using AzureStaticWebApp.Common.Shared.Requests;
using AzureStaticWebApp.Shared.Resources;
using AzureStaticWebApp.Shared.Responses.Movies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.Extensions.Localization;

namespace AzureStaticWebApp.Client.Pages;

public partial class Movies : IDisposable
{
    private readonly CancellationTokenSource TokenSource = new();

    [Inject] public HttpInterceptorService? Interceptor { get; set; }
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Inject] public IMovieHttpService Service { get; set; } = default!;
    private int TotalSize { get; set; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && Interceptor != null)
        {
            Interceptor.DisposeEvent();
            Interceptor = null;

            TokenSource.Cancel();
            TokenSource.Dispose();
        }
    }

    protected override void OnInitialized()
    {
        Interceptor?.RegisterEvent();
    }

    private async ValueTask<ItemsProviderResult<MovieListDto>> LoadData(ItemsProviderRequest request)
    {
        var count = Math.Min(request.Count, TotalSize - request.StartIndex);
        var parameters = new VirtualParameters { StartIndex = request.StartIndex, PageSize = count == 0 ? request.Count : count };
        var movies = await Service.ListAsync(parameters, TokenSource.Token);
        TotalSize = movies.TotalSize;

        return new ItemsProviderResult<MovieListDto>(movies.Items, movies.TotalSize);
    }
}