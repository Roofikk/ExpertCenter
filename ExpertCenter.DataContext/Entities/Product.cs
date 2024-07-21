using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class Product
{
    public int ProductId { get; set; }
    [Column(TypeName = "varchar(50)")]
    public string Name { get; set; } = null!;
    public int Article { get; set; }

    public int PriceListId { get; set; }

    public virtual PriceList PriceList { get; set; } = null!;
    public virtual ICollection<ColumnValueBase> ColumnValues { get; } = [];
}
