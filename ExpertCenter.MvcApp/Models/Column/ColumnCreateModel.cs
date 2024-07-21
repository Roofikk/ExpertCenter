using System.ComponentModel.DataAnnotations;

namespace ExpertCenter.MvcApp.Models.Column;

public class ColumnCreateModel
{
    public int? ColumnId { get; set; }
    public int Number { get; set; }
    [Display(Name = "Название колонки")]
    [MaxLength(50, ErrorMessage = "Максимальная длина 50 символов")]
    [Required(ErrorMessage = "Поле не может быть пустым")]
    public string ColumnName { get; set; } = null!;
    public string ColumnType { get; set; } = null!;
}
