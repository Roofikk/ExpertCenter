using ExpertCenter.DataContext;
using ExpertCenter.MvcApp.Services.Products;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ExpertCenter.MvcApp.Hubs;

public class ProductsHub : Hub
{
    private readonly ProductsService _productsService;

    public ProductsHub(IConfiguration configuration)
    {
        var connectionString = configuration["ConnectionStrings:DefaultConnection"];
        var optionsBuilder = new DbContextOptionsBuilder<ExpertCenterContext>();
        optionsBuilder.UseSqlServer(connectionString);

        var context = new ExpertCenterContext(optionsBuilder.Options);
        _productsService = new ProductsService(context);
    }

    public async Task SendProducts(int priceListId)
    {
        await Clients.All.SendAsync("ReceiveProducts", new
        {
            PriceListId = priceListId,
            Message = $"Price list has been updated"
        });
    }
}
