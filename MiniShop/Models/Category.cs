using System.ComponentModel.DataAnnotations;

namespace MiniShop.Models;

public class Category
{
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Slug { get; set; }

    public List<Product> Products { get; set; } = new();
}

