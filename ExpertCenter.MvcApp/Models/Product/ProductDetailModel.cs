using ExpertCenter.MvcApp.Models.PriceList;

namespace ExpertCenter.MvcApp.Models.Product;

public class ProductDetailModel : ProductViewIndexModel
{
    public IEnumerable<ProductColumnDetailModel> Columns { get; set; } = [];
}

public class ProductColumnDetailModel
{
    public int ColumnId { get; set; }
    public string Value { get; set; } = null!;
}
