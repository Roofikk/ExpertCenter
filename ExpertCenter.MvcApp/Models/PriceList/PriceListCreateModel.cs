using ExpertCenter.MvcApp.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.PriceList;

public class PriceListCreateModel
{
    [MaxLength(100, ErrorMessage = "Максимальная длина 100 символов")]
    [Display(Name = "Название")]
    public string Name { get; set; } = null!;
    [Display(Name = "Колонки")]
    public ICollection<ColumnCreateModel> Columns { get; set; } = [];
}
