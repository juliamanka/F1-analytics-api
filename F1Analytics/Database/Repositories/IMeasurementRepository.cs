using F1Analytics.DTOs;
using F1Analytics.Requests.Measurement;

namespace F1Analytics.Database.Repositories;

public interface IMeasurementRepository
{
    public Task<PagedResult<MeasurementDto>> GetFilteredMeasurementsAsync(MeasurementFilterParameters filters);
    public Task<MeasurementDto?> GetMeasurementByIdAsync(int id);
    public Task CreateMeasurementAsync(CreateMeasurementRequest measurementDto);
    public Task<MeasurementDto?> UpdateMeasurementAsync(int id, UpdateMeasurementRequest measurementDto);
    public Task<bool> DeleteMeasurementAsync(int id);

}