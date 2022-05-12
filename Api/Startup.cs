using AzureStaticWebApp.Api;
using AzureStaticWebApp.Api.Common.Data;
using AzureStaticWebApp.Api.Data;
using AzureStaticWebApp.Api.Data.Movies;
using AzureStaticWebApp.Shared.Common.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace AzureStaticWebApp.Api;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        _ = builder.Services.AddLogging();
        _ = builder.Services.AddAutoMapper(typeof(Startup));
        _ = builder.Services.AddTransient<IDateTime, DateTimeService>();
        _ = builder.Services.AddTransient<IGuid, GuidService>();
        _ = builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();

        _ = builder.Services.AddScoped<IMovieRepository, MovieRepository>();

        Migration.Setup(builder);
    }
}