using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Name), IsUnique = true)]
public class Category
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
}
