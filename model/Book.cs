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

    [Required, MaxLength(150)]
    public string Author { get; set; } = null!;

    [MaxLength(150)]
    public string? Publisher { get; set; }

    public int YearOfRelease { get; set; }

    [Range(1, int.MaxValue)]
    public int NumberOfPage { get; set; }

    public long Isbn { get; set; }

    public List<BookCategory> BookCategories { get; set; } = new();
    public List<Review> reviews { get; set; } = new();
}
