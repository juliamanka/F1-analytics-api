namespace F1Analytics.QueryParams;

public class MeasurementQueryParameters
{
    public List<string> SeriesIds { get; set; } = new();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public string SortBy { get; set; } = "timestamp";
    public bool SortDescending { get; set; } = false;
}