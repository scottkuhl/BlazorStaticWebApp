using AzureStaticWebApp.Api;
using AzureStaticWebApp.Api.Data;
using AzureStaticWebApp.Api.Data.Movies;
using AzureStaticWebApp.Api.Data.SimpleMovies;
using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Shared.Services;
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
        _ = builder.Services.AddScoped<IAzureBlobStorageService, AzureBlobStorageService>();

        _ = builder.Services.AddScoped<IMovieRepository, MovieRepository>();
        _ = builder.Services.AddScoped<ISimpleMovieRepository, SimpleMovieRepository>();

        Migration.Setup(builder);
    }
}