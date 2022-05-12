namespace AzureStaticWebApp.Shared.Responses.Movies;

public record MovieListDto(
    Guid Id,
    string PosterImageUrl,
    string Title,
    int Year
);