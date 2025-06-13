using System.ComponentModel.DataAnnotations;

namespace Plates.Shared;

public class PlateDTO
{
    public string? Registration { get; set; }

    [DataType(DataType.Currency)]
    public decimal PurchasePrice { get; set; }
    [DataType(DataType.Currency)]
    public decimal SalePrice { get; set; }

}
