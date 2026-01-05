using System.ComponentModel.DataAnnotations;

namespace MiniShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 999999)]
    public decimal Price { get; set; }

    // HTML opis iz RTE editora (TinyMCE/Quill)
    public string? DescriptionHtml { get; set; }

    // Putanja do slike (npr. /images/products/xxx.jpg)
    [StringLength(300)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    // Relacija prema kategoriji
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}

