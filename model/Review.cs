using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Review
{
    [Key]
    public long Id { get; set; }

    [MaxLength(4000)]
    public string? Text { get; set; }

    [Range(1, 5), Required]
    public int Star { get; set; }

    [Required]
    public float TimeToRead { get; set; }

    public DateTime DateOfReview { get; set; } = DateTime.UtcNow;

    public Boolean IsDeleted { get; set; } = false;

    public Boolean wasEdited { get; set; } = false;

    [Required]
    public long BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;

    public long UserId { get; set; }

    [ForeignKey(nameof(UserId)), Required]
    public User User { get; set; } = null!;
}
