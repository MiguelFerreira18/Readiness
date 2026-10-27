using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Type), IsUnique = true)]
public class Category
{
    [Key]
    public long Id { get; set; }

    [Required, MaxLength(50)]
    public CategoryType Type { get; set; }
}
