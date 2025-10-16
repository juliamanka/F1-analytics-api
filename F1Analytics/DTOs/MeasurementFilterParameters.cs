namespace F1Analytics.DTOs;

public class MeasurementFilterParameters
{
        public string? SeriesId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } = "timestamp";
        public string? SortDirection { get; set; } = "asc";
    
}