using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.DataContext.Entities;
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

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var expertCenterContext = _context.Product.Include(p => p.PriceList);
            return View(await expertCenterContext.ToListAsync());
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
            if (!await _context.PriceList.AnyAsync(x => x.PriceListId == priceListId))
            {
                return NotFound();
            }

            var columns = await _context.Columns
                .Where(x => x.PriceListId == priceListId)
                .Include(x => x.ColumnType)
                .ToListAsync();

            return View(new ProductCreateModel
            {
                Columns = columns.Select(x => new ProductCreateColumnModel
                {
                    ColumnId = x.Id,
                    ColumnTypeId = x.ColumnTypeId,
                    ColumnName = x.Name,
                }).ToList()
            });
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateModel product)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PriceListId"] = new SelectList(_context.PriceList, "PriceListId", "Name", product.PriceListId);
                return View(product);
            }

            if (await _context.Product.AnyAsync(x => x.PriceListId == product.PriceListId && x.Article == product.Article))
            {
                ModelState.AddModelError(nameof(product.Article), "Товар с таким артикулом уже существует");
                ViewData["PriceListId"] = new SelectList(_context.PriceList, "PriceListId", "Name", product.PriceListId);
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
                            ModelState.AddModelError(nameof(column.Value), "Некорректное значение");
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

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["PriceListId"] = new SelectList(_context.PriceList, "PriceListId", "Name", product.PriceListId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Article,PriceListId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["PriceListId"] = new SelectList(_context.PriceList, "PriceListId", "Name", product.PriceListId);
            return View(product);
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
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
