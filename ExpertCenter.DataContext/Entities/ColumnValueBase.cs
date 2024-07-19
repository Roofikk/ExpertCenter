namespace ExpertCenter.DataContext.Entities;

public abstract class ColumnValueBase
{
    public int ProductId { get; set; }
    public int ColumnId { get; set; }

    public virtual Product Product { get; set; } = null!;
    public virtual Column Column { get; set; } = null!;
}
