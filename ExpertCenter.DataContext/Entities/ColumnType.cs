using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class ColumnType
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(50)")]
    public virtual string Name { get; set; } = null!;

    public int PriceListId { get; set; }
    public virtual PriceList PriceList { get; set; } = null!;
    public virtual ICollection<ColumnValueBase> ColumnValues { get; set; } = [];
}
