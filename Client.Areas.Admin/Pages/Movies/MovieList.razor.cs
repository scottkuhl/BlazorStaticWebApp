using AzureStaticWebApp.Client.Shared.Common.Components;
using AzureStaticWebApp.Client.Shared.Common.Services;
using AzureStaticWebApp.Client.Shared.Services;
using AzureStaticWebApp.Shared.Common.Requests;
using AzureStaticWebApp.Shared.Resources;
using AzureStaticWebApp.Shared.Responses.Movies;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Client.Areas.Admin.Pages.Movies;

public partial class MovieList : IDisposable
{
    private readonly Parameters Parameters = new();
    private readonly CancellationTokenSource TokenSource = new();

    [Inject] public IDialogService Dialog { get; set; } = default!;
    [Inject] public HttpInterceptorService? Interceptor { get; set; }
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Inject] public IMovieHttpService Service { get; set; } = default!;

    private MudTable<MovieListDto> Movies { get; set; } = new();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Interceptor != null)
            {
                Interceptor.DisposeEvent();
                Interceptor = null;
            }

            TokenSource.Cancel();
            TokenSource.Dispose();
        }
    }

    protected override void OnInitialized()
    {
        Interceptor?.RegisterEvent();
    }

    private async Task DeleteAsync(Guid id, string name)
    {
        var parameters = new DialogParameters
        {
            { "Content", $"Delete {name}?" }
        };

        var dialog = Dialog.Show<Confirmation>("Delete", parameters, new DialogOptions { DisableBackdropClick = true });

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await Service.DeleteAsync(id, TokenSource.Token);
            await Movies.ReloadServerData();
        }
    }

    private async Task<TableData<MovieListDto>> GetServerData(TableState state)
    {
        Parameters.PageSize = state.PageSize;
        Parameters.PageNumber = state.Page + 1;
        Parameters.OrderBy = state.SortDirection == SortDirection.Descending ? state.SortLabel + " desc" : state.SortLabel;

        var response = await Service.ListAsync(Parameters, TokenSource.Token);

        return new TableData<MovieListDto>
        {
            Items = response.Items,
            TotalItems = response.MetaData.TotalCount
        };
    }

    private void OnSearch(string search)
    {
        Parameters.Search = search;
        _ = Movies.ReloadServerData();
    }
}