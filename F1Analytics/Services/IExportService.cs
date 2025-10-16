using F1Analytics.DTOs;
using F1Analytics.Requests;

namespace F1Analytics.Services;

public interface IExportService
{
    Task<string> GeneratePrintableHtmlAsync(PagedResult<MeasurementDto> measurements, ExportMeasurementsRequest config);
}