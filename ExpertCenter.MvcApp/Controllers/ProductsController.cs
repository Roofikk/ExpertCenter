using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.MvcApp.Models.Column;
using ExpertCenter.MvcApp.Models.Product;
using ExpertCenter.MvcApp.Services.Products;

namespace ExpertCenter.MvcApp.Controllers;

public class ProductsController : Controller
{
    private readonly ExpertCenterContext _context;
    private readonly IProductsService _productsService;

    public ProductsController(ExpertCenterContext context, IProductsService productsService)
    {
        _context = context;
        _productsService = productsService;
    }

    // GET: Products/Create/5
    public async Task<IActionResult> Create(int? priceListId)
    {
        if (priceListId == null || !await _context.PriceLists.AnyAsync(x => x.PriceListId == priceListId))
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

        if (await _context.Products.AnyAsync(x => x.PriceListId == product.PriceListId && x.Article == product.Article))
        {
            ModelState.AddModelError("Article", "Такой артикул уже существует");
            return View(product);
        }

        try
        {
            await _productsService.CreateAsync(product);
        }
        catch (Exception e)
        {
            ModelState.AddModelError($"Ошибка при создании товара: {product.ProductName}", e.Message);
            return View(product);
        }

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

        var product = await _context.Products
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
        if (!await _context.Products.AnyAsync(x => x.ProductId == model.ProductId))
        {
            return NotFound();
        }

        await _context.Products.Where(x => x.ProductId == model.ProductId).ExecuteDeleteAsync();

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
        return _context.Products.Any(e => e.ProductId == id);
    }
}
