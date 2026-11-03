using Microsoft.EntityFrameworkCore;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail);
    Task AddUserAsync(User user);
    Task<User?> GetUserByEmailAsync(string normalizedEmail);
    Task<User?> GetUserByIdAsync(long userId);
    Task<bool> EmailExistsForOtherUserAsync(string normalizedEmail, long userId);
    Task SaveChangesAsync();
}

public sealed class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context) => _context = context;

    public Task<bool> EmailExistsAsync(string normalizedEmail) =>
        _context.Users.AnyAsync(user => user.Email == normalizedEmail);

    public Task<User?> GetUserByEmailAsync(string normalizedEmail) =>
        _context.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail);

    public Task<User?> GetUserByIdAsync(long userId) =>
        _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

    public Task<bool> EmailExistsForOtherUserAsync(string normalizedEmail, long userId) =>
        _context.Users.AnyAsync(user => user.Email == normalizedEmail && user.Id != userId);

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}
