using AzureStaticWebApp.Common.Client.Exceptions;
using Microsoft.AspNetCore.Components;
using System.Net;
using Toolbelt.Blazor;

namespace AzureStaticWebApp.Common.Client.Services;

public class HttpInterceptorService
{
    private readonly HttpClientInterceptor _interceptor;
    private readonly NavigationManager _nav;

    public HttpInterceptorService(HttpClientInterceptor interceptor, NavigationManager nav)
    {
        _interceptor = interceptor;
        _nav = nav;
    }

    public void DisposeEvent()
    {
        _interceptor.AfterSend -= HandleResponse;
    }

    public void RegisterEvent()
    {
        _interceptor.AfterSend += HandleResponse;
    }

    private void HandleResponse(object? sender, HttpClientInterceptorEventArgs e)
    {
        if (e.Response == null)
        {
            _nav.NavigateTo("/error");
            throw new HttpResponseException("Server not available.");
        }

        if (!e.Response.IsSuccessStatusCode)
        {
            string? message;
            switch (e.Response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    _nav.NavigateTo("/error/404");
                    message = "Resource not found.";
                    break;

                case HttpStatusCode.Unauthorized:
                    _nav.NavigateTo("/error/unauthorized");
                    message = "Unauthorized access.";
                    break;

                default:
                    _nav.NavigateTo("/error");
                    message = "Something went wrong. Please try again in a few minutes or contact support.";
                    break;
            }

            throw new HttpRequestException(message);
        }
    }
}