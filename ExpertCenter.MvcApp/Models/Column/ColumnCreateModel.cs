using ExpertCenter.DataContext.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.Product;

public class ColumnCreateModel
{
    [Display(Name = "Название колонки")]
    [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
    [Required(ErrorMessage = "Поле не может быть пустым")]
    public string ColumnName { get; set; } = null!;
    public SelectListItem ColumnType { get; set; } = null!;
}
