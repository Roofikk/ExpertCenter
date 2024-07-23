using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.Column;

public class ProductCreateModel
{
    public int PriceListId { get; set; }
    [Required(ErrorMessage = "Поле не может быть пустым")]
    [Display(Name = "Название товара")]
    public string ProductName { get; set; } = null!;
    [Required(ErrorMessage = "Необходимо указать артикул")]
    [Display(Name = "Артикул")]
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
