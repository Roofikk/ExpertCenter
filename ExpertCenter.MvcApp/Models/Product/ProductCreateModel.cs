using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.Product;

public class ProductCreateModel
{
    [MaxLength(50, ErrorMessage = "Максимальная длина символов 50")]
    [Display(Name = "Название")]
    public string Name { get; set; } = null!;
    [Required]
    [Display(Name = "Артикул")]
    public int Article { get; set; }
    [MaxLength(255, ErrorMessage = "Максимальная длина символов 255")]
    [Display(Name = "Описание")]
    public string? Description { get; set; }
    [MaxLength(255, ErrorMessage = "Максимальная длина символов 255")]
    [Display(Name = "Ключевые слова")]
    public string? KeyWords { get; set; }
    [Range(0, double.PositiveInfinity, ErrorMessage = "Цена должна быть больше нуля")]
    [Display(Name = "Цена")]
    public double Price { get; set; }
    public int? PriceListId { get; set; }
}
