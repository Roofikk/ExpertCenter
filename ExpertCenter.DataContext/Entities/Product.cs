using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class Product
{
    public int ProductId { get; set; }
    public int Article { get; set; }
    [Column(TypeName = "varchar(50)")]
    public string Name { get; set; } = null!;
    [Column(TypeName = "varchar(255)")]
    public string? Description { get; set; }
    [Column(TypeName = "varchar(255)")]
    public string? KeyWords { get; set; }
    [Column(TypeName = "real")]
    public decimal Price { get; set; }

    public int PriceListId { get; set; }
    public PriceList PriceList { get; set; } = null!;
}
