namespace F1Analytics.DTOs;

public class MeasurementDto
{
    public int Id { get; set; }
    public string SeriesId { get; set; } = string.Empty;
    public string SeriesName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}