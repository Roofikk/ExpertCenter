using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpertCenter.DataContext;
using ExpertCenter.MvcApp.Models.Column;
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
            Article = await _productsService.GetRandomArticleAsync(priceListId.Value) ?? 0,
            Columns = columns.Select(x => new ProductCreateColumnModel
            {
                ColumnId = x.Id,
                ColumnTypeId = x.ColumnTypeId,
                ColumnName = x.Name,
            }).ToList()
        });
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCreateModel product)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, ModelState.Values.SelectMany(x => x.Errors).First().ErrorMessage);
            return View(product);
        }

        if (!await _context.PriceLists.AnyAsync(x => x.PriceListId == product.PriceListId))
        {
            return NotFound("Несуществующий прайс-лист");
        }

        try
        {
            await _productsService.CreateAsync(product);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "PriceLists", new { id = product.PriceListId });
    }

    // GET: Products/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return NotFound();
        }

        _context.Products.Remove(product);

        if (_context.ChangeTracker.HasChanges())
        {
            await _context.SaveChangesAsync();
        }

        return NoContent();
    }
}
