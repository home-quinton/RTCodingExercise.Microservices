using Catalog.API.Models;
using Plates.Shared;

namespace Catalog.API.Services
{
    public interface IPlatesService
    {
        //Task<PagedResult<Plate>> GetAllAsync();
        Task<PagedResult<Plate>> GetAsync(int pageNum, int pageSize);
        Task<PagedResult<Plate>> GetAsync(int pageNum, int pageSize, string sortColumn, string sortDirection, string filter);

        Task<APIStatus> CreateAsync(PlateDTO plateDTO);
    }
}
