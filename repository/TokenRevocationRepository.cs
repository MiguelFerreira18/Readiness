using Microsoft.EntityFrameworkCore;
using MySqlConnector;

public interface ITokenRevocationRepository
{
    Task RevokeAsync(string tokenId, DateTime expiresAtUtc);
    Task<bool> IsRevokedAsync(string tokenId);
}

public sealed class TokenRevocationRepository : ITokenRevocationRepository
{
    private readonly AppDbContext _context;

    public TokenRevocationRepository(AppDbContext context) => _context = context;

    public async Task RevokeAsync(string tokenId, DateTime expiresAtUtc)
    {
        var now = DateTime.UtcNow;
        await _context.RevokedTokens
            .Where(token => token.ExpiresAtUtc <= now)
            .ExecuteDeleteAsync();

        _context.RevokedTokens.Add(new RevokedToken
        {
            TokenId = tokenId,
            ExpiresAtUtc = expiresAtUtc
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (IsDuplicateKey(exception))
        {
            _context.Entry(_context.RevokedTokens.Local
                .Single(token => token.TokenId == tokenId)).State = EntityState.Detached;
        }
    }

    public Task<bool> IsRevokedAsync(string tokenId) =>
        _context.RevokedTokens.AnyAsync(token =>
            token.TokenId == tokenId && token.ExpiresAtUtc > DateTime.UtcNow);

    private static bool IsDuplicateKey(DbUpdateException exception) =>
        exception.InnerException is MySqlException { Number: 1062 };
}
