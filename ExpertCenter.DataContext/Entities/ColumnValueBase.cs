namespace ExpertCenter.DataContext.Entities;

public abstract class ColumnValueBase
{
    public int ProductId { get; set; }
    public int ColumnId { get; set; }

    public virtual Product Product { get; set; } = null!;
    public virtual ColumnType Column { get; set; } = null!;
}
