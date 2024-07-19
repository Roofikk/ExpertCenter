using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.PriceList;

public class PriceListViewModel
{
    [Display(Name = "#")]
    public int PriceListId { get; set; }
    [Display(Name = "Наименование")]
    public string Name { get; set; } = null!;
}
