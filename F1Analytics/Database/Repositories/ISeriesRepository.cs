using F1Analytics.DTOs;
using F1Analytics.Requests.Series;

namespace F1Analytics.Database.Repositories;

public interface ISeriesRepository
{
    public Task<List<SeriesDto>> GetAllSeriesAsync();
    public Task<SeriesDto?> GetSeriesByIdAsync(int id);
    public Task CreateSeriesAsync(CreateSeriesRequest seriesDto);
    public Task UpdateSeriesAsync(int id, SeriesDto seriesDto);
    public Task DeleteSeriesAsync(int id);

}