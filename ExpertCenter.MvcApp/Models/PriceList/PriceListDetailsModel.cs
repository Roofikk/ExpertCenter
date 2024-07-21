namespace ExpertCenter.MvcApp.Models.PriceList;

public class PriceListDetailsModel
{
    public int PriceListId { get; set; }
    public string PriceListName { get; set; } = null!;
    public ICollection<SortByModel> SortByModel { get; set; } = null!;
    public PaginationBarModel PaginationBarModel { get; set; } = null!;
    public Dictionary<ColumnViewModel, Dictionary<int, string>> ProdColumns { get; set; } = [];
    public IEnumerable<ProductDetailsModel> Products { get; set; } = [];
}

public class ProductDetailsModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Article { get; set; }
}

public class ColumnViewModel
{
    public string ColumnName { get; set; } = null!;
}
