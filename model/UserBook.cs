using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using readiness.model;

public class UserBook
{
    [Key]
    public long Id { get; set; }

    [Required]
    public long BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; } = null!;
    
    [Required]
    public long UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
    
    [Required]
    public long StatusId { get; set; }

    [ForeignKey(nameof(StatusId))]
    public Status Status { get; set; } = null!;
}
