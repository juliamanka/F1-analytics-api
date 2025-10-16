namespace F1Analytics.DTOs;

public class SeriesDto
{
    public int Id { get; set; }
    public string SeriesId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MinValue { get; set; }
    public decimal MaxValue { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string? MeasurementType { get; set; }
    public int MeasurementCount { get; set; }
}