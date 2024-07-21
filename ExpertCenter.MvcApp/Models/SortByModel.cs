namespace ExpertCenter.MvcApp.Models;

public class SortByModel
{
    public string ColumnId { get; set; } = null!;
    public string? ColumnName { get; set; }
    public bool IsDesc { get; set; } = false;
    public bool Selected { get; set; } = false;
}
