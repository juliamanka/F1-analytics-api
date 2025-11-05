using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;

namespace F1Analytics.Database.Models;

public class Series
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("series_id")]
    public string SeriesId { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "TEXT")]
    public string? Description { get; set; }

    [Column("min_value", TypeName = "decimal(10,2)")]
    public decimal MinValue { get; set; }

    [Column("max_value", TypeName = "decimal(10,2)")]
    public decimal MaxValue { get; set; }

    [Required]
    [StringLength(10)]
    public string Unit { get; set; } = string.Empty;

    [Required]
    [StringLength(7)]
    public string Color { get; set; } = string.Empty;

    [StringLength(100)]
    [Column("measurement_type")]
    public string? MeasurementType { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}