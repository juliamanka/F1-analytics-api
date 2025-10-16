using F1Analytics.Database.Models;
using F1Analytics.DTOs;
using F1Analytics.Requests.Measurement;
using Microsoft.EntityFrameworkCore;

namespace F1Analytics.Database.Repositories;

public class MeasurementRepository : IMeasurementRepository
{
    private readonly F1AnalyticsDbContext _context;

    public MeasurementRepository(F1AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<MeasurementDto>> GetFilteredMeasurementsAsync(MeasurementFilterParameters filters)
    {
        var query = _context.Measurements
            .Include(m => m.Series)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filters.SeriesId))
        {
            query = query.Where(m => m.SeriesId == filters.SeriesId);
        }

        if (filters.StartDate.HasValue)
            query = query.Where(m => m.Timestamp >= filters.StartDate);

        if (filters.EndDate.HasValue)
            query = query.Where(m => m.Timestamp <= filters.EndDate);

        query = filters.SortBy?.ToLower() switch
        {
            "value" => filters.SortDirection == "desc"
                ? query.OrderByDescending(m => m.Value)
                : query.OrderBy(m => m.Value),
            "series" => filters.SortDirection == "desc"
                ? query.OrderByDescending(m => m.Series.Name)
                : query.OrderBy(m => m.Series.Name),
            _ => filters.SortDirection == "desc"
                ? query.OrderByDescending(m => m.Timestamp)
                : query.OrderBy(m => m.Timestamp)
        };

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filters.Page - 1) * filters.PageSize)
            .Take(filters.PageSize)
            .Select(m => new MeasurementDto
            {
                Id = m.Id,
                SeriesId = m.SeriesId,
                Value = m.Value,
                Timestamp = m.Timestamp,
                SeriesName = m.Series.Name,
                Unit = m.Series.Unit,
                Color = m.Series.Color
            })
            .ToListAsync();

        return new PagedResult<MeasurementDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filters.Page,
            PageSize = filters.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
        };
    }

    public async Task<MeasurementDto?> GetMeasurementByIdAsync(int id)
    {
        var measurement = await _context.Measurements
            .Include(m => m.Series)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (measurement == null)
            return null;

        return new MeasurementDto
        {
            Id = measurement.Id,
            SeriesId = measurement.SeriesId,
            Value = measurement.Value,
            Timestamp = measurement.Timestamp,
            SeriesName = measurement.Series.Name,
            Unit = measurement.Series.Unit,
            Color = measurement.Series.Color
        };
    }

    public async Task CreateMeasurementAsync(CreateMeasurementRequest measurementDto)
    {
        await _context.Measurements.AddAsync(new Measurement()
        {
            SeriesId = measurementDto.SeriesId,
            Value = measurementDto.Value,
            Timestamp = measurementDto.Timestamp,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteMeasurementAsync(int id)
    {
        var measurement = await _context.Measurements.FirstOrDefaultAsync(m => m.Id == id);
        if (measurement == null)
            return false;

        _context.Measurements.Remove(measurement);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<MeasurementDto?> UpdateMeasurementAsync(int id, UpdateMeasurementRequest request)
    {
        var measurement = await _context.Measurements
            .Include(m => m.Series)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (measurement == null)
            return null;

        if (request.Value.HasValue)
            measurement.Value = request.Value.Value;

        if (request.Timestamp.HasValue)
            measurement.Timestamp = request.Timestamp.Value;

        if (!string.IsNullOrEmpty(request.SeriesId))
        {
            var seriesExists = await _context.Series
                .AnyAsync(s => s.SeriesId == request.SeriesId);

            if (!seriesExists)
                throw new ArgumentException($"Series with ID '{request.SeriesId}' does not exist");

            measurement.SeriesId = request.SeriesId;
        }


        await _context.SaveChangesAsync();

        return new MeasurementDto
        {
            Id = measurement.Id,
            SeriesId = measurement.SeriesId,
            Value = measurement.Value,
            Timestamp = measurement.Timestamp,
            SeriesName = measurement.Series?.Name,
        };
    }
}