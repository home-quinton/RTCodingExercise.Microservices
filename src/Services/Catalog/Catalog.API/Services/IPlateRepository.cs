using Plates.Shared;

namespace Catalog.API.Services;

public interface IPlateRepository
{
    Task<PagedResult<Plate>> GetAsync(int pageNum, int pageSize);

    Task<PagedResult<Plate>> GetAsync(int pageNum, int pageSize, string sortColumn, string sortDirection, string filter);

    Task<bool> ExistsAsync(string registration);

    Task CreateAsync(Plate plate);

}
