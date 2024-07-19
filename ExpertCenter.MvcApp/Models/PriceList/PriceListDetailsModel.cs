using ExpertCenter.DataContext.Entities;

namespace ExpertCenter.MvcApp.Models.PriceList;

public class PriceListDetailsModel
{
    public int PriceListId { get; set; }
    public string PriceListName { get; set; } = null!;
    public IEnumerable<ProductDetailsModel> Products { get; set; } = [];
}

public class ProductDetailsModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int Article { get; set; }
    public IEnumerable<ColumnViewModel> Columns { get; set; } = [];
}

public class ColumnViewModel
{
    public int ColumnId { get; set; }
    public string ColumnName { get; set; } = null!;
    public string Value { get; set; } = null!;
}
