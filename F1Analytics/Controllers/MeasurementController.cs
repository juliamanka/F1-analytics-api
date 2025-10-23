using F1Analytics.Database.Repositories;
using F1Analytics.DTOs;
using F1Analytics.Requests;
using F1Analytics.Requests.Measurement;
using F1Analytics.Responses;
using F1Analytics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace F1Analytics.Controllers;

[ApiController]
[Route("api/measurement")]
[SwaggerTag("Managing aerodynamic measurements for the F1 Analytics platform. Handles reading, creating, modifying and deleting downforce measurements from wind tunnel tests." +
            "Data modification operations require user authentication." +
            " Measurements represent downforce values (in Newtons) captured during testing of different F1 front wing configurations in the wind tunnel." +
            "All measurement values are expressed in Newtons (N) and represent negative downforce.")]
public class MeasurementController : ControllerBase
{
    private readonly IMeasurementRepository _measurementRepository;
    private readonly IExportService _exportService; 

    public MeasurementController(IMeasurementRepository measurementRepository, IExportService exportService)
    {
        _measurementRepository = measurementRepository;
        _exportService = exportService;
    }
    
    [HttpGet]
    [SwaggerOperation(
        Summary = "Get filtered measurements",
        Description = "Retrieves F1 aerodynamic measurements from wind tunnel testing with advanced filtering, sorting, and pagination options.",
        OperationId = "GetPagedMeasurements"
    )]
    [Produces("application/json")]
    [AllowAnonymous]
    [SwaggerResponse(200, "Correctly filtered out list of measurements returned with pagination", typeof(PagedResult<MeasurementDto>))]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<PagedResult<MeasurementDto>>> GetPagedMeasurements(
        [FromQuery] MeasurementFilterParameters filters)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _measurementRepository.GetFilteredMeasurementsPagedAsync(filters);
        return Ok(result);
    }
    
    [HttpGet("all")]
    [SwaggerOperation(
        Summary = "Get filtered measurements",
        Description = "Retrieves all F1 aerodynamic measurements from wind tunnel testing with advanced filtering options, for dashboard.",
        OperationId = "GetAllMeasurements"
    )]
    [Produces("application/json")]
    [AllowAnonymous]
    [SwaggerResponse(200, "Correctly filtered out list of measurements returned")]
    [SwaggerResponse(400, "Bad request")]
    public async Task<ActionResult<List<MeasurementDto>>> GetMeasurements(
        [FromQuery] MeasurementDashboardFilters filters)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _measurementRepository.GetFilteredMeasurementsAsync(filters);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Get measurement by ID",
        Description = "Retrieves a specific aerodynamic measurement by its unique ID.",
        OperationId = "GetMeasurementById"
    )]
    [Produces("application/json")]
    [SwaggerResponse(200, "Measurement returned")]
    [SwaggerResponse(404, "Not found")]
    [AllowAnonymous]
    public async Task<ActionResult<MeasurementDto>> GetMeasurementById(int id)
    {
        var measurement = await _measurementRepository.GetMeasurementByIdAsync(id);
        if (measurement == null) return NotFound();
        return Ok(measurement);
    }
    
    [HttpPost]
    [Authorize]
    [Consumes("application/json")] 
    [Produces("application/json")] 
    [SwaggerOperation(
        Summary = "Create new measurement",
        Description = "Creates a new aerodynamic measurement entry. Requires authentication.",
        OperationId = "CreateMeasurement"
    )]
    [SwaggerResponse(204, "No content")]
    [SwaggerResponse(400, "Error in request body")]
    public async Task<ActionResult> CreateMeasurement([FromBody] CreateMeasurementRequest measurementDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _measurementRepository.CreateMeasurementAsync(measurementDto);
        return NoContent();
    }
    
    [HttpPut("{id:int}")]
    [Authorize]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Update measurement",
        Description = "Updates an existing aerodynamic measurement entry by ID. Requires authentication.",
        OperationId = "UpdateMeasurement"
    )]
    [SwaggerResponse(400, "Error in request body")]
    [SwaggerResponse(404, "Not found")]
    [SwaggerResponse(200, "Measurement successfully updated")]
    public async Task<ActionResult> UpdateMeasurement(int id, [FromBody] UpdateMeasurementRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var updatedMeasurement = await _measurementRepository.UpdateMeasurementAsync(id, request);
        
            if (updatedMeasurement == null)
                return NotFound($"Measurement with ID {id} not found");

            return Ok(updatedMeasurement);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
    
    [HttpDelete("{id:int}")]
    [Authorize]
    [Produces("application/json")]
    [SwaggerOperation(
        Summary = "Delete measurement",
        Description = "Deletes an existing aerodynamic measurement entry by ID.",
        OperationId = "DeleteMeasurement"
    )]
    [SwaggerResponse(204, "Measurement deleted successfully")]
    [SwaggerResponse(404, "Measurement not found")]
    [SwaggerResponse(500, "Internal server error")]
    public async Task<ActionResult> DeleteMeasurement(int id)
    {
        var existing = await _measurementRepository.GetMeasurementByIdAsync(id);
        if (existing == null) return NotFound();
    
        var deleted = await _measurementRepository.DeleteMeasurementAsync(id);
        if (!deleted) return StatusCode(500);
    
        return NoContent();
    }
   
    [HttpGet("export")]
    [AllowAnonymous] 
    [Produces("text/html")] 
    [SwaggerOperation(
        Summary = "Export measurements for printing",
        Description = "Generates a clean, printable HTML document with measurement data in tabular format. Optimized for printing without UI elements."
    )]
    [SwaggerResponse(200, "Returns printable HTML document", typeof(string))]
    [SwaggerResponse(400, "Invalid export parameters")]
    public async Task<IActionResult> ExportMeasurements([FromQuery] ExportMeasurementsRequest exportRequest)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var measurements = await _measurementRepository.GetFilteredMeasurementsPagedAsync(exportRequest.ToFilterParameters());
    
        var htmlContent = await _exportService.GeneratePrintableHtmlAsync(measurements, exportRequest);
    
        return Content(htmlContent, "text/html; charset=utf-8");
    }
}