using Catalog.API.Services;
using Plates.Shared;

namespace Catalog.API.Controllers;

[Produces("application/json")]
[Route("Catalog")]

public class CatalogController : ControllerBase
{
    private readonly IPlatesService _platesService;
    private readonly ILogger<CatalogController> _logger;
    public CatalogController(IPlatesService platesService,
                             ILogger<CatalogController> logger)
    {
        _platesService = platesService;
        _logger = logger;
    }

    //[HttpGet()]
    //public async Task<ActionResult<List<Plate>>> GetPlates()
    //{
    //    var plates = await _platesService.GetAllAsync();
    //    return Ok(plates);
    //}

    [HttpGet()]
    public async Task<ActionResult<PagedResult<Plate>>> GetPlates(
        int pageNum = 1, 
        int pageSize = 20, 
        string sortColumn = "Registration",
        string sortDirection = "asc",
        string filter = null)
    {
        try
        {
            //var plates = await _platesService.GetPlatesAsync(pageNum, pageSize);
            var plates = await _platesService.GetAsync(pageNum, pageSize, sortColumn, sortDirection, filter);
            return Ok(plates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error");
            return StatusCode(500, "Unexpected error");
        }
    }

    [HttpPut("{registration}")]
    public async Task<ActionResult> PutAsync(string registration, [FromBody] PlateDTO plateDTO)
    {
        var apiStatus = await _platesService.CreateAsync(plateDTO);

        return StatusCode(apiStatus.Status, apiStatus.Message);
    }


}
