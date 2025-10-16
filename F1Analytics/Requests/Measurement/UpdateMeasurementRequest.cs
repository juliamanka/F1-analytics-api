using Swashbuckle.AspNetCore.Annotations;

namespace F1Analytics.Requests.Measurement;

public class UpdateMeasurementRequest
{
    [SwaggerParameter("Measured downforce value in Newtons (typically negative for downforce)")]
    public decimal? Value { get; set; }
    
    [SwaggerParameter("Date and time when the measurement was recorded (ISO 8601 format)")]
    public DateTime? Timestamp { get; set; }
    
    [SwaggerParameter("Identifier of the wing configuration series this measurement belongs to")]
    public string? SeriesId { get; set; } = string.Empty;
    
    [SwaggerParameter("Optional notes or comments about this measurement")]
    public string? Notes { get; set; }
    
}