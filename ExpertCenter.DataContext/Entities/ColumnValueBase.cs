using System.Runtime.CompilerServices;

namespace ExpertCenter.DataContext.Entities;

public abstract class ColumnValueBase
{
    public int ProductId { get; set; }
    public int ColumnId { get; set; }

    public string? Value
    {
        get
        {
            return Column.ColumnTypeId switch
            {
                nameof(IntColumn) => ((IntColumn)this).Value.ToString(),
                nameof(VarCharColumn) => ((VarCharColumn)this).Value,
                nameof(StringTextColumn) => ((StringTextColumn)this).Value,
                _ => null
            };
        }
    }

    public virtual Product Product { get; set; } = null!;
    public virtual Column Column { get; set; } = null!;
}
