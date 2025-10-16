using Swashbuckle.AspNetCore.Annotations;

namespace F1Analytics.Requests.Series;

public class UpdateSeriesRequest
{
    [SwaggerParameter("Display name for the wing configuration")]
    public string? Name { get; set; } = string.Empty;
    
    [SwaggerParameter("Detailed description of the wing configuration characteristics")]
    public string? Description { get; set; }
    
    [SwaggerParameter("Minimum expected downforce value in Newtons (negative number)")]
    public decimal? MinValue { get; set; }
    
    [SwaggerParameter("Maximum expected downforce value in Newtons (negative number)")]
    public decimal?MaxValue { get; set; }
    
    [SwaggerParameter("Unit of measurement for the values")]
    public string? Unit { get; set; } = "N";
    
    [SwaggerParameter("Hex color code for visual representation in charts")]
    public string? Color { get; set; }
    
    [SwaggerParameter("Category or type of measurement being recorded")]
    public string? MeasurementType { get; set; } = "Front Wing Downforce";
    
}