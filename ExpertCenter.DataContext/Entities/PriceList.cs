using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class PriceList
{
    public int PriceListId { get; set; }
    [Column(TypeName = "varchar(100)")]
    public string Name { get; set; } = null!;

    public ICollection<ColumnType> Columns { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
}
