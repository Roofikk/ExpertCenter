using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models;
using ExpertCenter.MvcApp.Models.Column;

namespace ExpertCenter.MvcApp.Services.Products;

public interface IProductsService
{
    public Task<int?> GetRandomArticleAsync(int priceListId);
    public Task<IQueryable<Product>> GetProductsQueryAsync(int? priceListId = null, SortByModel? sortBy = null);
    public Task<Product> CreateAsync(ProductCreateModel model);
}
