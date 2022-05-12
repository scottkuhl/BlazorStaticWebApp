using AutoMapper;
using AzureStaticWebApp.Api.Common.Data;
using AzureStaticWebApp.Shared.Models;
using AzureStaticWebApp.Shared.Responses.Movies;

namespace AzureStaticWebApp.Api.Data.Movies;

public class MovieEntity : Entity
{
    public string PosterImageUrl { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; } = DateTime.Today.Year;
}

public class MovieMappingProfile : Profile
{
    public MovieMappingProfile()
    {
        _ = CreateMap<MovieEntity, Movie>().ReverseMap();
        _ = CreateMap<MovieEntity, MovieListDto>();
    }
}