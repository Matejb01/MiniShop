using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniShop.Data;
using MiniShop.Dtos;

namespace MiniShop.Controllers.Api;

[ApiController]
[Route("api/products")]
public class ProductsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /api/productsapi?categoryId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] int? categoryId)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.IsActive);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var items = await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                DescriptionHtml = p.DescriptionHtml,
                ImagePath = p.ImagePath,
                IsActive = p.IsActive,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : ""
            })
            .ToListAsync();

        return Ok(items);
    }

    // GET: /api/productsapi/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var p = await _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (p == null)
            return NotFound();

        var dto = new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            DescriptionHtml = p.DescriptionHtml,
            ImagePath = p.ImagePath,
            IsActive = p.IsActive,
            CategoryId = p.CategoryId,
            CategoryName = p.Category != null ? p.Category.Name : ""
        };

        return Ok(dto);
    }
}
