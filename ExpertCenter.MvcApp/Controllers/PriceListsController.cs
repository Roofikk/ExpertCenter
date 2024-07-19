using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models.PriceList;

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
            .Select(x => new PriceListViewModel
            {
                PriceListId = x.PriceListId,
                Name = x.Name
            }).ToListAsync());
    }

    // GET: PriceLists/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (!await _context.PriceList.AnyAsync(x => x.PriceListId == id))
        {
            return NotFound();
        }

        var priceList = await _context.PriceList
            .Include(x => x.Products)
            .ThenInclude(x => x.ColumnValues)
            .Include(x => x.Columns)
            .ThenInclude(x => x.ColumnType)
            .SingleAsync(x => x.PriceListId == id);

        var model = new PriceListDetailsModel
        {
            PriceListId = priceList.PriceListId,
            PriceListName = priceList.Name,
            Products = priceList.Products.Select(x => new ProductDetailsModel
            {
                ProductName = x.Name,
                Article = x.Article,
            })
        };

        if (priceList == null)
        {
            return NotFound();
        }

        return View(model);
    }

    // GET: PriceLists/Create
    public IActionResult Create()
    {
        ViewBag.ColumnTypes = new List<SelectListItem>
        {
            new SelectListItem
            {
                Text = "Числовой",
                Value = nameof(IntColumn)
            },
            new SelectListItem
            {
                Text = "Однострочный",
                Value = nameof(VarCharColumn)
            },
            new SelectListItem
            {
                Text = "Многострочный",
                Value = nameof(StringTextColumn)
            }
        };

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
            return View(priceList);
        }

        var priceListEntity = new PriceList
        {
            Name = priceList.Name
        };

        var columnTypes = priceList.Columns.Select(c => c.ColumnType).ToList();

        if (columnTypes.Count != 0 && !await _context.ColumnTypes.AnyAsync(c => columnTypes.Any(t => t == c.ColumnTypeId)))
        {
            ModelState.AddModelError(nameof(priceList.Columns), "Некорректные типы данных");
            return View(priceList);
        }

        foreach (var column in priceList.Columns)
        {
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
            }
        }

        await _context.PriceList.AddAsync(priceListEntity);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
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
        var priceList = await _context.PriceList.FindAsync(id);
        if (priceList != null)
        {
            _context.PriceList.Remove(priceList);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PriceListExists(int id)
    {
        return _context.PriceList.Any(e => e.PriceListId == id);
    }
}
