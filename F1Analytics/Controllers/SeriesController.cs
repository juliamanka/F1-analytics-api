using F1Analytics.Database.Repositories;
using F1Analytics.DTOs;
using F1Analytics.Requests.Series;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace F1Analytics.Controllers
{
    [ApiController]
    [Route("api/series")]
    [SwaggerTag("Managing F1 wing configuration series for aerodynamic testing. " +
                "Series represent different front wing configurations tested in wind tunnel facilities, " +
                "each containing metadata like expected downforce ranges, visual properties, and measurement types. " +
                "Data modification operations require user authentication. " +
                "Each series acts as a container for multiple measurements, enabling organized analysis " +
                "of different wing designs and their aerodynamic performance characteristics.")]
    public class SeriesController : ControllerBase
    {
        private readonly ISeriesRepository _seriesRepository;

        public SeriesController(ISeriesRepository seriesRepository)
        {
            _seriesRepository = seriesRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Get all series",
            Description = "Retrieves all wing configuration series with metadata and measurement counts.",
            OperationId = "GetAllSeries"
        )]
        [Produces("application/json")]
        [SwaggerResponse(200, "Series list returned")]
        public async Task<ActionResult<List<SeriesDto>>> GetAllSeries()
        {
            var seriesList = await _seriesRepository.GetAllSeriesAsync();
            return Ok(seriesList);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [SwaggerResponse(404, "Series with given id not found")]
        [SwaggerResponse(200, "Series successfully returned")]
        [SwaggerOperation(
            Summary = "Get series by ID",
            Description = "Retrieves detailed information for a specific wing configuration series.",
            OperationId = "GetSeriesById"
        )]
        public async Task<ActionResult<SeriesDto>> GetSeriesById(int id)
        {
            var series = await _seriesRepository.GetSeriesByIdAsync(id);
            if (series == null) return NotFound();
            return Ok(series);
        }

        [HttpPost]
        [Authorize]
        [Produces("application/json")]
        [Consumes("application/json")]
        [SwaggerResponse(400, "Error in request body")]
        [SwaggerResponse(201, "New series created")]
        [SwaggerOperation(
            Summary = "Create new series",
            Description =
                "Creates a new wing configuration series for aerodynamic testing. Requires authentication.",
            OperationId = "CreateSeries"
        )]
        public async Task<ActionResult> CreateSeries([FromBody] CreateSeriesRequest req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _seriesRepository.CreateSeriesAsync(req);

            return CreatedAtAction(nameof(GetSeriesById), req);
        }
        
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces("application/json")]
        [SwaggerResponse(404, "Series with given id not found")]
        [SwaggerResponse(204, "Series deleted")]
        [SwaggerOperation(
            Summary = "Delete series",
            Description = "Deletes an existing wing configuration series by ID. Requires authentication."
        )]
        public async Task<ActionResult> DeleteSeries(int id)
        {
            var existingSeries = await _seriesRepository.GetSeriesByIdAsync(id);
            if (existingSeries == null)
                return NotFound();

            await _seriesRepository.DeleteSeriesAsync(id);
            return NoContent();
        }
        
        
        [HttpPut("{id:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Produces("application/json")]
        [Consumes("application/json")]
        [SwaggerResponse(400, "Error in request body")]
        [SwaggerResponse(404, "Series with given id not found")]
        [SwaggerResponse(200, "Series updated")]
        [SwaggerOperation(
            Summary = "Update series configuration",
            Description =
                "Updates wing configuration details including expected value ranges, description, and visual properties"
        )]
        public async Task<ActionResult> UpdateSeries(int id, [FromBody] UpdateSeriesRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingSeries = await _seriesRepository.GetSeriesByIdAsync(id);
            if (existingSeries == null)
                return NotFound();

            if (!string.IsNullOrEmpty(request.Name))
                existingSeries.Name = request.Name;

            if (request.Description != null)
                existingSeries.Description = request.Description;

            if (request.MinValue.HasValue)
                existingSeries.MinValue = request.MinValue.Value;

            if (request.MaxValue.HasValue)
                existingSeries.MaxValue = request.MaxValue.Value;

            if (!string.IsNullOrEmpty(request.Unit))
                existingSeries.Unit = request.Unit;

            if (!string.IsNullOrEmpty(request.Color))
                existingSeries.Color = request.Color;

            if (!string.IsNullOrEmpty(request.MeasurementType))
                existingSeries.MeasurementType = request.MeasurementType;

            await _seriesRepository.UpdateSeriesAsync(id, existingSeries);
            return Ok(existingSeries);
        }
    }
}