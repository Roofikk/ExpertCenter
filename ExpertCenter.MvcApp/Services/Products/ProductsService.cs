using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models;
using ExpertCenter.MvcApp.Models.Column;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace ExpertCenter.MvcApp.Services.Products;

public class ProductsService : IProductsService
{
    private readonly ExpertCenterContext _context;

    public ProductsService(ExpertCenterContext context)
    {
        _context = context;
    }

    public async Task<IQueryable<Product>> GetProductsAsync(int? priceListId = null, SortByModel? sortBy = null)
    {
        var query = _context.Products
            .Where(x => x.PriceListId == priceListId);

        if (sortBy != null)
        {
            switch (sortBy.ColumnId)
            {
                case "Name":
                    query = sortBy.IsDesc
                        ? query.OrderByDescending(x => x.Name)
                        : query.OrderBy(x => x.Name);
                    break;
                case "Article":
                    query = sortBy.IsDesc
                        ? query.OrderByDescending(x => x.Article)
                        : query.OrderBy(x => x.Article);
                    break;
                default:
                    var existingColumnSortBy = await _context.Columns.FirstOrDefaultAsync(x =>
                        x.Id.ToString() == sortBy.ColumnId && x.PriceLists.Any(y => y.PriceListId == priceListId));

                    if (existingColumnSortBy != null)
                    {
                        var columnType = existingColumnSortBy.ColumnTypeId;

                        switch (columnType)
                        {
                            case nameof(IntColumn):
                                query = sortBy.IsDesc
                                    ? query.OrderByDescending(x =>
                                        x.ColumnValues
                                            .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                            .Select(x => ((IntColumn)x).Value).First())
                                    : query.OrderBy(x =>
                                        x.ColumnValues
                                            .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                            .Select(x => ((IntColumn)x).Value).First());
                                break;
                            case nameof(StringTextColumn):
                                query = sortBy.IsDesc
                                    ? query.OrderByDescending(x =>
                                        x.ColumnValues
                                            .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                            .Select(x => ((StringTextColumn)x).Value).First())
                                    : query.OrderBy(x =>
                                        x.ColumnValues
                                            .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                            .Select(x => ((StringTextColumn)x).Value).First());
                                break;
                            case nameof(VarCharColumn):
                                query = sortBy.IsDesc
                                    ? query.OrderByDescending(x =>
                                        x.ColumnValues
                                            .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                            .Select(x => ((VarCharColumn)x).Value).First())
                                    : query.OrderBy(x =>
                                        x.ColumnValues
                                            .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                            .Select(x => ((VarCharColumn)x).Value).First());
                                break;
                            default:
                                throw new Exception();
                        }
                    }
                    else
                    {
                        query = sortBy.IsDesc
                            ? query
                                .OrderByDescending(x => x.ProductId)
                            : query
                                .OrderBy(x => x.ProductId);
                    }
                    break;
            }
        }

        return query;
    }

    public async Task<Product> CreateAsync(ProductCreateModel model)
    {
        var createdProduct = new Product
        {
            PriceListId = model.PriceListId,
            Name = model.ProductName,
            Article = model.Article,
        };

        foreach (var column in model.Columns)
        {
            switch (column.ColumnTypeId)
            {
                case nameof(IntColumn):
                    if (!int.TryParse(column.Value, out var intValue))
                    {
                        throw new ArgumentOutOfRangeException("Неверный тип данных", column.ColumnTypeId);
                    }

                    createdProduct.ColumnValues.Add(new IntColumn
                    {
                        ColumnId = column.ColumnId,
                        Value = intValue
                    });
                    break;
                case nameof(VarCharColumn):
                    createdProduct.ColumnValues.Add(new VarCharColumn
                    {
                        ColumnId = column.ColumnId,
                        Value = column.Value
                    });
                    break;
                case nameof(StringTextColumn):
                    createdProduct.ColumnValues.Add(new StringTextColumn
                    {
                        ColumnId = column.ColumnId,
                        Value = column.Value
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Неверный тип данных", column.ColumnTypeId);
            }
        }

        return (await _context.Products.AddAsync(createdProduct)).Entity;
    }
}
