using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace F1Analytics.Database.Models;

public class Measurement
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column("series_id")]
    public string SeriesId { get; set; } = string.Empty;

    [Required]
    [Column("timestamp")]
    public DateTime Timestamp { get; set; }

    [Required]
    [Column("value", TypeName = "decimal(10,2)")]
    public decimal Value { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    [ForeignKey("SeriesId")]
    public virtual Series? Series { get; set; }
}