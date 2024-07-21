using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models;
using ExpertCenter.MvcApp.Models.Column;

namespace ExpertCenter.MvcApp.Services.Products;

public interface IProductsService
{
    public Task<IQueryable<Product>> GetProductsAsync(int? priceListId = null, SortByModel? sortBy = null);
    public Task<Product> CreateAsync(ProductCreateModel model);
}
