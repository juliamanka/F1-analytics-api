namespace F1Analytics.Requests.Measurement;

public class CreateMeasurementRequest
{
    public string SeriesId { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
    
    public decimal Value { get; set; }
}