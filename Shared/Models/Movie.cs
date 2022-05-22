using AzureStaticWebApp.Common.Shared;
using Ganss.XSS;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace AzureStaticWebApp.Shared.Models;

public class Movie : Model
{
    public const int MaxYear = 2120;
    public const int MinYear = 1890;

    private string _summary = string.Empty;
    private string _title = string.Empty;
    public bool IsNew => Id == Guid.Empty;

    [MaxLength(2048, ErrorMessage = "Poster URL must be 2048 characters or less.")]
    [Url(ErrorMessage = "Not a valid URL.")]
    public string PosterImageUrl { get; set; } = string.Empty;

    public string Summary
    {
        get => _summary;
        set => _summary = new HtmlSanitizer().Sanitize(value);
    }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "Title must be 200 characters or less.")]
    public string Title
    {
        get => _title;
        set => _title = HttpUtility.HtmlEncode(value);
    }

    [Required(ErrorMessage = "Year is required.")]
    [Range(MinYear, MaxYear, ErrorMessage = "Year must be between 1890 and 2120")]
    public int Year { get; set; } = DateTime.Today.Year;
}