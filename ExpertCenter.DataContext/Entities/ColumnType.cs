namespace ExpertCenter.DataContext.Entities;

public class ColumnType
{
    public string ColumnTypeId { get; set; } = null!;
    public string DisplayColumnName { get; set; } = null!;

    public virtual ICollection<Column> Columns { get; set; } = [];
}
