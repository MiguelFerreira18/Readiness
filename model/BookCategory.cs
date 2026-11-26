using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class BookCategory
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long BookId { get; set; }
    
    [Required]
    public CategoryType Category { get; set; }

    [ForeignKey("BookId")]
    public Book Book { get; set; } = null!;
}
