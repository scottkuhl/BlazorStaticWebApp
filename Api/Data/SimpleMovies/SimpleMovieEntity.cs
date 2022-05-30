using AutoMapper;
using AzureStaticWebApp.Common.Api.Data;
using AzureStaticWebApp.Common.Shared.Services;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Responses.Movies;

namespace AzureStaticWebApp.Api.Data.SimpleMovies;

public class SimpleMovieEntity : AzureTableEntity
{
    public SimpleMovieEntity(IDateTime dateTime) : base(dateTime)
    {
    }

    public SimpleMovieEntity() : base(new DateTimeService())
    {
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string PosterImageUrl { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.Today.Year;
}

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        _ = CreateMap<SimpleMovieEntity, Movie>().ReverseMap();
        _ = CreateMap<SimpleMovieEntity, MovieListDto>();
    }
}