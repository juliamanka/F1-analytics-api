using F1Analytics.DTOs;

namespace F1Analytics.Requests;

public class ExportMeasurementsRequest
{
    public List<string>? SeriesIds { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string Title { get; set; } = "F1 Aerodynamic Measurements Report";
    
    public bool IncludeSummary { get; set; } = true;
    
    public bool GroupBySeries { get; set; } = true;
    
    public int MaxRecords { get; set; } = 1000;
    
    public string SortBy { get; set; } = "timestamp";
    
    public string SortDirection { get; set; } = "asc";

    public MeasurementFilterParameters ToFilterParameters()
    {
        return new MeasurementFilterParameters
        {
            StartDate = StartDate,
            EndDate = EndDate,
            Page = 1,
            PageSize = MaxRecords,
            SortBy = SortBy,
            SortDirection = SortDirection
        };
    }
}