using ExpertCenter.MvcApp.Models.PriceList;
using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.Product;

public class ProductCreateModel
{
    public int PriceListId { get; set; }
    [Display(Name = "Название товара")]
    [Required(ErrorMessage = "Необходимо указать название товара")]
    public string ProductName { get; set; } = null!;
    [Display(Name = "Артикул")]
    [Required(ErrorMessage = "Необходимо указать артикул")]
    public int Article { get; set; }
    public List<ProductCreateColumnModel> Columns { get; set; } = [];
}

public class ProductCreateColumnModel
{
    public int ColumnId { get; set; }
    public string ColumnTypeId { get; set; } = null!;
    public string ColumnName { get; set; } = null!;
    public string Value { get; set; } = null!;
}
