
using Catalog.API.Helpers;
using Catalog.API.Models;
using Plates.Shared;
using System.Linq;
using System.Net;
using System.Text;
using static Humanizer.On;

namespace Catalog.API.Services;


/// <summary>
/// service to return plate/reg data which is "paged" and has supplied filter applied
/// </summary>
/// <remarks>
/// ideally there would be another injected service that interacts with the sql db directly.
/// i.e. this service wouldn't use ApplicationDbContext _context
/// </remarks>
public class PlatesService : IPlatesService
{

    private readonly IPlateRepository _plateRepository;
    public PlatesService(IPlateRepository plateRepository)
    {
        _plateRepository = plateRepository;
    }

    public async Task<PagedResult<Plate>> GetAsync(int pageNum = 1, int pageSize = 20)
    {
        return await _plateRepository.GetAsync(pageNum, pageSize);
    }

    public async Task<PagedResult<Plate>> GetAsync(int pageNum = 1, 
                                                   int pageSize = 20,
                                                   string sortColumn = "Registration",
                                                   string sortDirection = "asc",
                                                   string filter = null)
    {
        return await _plateRepository.GetAsync(pageNum, pageSize, sortColumn, sortDirection, filter);
    }


    public async Task<APIStatus> CreateAsync(PlateDTO plateDTO)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(plateDTO.Registration))
                return new APIStatus(HttpStatusCode.BadRequest, "Bad reg");

            plateDTO.Registration = plateDTO.Registration.ToUpper();

            if (await _plateRepository.ExistsAsync(plateDTO.Registration))
                return new APIStatus(HttpStatusCode.Conflict, "Reg already exists");

            var newPlate = CreatePlate(plateDTO);

            await _plateRepository.CreateAsync(newPlate);

            return new APIStatus(HttpStatusCode.NoContent ,""); //204
        }
        catch (Exception ex)
        {
            //log it
            return new APIStatus(HttpStatusCode.InternalServerError, "Service failed");
        }
    }

    private Plate CreatePlate (PlateDTO plateDTO)
    {
        return new Plate
        {
            Id = Guid.NewGuid(),
            Registration = plateDTO.Registration,
            SalePrice = plateDTO.SalePrice,
            PurchasePrice = plateDTO.PurchasePrice,
            Numbers = PlateHelper.GetDigits(plateDTO.Registration),
            Letters = PlateHelper.GetLetters(plateDTO.Registration)
        };
    }
}
