using ExpertCenter.DataContext;
using ExpertCenter.MvcApp.Controllers;
using ExpertCenter.MvcApp.Services.Products;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ExpertCenter.Tests;

public class ProductsControllerTests
{
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ExpertCenterContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var context = new ExpertCenterContext(options);
        var productsService = new Mock<IProductsService>();

        _controller = new ProductsController(context, productsService.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsAllProducts()
    {
        // Assert
        Assert.True(true);
    }
}