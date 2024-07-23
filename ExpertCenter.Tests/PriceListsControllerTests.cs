using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Controllers;
using ExpertCenter.MvcApp.Models.Column;
using ExpertCenter.MvcApp.Models.PriceList;
using ExpertCenter.MvcApp.Services.PriceLists;
using ExpertCenter.MvcApp.Services.Products;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ExpertCenter.Tests;

public class PriceListsControllerTests
{
    private readonly ExpertCenterContext _context;
    private readonly PriceListsController _controller;

    public PriceListsControllerTests()
    {
        var options = new DbContextOptionsBuilder<ExpertCenterContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase-{Guid.NewGuid()}")
            .Options;
        _context = new ExpertCenterContext(options);
        var priceListsService = new Mock<PriceListsService>(MockBehavior.Default, _context);
        var productsService = new Mock<ProductsService>(MockBehavior.Default, _context);
        _controller = new PriceListsController(_context, productsService.Object, priceListsService.Object);
    }

    [Fact]
    public async Task GetPriceLists_ReturnsAllPriceLists()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<PriceListViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count());
        Assert.Equal("PriceList2", model.ElementAt(0).Name);
        Assert.Equal("PriceList1", model.ElementAt(1).Name);
    }

    [Fact]
    public async Task GetDetailsOfPriceList_ReturnDetails()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Details(1);

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<PriceListDetailsModel>(viewResult.Model);
        Assert.Equal("Product1", model.Products.ElementAt(0).ProductName);
        Assert.Equal("Product2", model.Products.ElementAt(1).ProductName);
        Assert.Equal(0, model.Products.ElementAt(0).Article);
        Assert.Equal(1, model.Products.ElementAt(1).Article);
    }

    [Fact]
    public async Task GetDetailsOfPriceList_ReturnsNotFound()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Details(3);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CreatePriceList_ReturnsView()
    {
        // Arrange
        await _context.InitializeAsync();

        // Act
        var result = await _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreatePriceList_ReturnsCreated()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList3",
        });

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(3, redirectToActionResult.RouteValues!["id"]);
    }

    [Fact]
    public async Task CreatePriceListWithDuplicateName_ShouldReturnsCreate()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList1",
        });

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(3, redirectToActionResult.RouteValues!["id"]);

        var viewResult = Assert.IsType<ViewResult>(await _controller.Details(3));
        var model = Assert.IsAssignableFrom<PriceListDetailsModel>(viewResult.Model);
        Assert.Equal("PriceList1", model.PriceListName);
    }

    [Fact]
    public async Task CreatePriceListWithColumns_ReturnsCreated()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList3",
            Columns = new List<ColumnCreateModel>
            {
                new ColumnCreateModel
                {
                    ColumnName = "Int column",
                    ColumnType = "IntColumn",
                },
                new ColumnCreateModel
                {
                    ColumnName = "String text column",
                    ColumnType = "StringTextColumn",
                },
            },
        });

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(3, redirectToActionResult.RouteValues!["id"]);

        var viewResult = Assert.IsType<ViewResult>(await _controller.Details(3));
        var model = Assert.IsAssignableFrom<PriceListDetailsModel>(viewResult.Model);
        Assert.Equal(2, model.ProdColumns.Keys.Count);
        Assert.Equal("Int column", model.ProdColumns.ElementAt(0).Key.ColumnName);
        Assert.Equal("String text column", model.ProdColumns.ElementAt(1).Key.ColumnName);
    }

    [Fact]
    public async Task CreatePriceListWithExistingColumns_ShouldCreated()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList3",
            Columns = new List<ColumnCreateModel>
            {
                new ColumnCreateModel
                {
                    ColumnId = 1,
                },
                new ColumnCreateModel
                {
                    ColumnId = 2,
                },
            },
        });

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(3, redirectToActionResult.RouteValues!["id"]);

        var viewResult = Assert.IsType<ViewResult>(await _controller.Details(3));
        var model = Assert.IsAssignableFrom<PriceListDetailsModel>(viewResult.Model);
        Assert.Equal(2, model.ProdColumns.Keys.Count);
        Assert.Equal("Column1", model.ProdColumns.ElementAt(0).Key.ColumnName);
        Assert.Equal("Column2", model.ProdColumns.ElementAt(1).Key.ColumnName);
    }

    [Fact]
    public async Task CreatePriceListWithExistingAndNewColumns_ShouldCreated()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList3",
            Columns = new List<ColumnCreateModel>
            {
                new ColumnCreateModel
                {
                    ColumnId = 1,
                },
                new ColumnCreateModel
                {
                    ColumnName = "String text column",
                    ColumnType = "StringTextColumn",
                },
            },
        });

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectToActionResult.ActionName);
        Assert.Equal(3, redirectToActionResult.RouteValues!["id"]);

        var viewResult = Assert.IsType<ViewResult>(await _controller.Details(3));
        var model = Assert.IsAssignableFrom<PriceListDetailsModel>(viewResult.Model);
        Assert.Equal(2, model.ProdColumns.Keys.Count);
        Assert.Equal("Column1", model.ProdColumns.ElementAt(0).Key.ColumnName);
        Assert.Equal("String text column", model.ProdColumns.ElementAt(1).Key.ColumnName);
    }

    [Fact]
    public async Task CreatePriceListWithInvalidColumnType_ShouldReturnsBadRequest()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList1",
            Columns = new List<ColumnCreateModel>
            {
                new ColumnCreateModel
                {
                    ColumnType = "RandomColumn",
                    ColumnName = "Name",
                },
            }
        });

        // Assert
        var viewResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, viewResult.StatusCode);
    }

    [Fact]
    public async Task CreatePriceListWithNonExistentColumn_ShouldReturnsBadRequest()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList1",
            Columns = new List<ColumnCreateModel>
            {
                new ColumnCreateModel
                {
                    ColumnId = 2534,
                },
            }
        });

        // Assert
        var viewResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, viewResult.StatusCode);
    }

    [Fact]
    public async Task DeletePriceList_ReturnDetails()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.DeleteConfirmed(1);

        // Assert
        Assert.IsType<RedirectToActionResult>(result);
        var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectToActionResult.ActionName);
    }

    [Fact]
    public async Task DeletePriceList_ReturnsNotFound()
    {
        // Arrange
        await InitializeAsync();

        // Act
        var result = await _controller.DeleteConfirmed(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    private async Task InitializeAsync()
    {
        await _context.InitializeAsync();

        await _context.PriceLists.AddRangeAsync(GetPriceLists());
        await _context.Columns.AddRangeAsync(GetColumns());
        await _context.SaveChangesAsync();
    }

    private List<PriceList> GetPriceLists()
    {
        return
        [
            new PriceList
            {
                Name = "PriceList1",
                Products = new List<Product>
                {
                    new Product
                    {
                        Name = "Product1",
                        Article = 0
                    },
                    new Product
                    {
                        Name = "Product2",
                        Article = 1,
                    }
                }
            },
            new PriceList
            {
                Name = "PriceList2",
                Products = new List<Product>
                {
                    new Product
                    {
                        Name = "Product3",
                        Article = 2,
                    },
                    new Product
                    {
                        Name = "Product4",
                        Article = 3,
                    }
                }
            }
        ];
    }

    private List<Column> GetColumns()
    {
        return
        [
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
        ];
    }
}
