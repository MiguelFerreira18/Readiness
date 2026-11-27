namespace readiness.model;

public class UserBook
{
    // Chave primária desta tabela de ligação
    public long Id { get; set; }

    // 1. Ligação ao Utilizador

    public long UserId { get; set; }
    public virtual User User { get; set; } = null!;

    // 2. Ligação ao Livro
    public long BookId { get; set; }
    public virtual Book Book { get; set; } = null!;

    // 3. O Estado do livro

    public ReadingStatus Status { get; set; }
}