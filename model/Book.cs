using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class Book
{
    [Key]
    public long Id { get; set; }
    
    [Required, MaxLength(255)]
    public string Name { get; set; } = null!;

    [MaxLength(4000)]
    public string? Summary { get; set; }

    [MaxLength(150), Required]
    public string? Publisher { get; set; }
    
    [Required]
    public int YearOfRelease { get; set; }

    [Range(1, int.MaxValue), Required]
    public int NumberOfPage { get; set; }
    
    [Required]
    public long Isbn { get; set; }

    public virtual List<BookCategory> BookCategories { get; set; } = new();
    public virtual List<Review> Reviews { get; set; } = new();
}
