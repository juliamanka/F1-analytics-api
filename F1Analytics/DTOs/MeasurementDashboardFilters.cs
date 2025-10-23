using System.Text.Json.Serialization;

namespace F1Analytics.DTOs;

public class MeasurementDashboardFilters
{
    public List<string>? SeriesIds { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
}