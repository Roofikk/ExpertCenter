using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models.PriceList;
using Microsoft.EntityFrameworkCore;

namespace ExpertCenter.MvcApp.Services.PriceLists;

public class PriceListsService : IPriceListsService
{
    private readonly ExpertCenterContext _context;

    public PriceListsService(ExpertCenterContext context)
    {
        _context = context;
    }

    public async Task<PriceList> CreateAsync(PriceListCreateModel model)
    {
        var priceListEntity = new PriceList
        {
            Name = model.Name
        };

        var existingColumns = await _context.Columns
            .Where(c => model.Columns.Select(t => t.ColumnId).Any(t => c.Id == t))
            .ToListAsync();

        foreach (var column in model.Columns)
        {
            if (column.ColumnId != null)
            {
                var existingColumn = existingColumns
                    .FirstOrDefault(x => x.Id == column.ColumnId) ??
                    throw new ArgumentException($"Неизвестный тип данных. " +
                        $"ID: {column.ColumnId}, Название: {column.ColumnName ?? "пусто"}, тип: {column.ColumnType ?? "пусто"}", "Columns");

                priceListEntity.Columns.Add(existingColumn);
                continue;
            }

            switch (column.ColumnType)
            {
                case nameof(IntColumn):
                    priceListEntity.Columns.Add(new Column
                    {
                        ColumnTypeId = nameof(IntColumn),
                        Name = column.ColumnName
                    });
                    break;
                case nameof(VarCharColumn):
                    priceListEntity.Columns.Add(new Column
                    {
                        ColumnTypeId = nameof(VarCharColumn),
                        Name = column.ColumnName
                    });
                    break;
                case nameof(StringTextColumn):
                    priceListEntity.Columns.Add(new Column
                    {
                        ColumnTypeId = nameof(StringTextColumn),
                        Name = column.ColumnName
                    });
                    break;
                default:
                    throw new ArgumentException($"Неизвестный тип данных. Название: {column.ColumnName}, тип: {column.ColumnType}", "ColumnTypes");
            }
        }

        try
        {
            return (await _context.PriceLists.AddAsync(priceListEntity)).Entity;
        }
        catch (DbUpdateException)
        {
            throw;
        }
    }
}
