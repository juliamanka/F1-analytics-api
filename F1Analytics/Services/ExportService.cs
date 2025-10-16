using System.Text;
using F1Analytics.DTOs;
using F1Analytics.Requests;

namespace F1Analytics.Services;

public class ExportService : IExportService
{
    public async Task<string> GeneratePrintableHtmlAsync(PagedResult<MeasurementDto> measurements, ExportMeasurementsRequest config)
    {
        var html = new StringBuilder();
        
        // HTML document structure
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html lang='en'>");
        html.AppendLine("<head>");
        html.AppendLine("    <meta charset='UTF-8'>");
        html.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
        html.AppendLine($"   <title>{config.Title}</title>");
        html.AppendLine("    <style>");
        html.AppendLine(GetPrintableStyles());
        html.AppendLine("    </style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");
        
        // Header
        html.AppendLine($"<div class='header'>");
        html.AppendLine($"    <h1>{config.Title}</h1>");
        html.AppendLine($"    <p class='export-info'>Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>");
        if (config.StartDate.HasValue || config.EndDate.HasValue)
        {
            html.AppendLine($"    <p class='date-range'>Date Range: {FormatDateRange(config.StartDate, config.EndDate)}</p>");
        }
        html.AppendLine("</div>");

        // Summary statistics
        if (config.IncludeSummary)
        {
            html.AppendLine(GenerateSummarySection(measurements.Items));
        }

        // Data table
        if (config.GroupBySeries)
        {
            html.AppendLine(GenerateGroupedTable(measurements.Items));
        }
        else
        {
            html.AppendLine(GenerateStandardTable(measurements.Items));
        }

        // Footer
        html.AppendLine("<div class='footer'>");
        html.AppendLine($"    <p>Total Records: {measurements.Items.Count} / Page: {measurements.Page} of {measurements.TotalPages}</p>");
        html.AppendLine("</div>");
        
        html.AppendLine("</body>");
        html.AppendLine("</html>");

        return html.ToString();
    }

