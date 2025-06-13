using Plates.Shared;
using WebMVC.ViewModels;

namespace WebMVC.Services;

public interface IPlateService
{
    Task<PagedViewModel<PlateDTO>> GetPlates(int pageNum, int pageSize);
    Task<PagedViewModel<PlateDTO>> GetPlates(int pageNum, int pageSize, string sortColumn, string sortDirection, string searchString);
    
    Task<bool> Create(PlateDTO plate);
}
