using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class Column
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(50)")]
    public virtual string Name { get; set; } = null!;

    public int PriceListId { get; set; }
    public string ColumnTypeId { get; set; } = null!;
    public ColumnType ColumnType { get; set; } = null!;
    public virtual PriceList PriceList { get; set; } = null!;
    public virtual ICollection<ColumnValueBase> ColumnValues { get; set; } = [];
}