    private string GetPrintableStyles()
    {
        return @"
            @media print {
                @page { margin: 1cm; size: A4; }
                body { font-family: 'Arial', sans-serif; font-size: 10pt; }
                .no-print { display: none !important; }
            }
            
            body { 
                font-family: 'Arial', sans-serif; 
                margin: 0; 
                padding: 20px; 
                background: white; 
                color: #333;
                line-height: 1.4;
            }
            
            .header { 
                text-align: center; 
                margin-bottom: 30px; 
                border-bottom: 2px solid #333;
                padding-bottom: 15px;
            }
            
            .header h1 { 
                color: #c41e3a; 
                margin: 0; 
                font-size: 24pt; 
                font-weight: bold;
            }
            
            .export-info, .date-range { 
                margin: 5px 0; 
                font-size: 10pt; 
                color: #666; 
            }
            
            .summary-section {
                margin: 20px 0;
                padding: 15px;
                background-color: #f8f9fa;
                border: 1px solid #ddd;
                border-radius: 5px;
            }
            
            .summary-section h2 {
                margin-top: 0;
                color: #333;
                font-size: 14pt;
            }
            
            .summary-stats {
                display: flex;
                justify-content: space-around;
                flex-wrap: wrap;
            }
            
            .stat-item {
                text-align: center;
                margin: 10px;
            }
            
            .stat-value {
                font-size: 16pt;
                font-weight: bold;
                color: #c41e3a;
            }
            
            .stat-label {
                font-size: 9pt;
                color: #666;
                margin-top: 5px;
            }
            
            table { 
                width: 100%; 
                border-collapse: collapse; 
                margin: 20px 0; 
                font-size: 9pt;
            }
            
            table.grouped {
                margin: 10px 0;
            }
            
            th, td { 
                border: 1px solid #ddd; 
                padding: 8px; 
                text-align: left; 
            }
            
            th { 
                background-color: #f5f5f5; 
                font-weight: bold; 
                color: #333;
            }
            
            .series-header {
                background-color: #e3f2fd;
                font-weight: bold;
                color: #1976d2;
            }
            
            .series-group {
                margin-bottom: 30px;
            }
            
            .series-title {
                color: #1976d2;
                font-size: 12pt;
                font-weight: bold;
                margin: 20px 0 10px 0;
            }
            
            .numeric { text-align: right; }
            .datetime { text-align: center; }
            
            tbody tr:nth-child(even) { background-color: #f9f9f9; }
            tbody tr:hover { background-color: #f0f0f0; }
            
            .footer { 
                margin-top: 30px; 
                text-align: center; 
                font-size: 8pt; 
                color: #666; 
                border-top: 1px solid #ddd;
                padding-top: 10px;
            }
            
            .page-break { page-break-before: always; }
        ";
    }

    private string GenerateSummarySection(List<MeasurementDto> measurements)
    {
        if (!measurements.Any()) return "";

        var summary = new StringBuilder();
        var groupedBySeries = measurements.GroupBy(m => m.SeriesName ?? "Unknown").ToList();

        summary.AppendLine("<div class='summary-section'>");
        summary.AppendLine("    <h2>Summary Statistics</h2>");
        summary.AppendLine("    <div class='summary-stats'>");
        summary.AppendLine($"       <div class='stat-item'>");
        summary.AppendLine($"           <div class='stat-value'>{measurements.Count}</div>");
        summary.AppendLine($"           <div class='stat-label'>Total Measurements</div>");
        summary.AppendLine($"       </div>");
        summary.AppendLine($"       <div class='stat-item'>");
        summary.AppendLine($"           <div class='stat-value'>{groupedBySeries.Count}</div>");
        summary.AppendLine($"           <div class='stat-label'>Series</div>");
        summary.AppendLine($"       </div>");
        summary.AppendLine($"       <div class='stat-item'>");
        summary.AppendLine($"           <div class='stat-value'>{measurements.Min(m => m.Value):F1} N</div>");
        summary.AppendLine($"           <div class='stat-label'>Min Downforce</div>");
        summary.AppendLine($"       </div>");
        summary.AppendLine($"       <div class='stat-item'>");
        summary.AppendLine($"           <div class='stat-value'>{measurements.Max(m => m.Value):F1} N</div>");
        summary.AppendLine($"           <div class='stat-label'>Max Downforce</div>");
        summary.AppendLine($"       </div>");
        summary.AppendLine($"       <div class='stat-item'>");
        summary.AppendLine($"           <div class='stat-value'>{measurements.Average(m => m.Value):F1} N</div>");
        summary.AppendLine($"           <div class='stat-label'>Avg Downforce</div>");
        summary.AppendLine($"       </div>");
        summary.AppendLine("    </div>");
        summary.AppendLine("</div>");

        return summary.ToString();
    }

    private string GenerateGroupedTable(List<MeasurementDto> measurements)
    {
        var html = new StringBuilder();
        var groupedBySeries = measurements.GroupBy(m => m.SeriesName ?? "Unknown")
                                        .OrderBy(g => g.Key);

        foreach (var seriesGroup in groupedBySeries)
        {
            html.AppendLine("<div class='series-group'>");
            html.AppendLine($"    <h3 class='series-title'>{seriesGroup.Key} ({seriesGroup.Count()} measurements)</h3>");
            html.AppendLine("    <table class='grouped'>");
            html.AppendLine("        <thead>");
            html.AppendLine("            <tr>");
            html.AppendLine("                <th>Timestamp</th>");
            html.AppendLine("                <th>Downforce (N)</th>");
            html.AppendLine("            </tr>");
            html.AppendLine("        </thead>");
            html.AppendLine("        <tbody>");

            foreach (var measurement in seriesGroup.OrderBy(m => m.Timestamp))
            {
                html.AppendLine("            <tr>");
                html.AppendLine($"               <td class='datetime'>{measurement.Timestamp:yyyy-MM-dd HH:mm:ss}</td>");
                html.AppendLine($"               <td class='numeric'>{measurement.Value:F2}</td>");
                html.AppendLine("            </tr>");
            }

            html.AppendLine("        </tbody>");
            html.AppendLine("    </table>");
            html.AppendLine("</div>");
        }

        return html.ToString();
    }

    private string GenerateStandardTable(List<MeasurementDto> measurements)
    {
        var html = new StringBuilder();
        
        html.AppendLine("<table>");
        html.AppendLine("    <thead>");
        html.AppendLine("        <tr>");
        html.AppendLine("            <th>Series</th>");
        html.AppendLine("            <th>Timestamp</th>");
        html.AppendLine("            <th>Downforce (N)</th>");
        html.AppendLine("        </tr>");
        html.AppendLine("    </thead>");
        html.AppendLine("    <tbody>");

        foreach (var measurement in measurements)
        {
            html.AppendLine("        <tr>");
            html.AppendLine($"           <td>{measurement.SeriesName ?? "Unknown"}</td>");
            html.AppendLine($"           <td class='datetime'>{measurement.Timestamp:yyyy-MM-dd HH:mm:ss}</td>");
            html.AppendLine($"           <td class='numeric'>{measurement.Value:F2}</td>");
            html.AppendLine("        </tr>");
        }

        html.AppendLine("    </tbody>");
        html.AppendLine("</table>");

        return html.ToString();
    }

    private string FormatDateRange(DateTime? start, DateTime? end)
    {
        if (start.HasValue && end.HasValue)
            return $"{start.Value:yyyy-MM-dd} to {end.Value:yyyy-MM-dd}";
        if (start.HasValue)
            return $"From {start.Value:yyyy-MM-dd}";
        if (end.HasValue)
            return $"Until {end.Value:yyyy-MM-dd}";
        return "All dates";
    }
}