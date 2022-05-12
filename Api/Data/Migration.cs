using AzureStaticWebApp.Api.Data.Movies;
using Bogus;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace AzureStaticWebApp.Api.Data;

public static class Migration
{
    public static void Setup(IFunctionsHostBuilder builder)
    {
        var serviceProvider = builder.Services.BuildServiceProvider();
        var movieRepository = serviceProvider.GetRequiredService<IMovieRepository>();

        var environment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT");
        if (environment != "Development")
        {
            return;
        }

        Randomizer.Seed = new Random(11232021);

        movieRepository.InitializeCollection();
        movieRepository.SeedTestData();
    }
}