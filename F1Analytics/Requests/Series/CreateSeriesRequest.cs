using System.ComponentModel.DataAnnotations;

namespace F1Analytics.Requests.Series;

public class CreateSeriesRequest
{
    [Required]
    [StringLength(50)]
    public string SeriesId { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public decimal MinValue { get; set; }

    [Required]
    public decimal MaxValue { get; set; }

    [Required]
    [StringLength(10)]
    public string Unit { get; set; } = string.Empty;

    [Required]
    [StringLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex color code")]
    public string Color { get; set; } = string.Empty;

    [StringLength(100)]
    public string? MeasurementType { get; set; }
}