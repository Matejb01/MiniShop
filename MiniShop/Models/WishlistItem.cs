using System.ComponentModel.DataAnnotations;

namespace MiniShop.Models;

public class WishlistItem
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

