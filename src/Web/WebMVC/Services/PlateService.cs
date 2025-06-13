using Plates.Shared;
using WebMVC.Clients;
using WebMVC.Config;
using WebMVC.ViewModels;

namespace WebMVC.Services;

public class PlateService : IPlateService
{
    private readonly IPlateClient _httpClient;
    private readonly PlateServiceConfig _config;
    public PlateService(IPlateClient httpClient,
                        IOptions<PlateServiceConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
    }

     
    public async Task<PagedViewModel<PlateDTO>> GetPlates(int pageNum = 1, int pageSize = 20)
    {
        try
        {
            string endpoint = string.Format(_config.GetUrl, pageNum, pageSize);
            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            var plates = await response.Content.ReadFromJsonAsync<PagedResult<Plate>>() ?? new PagedResult<Plate>();

            //replace with automapper
            var vm = new PagedViewModel<PlateDTO>
            {
                CurrentPage = plates.PageNumber,
                TotalPages = plates.TotalPages,
                Items = plates.Items.Select(p => new PlateDTO
                {
                    Registration = p.Registration,
                    PurchasePrice = p.PurchasePrice,
                    SalePrice = Math.Round(p.SalePrice * 1.2M, 2)    //config this 20% factor
                }).ToList()
            };

            return vm;
        }
        catch (Exception e)
        {
            //log it
        }
        return null;
    }

    public async Task<PagedViewModel<PlateDTO>> GetPlates(int pageNum = 1, 
                                                          int pageSize = 20, 
                                                          string sortColumn = "", 
                                                          string sortDirection = "asc",
                                                          string searchString = null)
    {
        try
        {
            PagedResult<Plate>? plates = null;

            string endpoint = string.Format(_config.GetSortedUrl, pageNum, pageSize, sortColumn, sortDirection, searchString);
            var response = await _httpClient.GetAsync(endpoint);

            response.EnsureSuccessStatusCode();

            plates = await response.Content.ReadFromJsonAsync<PagedResult<Plate>>();

            //replace with automapper
            var vm = new PagedViewModel<PlateDTO>
            {
                CurrentPage = plates.PageNumber,
                TotalPages = plates.TotalPages,
                SortColumn = plates.SortColumn,
                SortDirection = plates.SortDirection,
                Filter = plates.Filter,
                Items = plates.Items.Select(p => new PlateDTO
                {
                    Registration = p.Registration,
                    PurchasePrice = p.PurchasePrice,
                    SalePrice = Math.Round(p.SalePrice * 1.2M, 2)    //config this 20% factor
                }).ToList()
            };

            return vm;
        }
        catch (Exception e)
        {
            //log it
        }

        return new PagedViewModel<PlateDTO>();  //return empty list for failure
    }


    public async Task<bool> Create(PlateDTO plateDTO)
    {
        try
        {
            string endpoint = string.Format(_config.PutUrl, plateDTO.Registration);

            var response = await _httpClient.PutAsJsonAsync<PlateDTO>(endpoint, plateDTO);

            response.EnsureSuccessStatusCode();

            return true;

        }
        catch (Exception e)
        {
            //log it
            return false;
        }
    }

}
