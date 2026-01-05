using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniShop.Data;

namespace MiniShop.Controllers;

public class ShopController : Controller
{
    private readonly ApplicationDbContext _context;

    public ShopController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Shop?categoryId=1
    public async Task<IActionResult> Index(int? categoryId)
    {
        // dropdown kategorija
        var categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewData["Categories"] = new SelectList(categories, "Id", "Name", categoryId);

        // proizvodi (filtriranje)
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(products);
    }

    // GET: /Shop/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null)
            return NotFound();

        return View(product);
    }
}
