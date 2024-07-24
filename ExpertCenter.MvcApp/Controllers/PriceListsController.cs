using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models.PriceList;
using ExpertCenter.MvcApp.Models;
using ExpertCenter.MvcApp.Services.Products;
using ExpertCenter.MvcApp.Services.PriceLists;
using ExpertCenter.MvcApp.Models.Product;

namespace ExpertCenter.MvcApp.Controllers;

public class PriceListsController : Controller
{
    private readonly ExpertCenterContext _context;
    private readonly IPriceListsService _priceListsService;
    private readonly IProductsService _productsService;

    public PriceListsController(ExpertCenterContext context,
        IProductsService productsService, IPriceListsService priceListsService)
    {
        _context = context;
        _productsService = productsService;
        _priceListsService = priceListsService;
    }

    /// <summary>
    /// GET: /PriceLists
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> Index()
    {
        return View(await _context.PriceLists
            .AsNoTracking()
            .OrderByDescending(x => x.PriceListId)
            .Select(x => new PriceListViewModel
            {
                PriceListId = x.PriceListId,
                Name = x.Name
            }).ToListAsync());
    }

    /// <summary>
    /// GET: PriceLists/Details/5
    /// </summary>
    /// <param name="id">ID прайс листа</param>
    /// <param name="pageIndex">номер страницы</param>
    /// <param name="sortBy">условие сортировки</param>
    /// <param name="isDesc">порядок сортировки</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<IActionResult> Details(int? id, int? pageIndex = 1, string? sortBy = "default", bool isDesc = false)
    {
        pageIndex ??= 1;
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;

        if (id == null)
        {
            return NotFound();
        }

        if (!await _context.PriceLists.AnyAsync(x => x.PriceListId == id))
        {
            return NotFound();
        }

        var priceList = await _context.PriceLists
            .AsNoTracking()
            .Include(x => x.Columns)
            .ThenInclude(x => x.ColumnValues)
            .SingleAsync(x => x.PriceListId == id);

        var dict = new Dictionary<ColumnViewModel, Dictionary<int, string>>(priceList.Columns.Count);

        foreach (var column in priceList.Columns)
        {
            dict.Add(
                new ColumnViewModel
                {
                    ColumnId = column.Id,
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

        var productsQuery = await _productsService.GetProductsQueryAsync(id, new SortByModel
        {
            ColumnId = sortBy ?? "default",
            IsDesc = isDesc
        });

        var totalPages = (int)Math.Ceiling((double)await productsQuery.CountAsync() / 10) == 0 
            ? 1 : (int)Math.Ceiling((double)await productsQuery.CountAsync() / 10);
        var currentPage = pageIndex.Value <= totalPages ? pageIndex.Value : totalPages;

        var model = new PriceListDetailsModel
        {
            PaginationBarModel = new PaginationBarModel
            {
                CurrentPage = currentPage,
                TotalPages = totalPages,
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
            .Skip((currentPage - 1) * 10)
            .Take(10)
            .AsNoTracking()
            .Select(x => new ProductViewIndexModel
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

    /// <summary>
    /// GET: PriceLists/DetailsRaw/5<br />
    /// Для предоставления данных в формате JSON при сокетном подключении
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pageIndex"></param>
    /// <param name="sortBy"></param>
    /// <param name="isDesc"></param>
    /// <returns></returns>
    public async Task<IActionResult> DetailsRaw(int id, int? pageIndex = 1, string? sortBy = "default", bool isDesc = false)
    {
        pageIndex ??= 1;

        var productsQuery = await _productsService.GetProductsQueryAsync(id, new SortByModel
        {
            ColumnId = sortBy ?? "default",
            IsDesc = isDesc
        });

        var totalPages = (int)Math.Ceiling((double)await productsQuery.CountAsync() / 10);
        var currentPage = pageIndex.Value <= totalPages ? pageIndex.Value : totalPages;

        var result = new
        {
            Products = await productsQuery
                .Include(x => x.ColumnValues)
                .ThenInclude(x => x.Column)
                .AsNoTracking()
                .Skip((currentPage - 1) * 10)
                .Take(10)
                .Select(x => new ProductDetailModel
                {
                    Article = x.Article,
                    ProductId = x.ProductId,
                    ProductName = x.Name,
                    Columns = x.ColumnValues.Select(x => new ProductColumnDetailModel
                    {
                        ColumnId = x.ColumnId,
                        Value = x.Value,
                    })
                })
                .ToListAsync(),
            PaginationBarModel = new PaginationBarModel
            {
                ActionName = "Details",
                ControllerName = "PriceLists",
                CurrentPage = currentPage,
                TotalPages = totalPages,
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
            }
        };

        return Json(result);
    }

    // GET: PriceLists/Create
    public async Task<IActionResult> Create()
    {
        await CreateViewBagWithExistingColumns();
        return View();
    }

    /// <summary>
    /// POST: PriceLists/Create
    /// </summary>
    /// <param name="priceList">модель создания прайс листа</param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PriceListCreateModel priceList)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Заполните все обязательные поля");
            return BadRequest(ModelState);
        }

        try
        {
            var entity = await _priceListsService.CreateAsync(priceList);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = entity.PriceListId });
        }
        catch (Exception e)
        {
            ModelState.AddModelError($"Ошибка при создании прайс листа: {priceList.Name}", e.Message);
            return BadRequest(ModelState);
        }
    }

    /// <summary>
    /// GET: PriceLists/Edit/5
    /// </summary>
    /// <param name="id">id прайс листа</param>
    /// <returns></returns>
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceList = await _context.PriceLists.FindAsync(id);
        if (priceList == null)
        {
            return NotFound();
        }

        return View(priceList);
    }

    /// <summary>
    /// POST: PriceLists/Edit/5
    /// </summary>
    /// <param name="id">id прайс листа</param>
    /// <param name="priceList">сущность прайс листа</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PriceListId,Name")] PriceList priceList)
    {
        if (id != priceList.PriceListId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(priceList);
        }

        if (!await _context.PriceLists.AnyAsync(x => x.PriceListId == id))
        {
            return NotFound();
        }

        try
        {
            _context.Update(priceList);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new Exception($"PriceList {id} update error: {ex.Message}");
        }

        return RedirectToAction(nameof(Index));
    }

    // GET: PriceLists/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var priceList = await _context.PriceLists
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
        // Данный вариант не работает с тестами. Пока не разобрался почему LINQ ExecuteDeleteAsync не работает именно в тестах.
        // При запуске приложения удаление работает нормально.
        //if (!await _context.PriceLists.AnyAsync(x => x.PriceListId == id))
        //{
        //    return NotFound();
        //}

        //await _context.PriceLists.Where(x => x.PriceListId == id).ExecuteDeleteAsync();

        var priceList = await _context.PriceLists.FindAsync(id);

        if (priceList == null)
        {
            return NotFound();
        }

        _context.PriceLists.Remove(priceList);

        if (_context.ChangeTracker.HasChanges())
        {
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task CreateViewBagWithExistingColumns()
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
}
