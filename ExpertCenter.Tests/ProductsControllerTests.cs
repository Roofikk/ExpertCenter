using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Controllers;
using ExpertCenter.MvcApp.Models.Column;
using ExpertCenter.MvcApp.Models.Product;
using ExpertCenter.MvcApp.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ExpertCenter.Tests;

public class ProductsControllerTests
{
    private readonly ExpertCenterContext _context;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ExpertCenterContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase-{Guid.NewGuid()}")
            .Options;
        _context = new ExpertCenterContext(options);
        var productsService = new Mock<ProductsService>(MockBehavior.Default, _context);
        _controller = new ProductsController(_context, productsService.Object);
    }

    [Fact]
    public async Task CreateProduct_ReturnsView()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(1);

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreateProduct_ReturnsNotFound()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(3);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreated()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new ProductCreateModel
        {
            PriceListId = 1,
            ProductName = "Product1",
            Article = 0,
            Columns = new List<ProductCreateColumnModel>()
            {
                new ProductCreateColumnModel
                {
                    ColumnId = 1,
                    Value = "1",
                },
                new ProductCreateColumnModel
                {
                    ColumnId = 2,
                    Value = "2",
                },
                new ProductCreateColumnModel
                {
                    ColumnId = 3,
                    Value = "3",
                }
            }
        });

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal("PriceLists", redirectToActionResult.ControllerName);
        Assert.Equal(1, redirectToActionResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task CreateProductWithUnexistingPriceList_ReturnsNotFound()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new ProductCreateModel
        {
            PriceListId = 3,
            ProductName = "Product1",
            Article = 0
        });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CreateProductWithUnexistingColumns_ReturnsNotFound()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new ProductCreateModel
        {
            PriceListId = 1,
            ProductName = "Product1",
            Article = 3,
            Columns = new List<ProductCreateColumnModel>
            {
                new ProductCreateColumnModel
                {
                    ColumnId = 45,
                    ColumnTypeId = "IntColumn",
                    Value = "14325",
                }
            }
        });

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ReturnNoContent()
    {
        // Arrange
        await InitializeAsync();
        await _context.Products.AddAsync(new Product
        {
            ProductId = 1,
            PriceListId = 1,
            Name = "Product1",
            Article = 0
        });
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        var noContentResult = Assert.IsType<NoContentResult>(result);
        Assert.Equal(204, noContentResult.StatusCode);
    }

    [Fact]
    public async Task DeleteProduct_ReturnsNotFound()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Delete(3);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    private async Task InitializeAsync()
    {
        await _context.InitializeAsync();
        await _context.PriceLists.AddAsync(GetPriceList());
        await _context.SaveChangesAsync();
    }

    private PriceList GetPriceList()
    {
        return new PriceList
        {
            PriceListId = 1,
            Name = "PriceList1",
            Columns = GetColumns()
        };
    }

    private List<Column> GetColumns()
    {
        return new List<Column>()
        {
            new Column
            {
                Id = 1,
                ColumnTypeId = "IntColumn",
                Name = "Column1",
            },
            new Column
            {
                Id = 2,
                ColumnTypeId = "StringTextColumn",
                Name = "Column2",
            },
            new Column
            {
                Id = 3,
                ColumnTypeId = "VarCharColumn",
                Name = "Column3",
            }
        };
    }
}