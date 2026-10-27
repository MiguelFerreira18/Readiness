using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class BookCategory
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;

    [Required]
    public long CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;
}
