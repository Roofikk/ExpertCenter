using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
using ExpertCenter.MvcApp.Models.Column;
using ExpertCenter.MvcApp.Models.Product;

namespace ExpertCenter.MvcApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ExpertCenterContext _context;

        public ProductsController(ExpertCenterContext context)
        {
            _context = context;
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.PriceList)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create/5
        public async Task<IActionResult> Create(int? priceListId)
        {
            if (priceListId == null || !await _context.PriceList.AnyAsync(x => x.PriceListId == priceListId))
            {
                return NotFound();
            }

            var columns = await _context.Columns
                .Where(x => x.PriceLists.Any(y => y.PriceListId == priceListId))
                .Include(x => x.ColumnType)
                .ToListAsync();

            return View(new ProductCreateModel
            {
                PriceListId = priceListId.Value,
                Columns = columns.Select(x => new ProductCreateColumnModel
                {
                    ColumnId = x.Id,
                    ColumnTypeId = x.ColumnTypeId,
                    ColumnName = x.Name,
                }).ToList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateModel product)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError($"Columns", "Некорректное значение");
                return View(product);
            }

            if (await _context.Product.AnyAsync(x => x.PriceListId == product.PriceListId && x.Article == product.Article))
            {
                ModelState.AddModelError("Article", "Такой артикул уже существует");
                return View(product);
            }

            var createdProduct = new Product
            {
                PriceListId = product.PriceListId,
                Name = product.ProductName,
                Article = product.Article,
            };

            foreach (var column in product.Columns)
            {
                switch (column.ColumnTypeId)
                {
                    case nameof(IntColumn):
                        if (!int.TryParse(column.Value, out var intValue))
                        {
                            ModelState.AddModelError("Columns", "Некорректное значение");
                            ViewData["PriceListId"] = new SelectList(_context.PriceList, "PriceListId", "Name", product.PriceListId);
                            return View(product);
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
                }
            }

            await _context.Product.AddAsync(createdProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "PriceLists", new { id = product.PriceListId });
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.PriceList)
                .Select(x => new ProductDeleteModel
                {
                    ProductId = x.ProductId,
                    ProductName = x.Name,
                    PriceListName = x.PriceList.Name
                })
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return PartialView("Product/_Delete", product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ProductDeleteModel model)
        {
            if (!await _context.Product.AnyAsync(x => x.ProductId == model.ProductId))
            {
                return NotFound();
            }

            await _context.Product.Where(x => x.ProductId == model.ProductId).ExecuteDeleteAsync();

            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "PriceLists", new
            {
                id = model.PriceListId,
                pageIndex = model.PageIndex,
                sortBy = model.SortByModel?.ColumnId ?? "default",
                isDesc = model.SortByModel?.IsDesc ?? false
            });
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
