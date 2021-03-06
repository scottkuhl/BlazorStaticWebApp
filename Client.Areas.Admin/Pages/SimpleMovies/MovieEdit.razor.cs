using AzureStaticWebApp.Client.Shared.Services;
using AzureStaticWebApp.Common.Client.Components;
using AzureStaticWebApp.Common.Client.Services;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AzureStaticWebApp.Client.Areas.Admin.Pages.SimpleMovies;

public partial class MovieEdit : IDisposable
{
    private readonly CancellationTokenSource TokenSource = new();

    [Inject] public IDialogService Dialog { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }
    [Inject] public HttpInterceptorService? Interceptor { get; set; }
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Inject] public NavigationManager Nav { get; set; } = default!;
    [Inject] public ISimpleMovieHttpService Service { get; set; } = default!;
    [Inject] public IServiceProvider Services { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;
    private bool IsLoading { get; set; } = true;
    private bool IsNew { get; set; } = true;
    private bool IsSubmitting { get; set; }
    private Movie Movie { get; set; } = new();

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

    protected override async Task OnInitializedAsync()
    {
        Interceptor?.RegisterEvent();

        if (Id.HasValue)
        {
            Movie = await Service.GetAsync(Id.Value, TokenSource.Token);
            IsNew = false;
        }

        IsLoading = false;
    }

    private async Task DeleteAsync()
    {
        var parameters = new DialogParameters
        {
            { "Content", $"Delete {Movie.Title}?" }
        };

        var dialog = Dialog.Show<Confirmation>("Delete", parameters, new DialogOptions { DisableBackdropClick = true });

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            await Service.DeleteAsync(Movie.Id, TokenSource.Token);
            Nav.NavigateTo("/admin/simplemovies");
        }
    }

    private async Task ExecuteDialog()
    {
        var parameters = new DialogParameters
        {
            { "Content", IsNew ? "You have successfully created a new movie." : "You have successfully updated the movie." },
            { "ButtonColor", Color.Primary },
            { "ButtonText", "OK" }
        };

        var dialog = Dialog.Show<DialogNotification>("Success", parameters);

        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            _ = bool.TryParse(result.Data.ToString(), out var shouldNavigate);
            if (shouldNavigate)
            {
                Nav.NavigateTo($"/admin/simplemovies/edit/{Movie.Id}");
            }
        }
    }

    private async Task SaveAsync()
    {
        if (IsSubmitting)
        {
            return;
        }

        IsSubmitting = true;

        await Service.SaveAsync(Movie, TokenSource.Token);

        IsNew = false;
        IsSubmitting = false;

        await ExecuteDialog();
    }

    private void SetImageUrl(string imageUrl)
    {
        Movie.PosterImageUrl = imageUrl;
    }
}