namespace ExpertCenter.MvcApp.Models.Product;

public class ProductDeleteModel
{
    public int ProductId { get; set; }
    public int PriceListId { get; set; }
    public string? ProductName { get; set; }
    public string? PriceListName { get; set; }
    public int PageIndex { get; set; }
    public SortByModel? SortByModel { get; set; }
}
