using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class Column
{
    public int Id { get; set; }
    [Column(TypeName = "varchar(50)")]
    public virtual string Name { get; set; } = null!;

    public string ColumnTypeId { get; set; } = null!;
    public ColumnType ColumnType { get; set; } = null!;
    public virtual ICollection<PriceList> PriceLists { get; set; } = [];
    public virtual ICollection<ColumnValueBase> ColumnValues { get; set; } = [];
}
