using System.ComponentModel.DataAnnotations;

public sealed class RevokedToken
{
    [Key, MaxLength(36)]
    public string TokenId { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiresAtUtc { get; set; }
}
