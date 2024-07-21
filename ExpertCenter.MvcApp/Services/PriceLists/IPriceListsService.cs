using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models.PriceList;

namespace ExpertCenter.MvcApp.Services.PriceLists;

public interface IPriceListsService
{
    public Task<PriceList> CreateAsync(PriceListCreateModel model);
}
