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
        // Act
        await _context.InitializeAsync();
        var priceLists = GetPriceLists();

        await _context.PriceLists.AddRangeAsync(priceLists);
        await _context.SaveChangesAsync();
        var priceListsService = new Mock<IPriceListsService>();
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
        // Act
        await _context.InitializeAsync();
        var priceLists = GetPriceLists();

        await _context.PriceLists.AddRangeAsync(priceLists);
        await _context.SaveChangesAsync();
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
        // Act
        await _context.InitializeAsync();
        var priceLists = GetPriceLists();

        await _context.PriceLists.AddRangeAsync(priceLists);
        await _context.SaveChangesAsync();
        var result = await _controller.Details(3);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        var notFoundResult = Assert.IsType<NotFoundResult>(result);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task CreatePriceList_ReturnsView()
    {
        // Act
        await _context.InitializeAsync();
        var result = await _controller.Create();

        // Assert
        Assert.IsType<ViewResult>(result);
        var viewResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task CreatePriceList_ReturnsCreated()
    {
        // Act
        await _context.InitializeAsync();

        await _context.PriceLists.AddRangeAsync(GetPriceLists());
        await _context.Columns.AddRangeAsync(GetColumns());
        await _context.SaveChangesAsync();

        // Assert
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
    public async Task CreatePriceList_ReturnsBadRequest()
    {
        // Act
        await _context.InitializeAsync();

        await _context.PriceLists.AddRangeAsync(GetPriceLists());
        await _context.Columns.AddRangeAsync(GetColumns());
        await _context.SaveChangesAsync();

        // Assert
        var result = await _controller.Create(new PriceListCreateModel
        {
            Name = "PriceList1",
            Columns = new List<ColumnCreateModel>
            {
                new ColumnCreateModel
                {
                    ColumnType = "RanodmColumn",
                    ColumnName = "Name",
                },
            }
        });

        // Assert
        Assert.IsType<BadRequestResult>(result);
        var badRequestResult = Assert.IsType<BadRequestResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
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
                ColumnTypeId = "IntColumn",
                Name = "Column1",
            },
            new Column
            {
                ColumnTypeId = "StringTextColumn",
                Name = "Column2",
            },
            new Column
            {
                ColumnTypeId = "VarCharColumn",
                Name = "Column3",
            }
        ];
    }
}
