using F1Analytics.Database.Models;
using F1Analytics.DTOs;
using F1Analytics.Requests.Series;
using Microsoft.EntityFrameworkCore;

namespace F1Analytics.Database.Repositories;

public class SeriesRepository : ISeriesRepository
{
    private readonly F1AnalyticsDbContext _context;
    
    public SeriesRepository(F1AnalyticsDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<SeriesDto>> GetAllSeriesAsync()
    {
        return await _context.Series.
            Include(s=>s.Measurements).
            Select(s=> new SeriesDto()
            {
                Id=s.Id,
                Name=s.Name,
                Color = s.Color,
                Description = s.Description,
                MaxValue = s.MaxValue,
                MeasurementCount = s.Measurements.Count,
                MeasurementType = s.MeasurementType,
                MinValue = s.MinValue,
                SeriesId = s.SeriesId,
                Unit = s.Unit
            }).ToListAsync();
    }
    
    public async Task<SeriesDto?> GetSeriesByIdAsync(int id)
    {
        var series = await _context.Series.Include(s => s.Measurements)
            .FirstOrDefaultAsync(s => s.Id == id);
        
        if(series == null) return null;
        
        return new SeriesDto()
            {
                Id=series.Id,
                Name=series.Name,
                Color = series.Color,
                Description = series.Description,
                MaxValue = series.MaxValue,
                MeasurementCount = series.Measurements.Count,
                MeasurementType = series.MeasurementType,
                MinValue = series.MinValue,
                SeriesId = series.SeriesId,
                Unit = series.Unit
            };
    }

    public async Task CreateSeriesAsync(CreateSeriesRequest seriesDto)
    {
        await _context.Series.AddAsync(new Series()
        {
            Name = seriesDto.Name,
            Color = seriesDto.Color,
            Description = seriesDto.Description,
            MaxValue = seriesDto.MaxValue,
            MeasurementType = seriesDto.MeasurementType,
            MinValue = seriesDto.MinValue,
            SeriesId = seriesDto.SeriesId,
            Unit = seriesDto.Unit,
            CreatedAt = DateTime.UtcNow
        });
        
        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateSeriesAsync(int id, SeriesDto seriesDto)
    {
        var series = await _context.Series.FirstAsync(s => s.Id == id);
        
        series.Name = seriesDto.Name;
        series.Color = seriesDto.Color;
        series.Description = seriesDto.Description;
        series.MaxValue = seriesDto.MaxValue;
        series.MeasurementType = seriesDto.MeasurementType;
        series.MinValue = seriesDto.MinValue;
        series.Unit = seriesDto.Unit;
        series.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
    }
}