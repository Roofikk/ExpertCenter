using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models.PriceList;
using ExpertCenter.MvcApp.Models;

namespace ExpertCenter.MvcApp.Controllers;

public class PriceListsController : Controller
{
    private readonly ExpertCenterContext _context;

    public PriceListsController(ExpertCenterContext context)
    {
        _context = context;
    }

    // GET: PriceLists
    public async Task<IActionResult> Index()
    {
        return View(await _context.PriceList
            .OrderByDescending(x => x.PriceListId)
            .Select(x => new PriceListViewModel
            {
                PriceListId = x.PriceListId,
                Name = x.Name
            }).ToListAsync());
    }

    // GET: PriceLists/Details/5
    public async Task<IActionResult> Details(int? id, int? pageIndex, string? sortBy = "default", bool isDesc = false)
    {
        pageIndex ??= 1;

        if (id == null)
        {
            return NotFound();
        }

        if (!await _context.PriceList.AnyAsync(x => x.PriceListId == id))
        {
            return NotFound();
        }

        var priceList = await _context.PriceList
            .Include(x => x.Columns)
            .ThenInclude(x => x.ColumnValues)
            .SingleAsync(x => x.PriceListId == id);

        var productsQuery = _context.Product
            .Where(x => x.PriceListId == id);

        switch (sortBy)
        {
            case "Name":
                productsQuery = isDesc
                    ? productsQuery.OrderByDescending(x => x.Name)
                    : productsQuery.OrderBy(x => x.Name);
                break;
            case "Article":
                productsQuery = isDesc
                    ? productsQuery.OrderByDescending(x => x.Article)
                    : productsQuery.OrderBy(x => x.Article);
                break;
            default:
                var existingColumnSortBy = priceList.Columns.FirstOrDefault(x => x.Id.ToString() == sortBy);

                if (existingColumnSortBy != null)
                {
                    var columnType = existingColumnSortBy.ColumnTypeId;

                    switch (columnType)
                    {
                        case nameof(IntColumn):
                            productsQuery = isDesc
                                ? productsQuery.OrderByDescending(x =>
                                    x.ColumnValues
                                        .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                        .Select(x => ((IntColumn)x).Value).First())
                                : productsQuery.OrderBy(x => 
                                    x.ColumnValues
                                        .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                        .Select(x => ((IntColumn)x).Value).First());
                            break;
                        case nameof(StringTextColumn):
                            productsQuery = isDesc
                                ? productsQuery.OrderByDescending(x =>
                                    x.ColumnValues
                                        .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                        .Select(x => ((StringTextColumn)x).Value).First())
                                : productsQuery.OrderBy(x =>
                                    x.ColumnValues
                                        .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                        .Select(x => ((StringTextColumn)x).Value).First());
                            break;
                        case nameof(VarCharColumn):
                            productsQuery = isDesc
                                ? productsQuery.OrderByDescending(x =>
                                    x.ColumnValues
                                        .Where(x => x.ColumnId == existingColumnSortBy.Id)
                                        .Select(x => ((VarCharColumn)x).Value).First())
                                : productsQuery.OrderBy(x =>
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
                    productsQuery = isDesc
                        ? productsQuery
                            .OrderByDescending(x => x.ProductId)
                        : productsQuery
                            .OrderBy(x => x.ProductId);
                }

                break;
        }

        var dict = new Dictionary<ColumnViewModel, Dictionary<int, string>>(priceList.Columns.Count);

        foreach (var column in priceList.Columns)
        {
            dict.Add(
                new ColumnViewModel
                {
                    ColumnName = column.Name
                },
                column.ColumnValues.Select(x => new
                {
                    ProdId = x.ProductId,
                    Value = x switch
                    {
                        IntColumn intCol => intCol.Value.ToString(),
                        StringTextColumn stringCol => stringCol.Value,
                        VarCharColumn varCharCol => varCharCol.Value,
                        _ => throw new Exception()
                    }
                }).ToDictionary(x => x.ProdId, x => x.Value)
            );
        }

        var model = new PriceListDetailsModel
        {
            PaginationBarModel = new PaginationBarModel
            {
                CurrentPage = pageIndex.Value,
                TotalPages = (int)Math.Ceiling((double)await productsQuery.CountAsync() / 10),
                ActionName = "Details",
                ControllerName = "PriceLists",
                RouteValues = new Dictionary<string, string>
                {
                    {
                        "id",
                        id.ToString()!
                    },
                    {
                        "sortBy",
                        sortBy!
                    },
                    {
                        "isDesc",
                        isDesc.ToString()
                    }
                }
            },
            PriceListId = priceList.PriceListId,
            PriceListName = priceList.Name,
            ProdColumns = dict,
        };

        model.SortByModel =
        [
            new SortByModel
            {
                ColumnId = "Name",
                ColumnName = "Наименование",
                IsDesc = isDesc,
                Selected = sortBy == "Name",
            },
            new SortByModel
            {
                ColumnId = "Article",
                ColumnName = "Артикул",
                IsDesc = isDesc,
                Selected = sortBy == "Article",
            },
            .. priceList.Columns.Select(x => new SortByModel
            {
                ColumnId = x.Id.ToString(),
                ColumnName = x.Name,
                IsDesc = isDesc,
                Selected = sortBy == x.Id.ToString(),
            }),
        ];

        model.Products = await productsQuery
            .Skip((pageIndex.Value - 1) * 10)
            .Take(10)
            .Select(x => new ProductDetailsModel
            {
                Article = x.Article,
                ProductId = x.ProductId,
                ProductName = x.Name,
            })
            .ToListAsync();

        if (priceList == null)
        {
            return NotFound();
        }

        return View(model);
    }

    private async Task CreateViewBag()
    {
        ViewBag.ColumnTypes = await _context.ColumnTypes.Select(x => new SelectListItem
        {
            Text = x.DisplayName,
            Value = x.ColumnTypeId
        }).ToListAsync();

        ViewBag.ExistingColumns = await _context.Columns
            .Include(x => x.ColumnType)
            .Select(x => new
            {
                x.Id,
                x.Name,
                Type = x.ColumnType.DisplayName
            })
            .ToListAsync();
    }

    // GET: PriceLists/Create
    public async Task<IActionResult> Create()
    {
        await CreateViewBag();
        return View();
    }

    // POST: PriceLists/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PriceListCreateModel priceList)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Заполните все обязательные поля");
            await CreateViewBag();
            return View(priceList);
        }

        var priceListEntity = new PriceList
        {
            Name = priceList.Name
        };

        var columnTypes = priceList.Columns
            .Where(c => c.ColumnId == null)
            .Select(c => c.ColumnType).ToList();

        if (columnTypes.Count != 0 && !await _context.ColumnTypes.AnyAsync(c => columnTypes.Any(t => t == c.ColumnTypeId)))
        {
            ModelState.AddModelError("", "Неизвестный тип данных");
            await CreateViewBag();
            return View(priceList);
        }

        foreach (var column in priceList.Columns)
        {
            if (column.ColumnId != null)
            {
                var existingColumn = await _context.Columns
                    .FirstOrDefaultAsync(x => x.Id == column.ColumnId);

                if (existingColumn == null)
                {
                    ModelState.AddModelError("", "Неизвестный тип данных");
                    await CreateViewBag();
                    return View(priceList);
                }

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
                    return BadRequest(new
                    {
                        Errors = new[]
                        {
                            new
                            {
                                ErrorMessage = "Неизвестный тип данных",
                            }
                        }
                    });
            }
        }

        var entity = await _context.PriceList.AddAsync(priceListEntity);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = entity.Entity.PriceListId });
    }

    // GET: PriceLists/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceList = await _context.PriceList.FindAsync(id);
        if (priceList == null)
        {
            return NotFound();
        }
        return View(priceList);
    }

    // POST: PriceLists/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PriceListId,Name")] PriceList priceList)
    {
        if (id != priceList.PriceListId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(priceList);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PriceListExists(priceList.PriceListId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(priceList);
    }

    // GET: PriceLists/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceList = await _context.PriceList
            .FirstOrDefaultAsync(m => m.PriceListId == id);
        if (priceList == null)
        {
            return NotFound();
        }

        return View(priceList);
    }

    // POST: PriceLists/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await _context.PriceList.AnyAsync(x => x.PriceListId == id))
        {
            return NotFound();
        }

        await _context.PriceList.Where(x => x.PriceListId == id).ExecuteDeleteAsync();

        if (_context.ChangeTracker.HasChanges())
        {
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PriceListExists(int id)
    {
        return _context.PriceList.Any(e => e.PriceListId == id);
    }
}
