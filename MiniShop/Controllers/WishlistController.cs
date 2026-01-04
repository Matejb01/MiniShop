using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniShop.Data;
using MiniShop.Models;

namespace MiniShop.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public WishlistController(
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Wishlist
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);

        var items = await _context.WishlistItems
            .Include(w => w.Product)
            .ThenInclude(p => p.Category)
            .Where(w => w.UserId == userId)
            .ToListAsync();

        return View(items);
    }

    // POST: /Wishlist/Add/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId)
    {
        var userId = _userManager.GetUserId(User);

        var exists = await _context.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (!exists)
        {
            _context.WishlistItems.Add(new WishlistItem
            {
                UserId = userId!,
                ProductId = productId
            });

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Shop");
    }

    // POST: /Wishlist/Remove/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = _userManager.GetUserId(User);

        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

        if (item != null)
        {
            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
