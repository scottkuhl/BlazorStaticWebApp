using AzureStaticWebApp.Common.Client.Services;
using AzureStaticWebApp.Common.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Net.Http.Headers;

namespace AzureStaticWebApp.Common.Client.Components;

public partial class ImageUpload : IDisposable
{
    private readonly CancellationTokenSource TokenSource = new();

    [Parameter] public string ImageUrl { get; set; } = string.Empty;
    [Inject] public HttpInterceptorService? Interceptor { get; set; }
    [Inject] public IStringLocalizer<Resource> Localizer { get; set; } = default!;
    [Parameter] public EventCallback<string> OnChange { get; set; }
    [Inject] public IHttpClientService Service { get; set; } = default!;
    [Inject] public ISnackbar Snackbar { get; set; } = default!;

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

    private async Task UploadImageAsync(InputFileChangeEventArgs e)
    {
        foreach (var imageFile in e.GetMultipleFiles())
        {
            if (imageFile != null)
            {
                var resizedFile = await imageFile.RequestImageFileAsync("image/png", 300, 500);
                using var ms = resizedFile.OpenReadStream(resizedFile.Size);
                var content = new MultipartFormDataContent();
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                content.Add(new StreamContent(ms, Convert.ToInt32(resizedFile.Size)), "image", imageFile.Name);
                ImageUrl = await Service.UploadImage(content, TokenSource.Token);
                await OnChange.InvokeAsync(ImageUrl);
            }
        }

        _ = Snackbar.Add("Image uploaded successfully", Severity.Info);
    }
}