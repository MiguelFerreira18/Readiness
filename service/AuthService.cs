using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

public interface IAuthService
{
    Task<RegisteredUserResponse?> RegisterAsync(RegisterRequest request);
    Task<LoginResult?> LoginAsync(LoginRequest request);
    Task RevokeTokenAsync(string tokenId, DateTime expiresAtUtc);
    Task<ChangePasswordResult> ChangePasswordAsync(
        long userId,
        ChangePasswordRequest request);
    Task<UpdateUserProfileResult> UpdateProfileAsync(
        long userId,
        UpdateUserProfileRequest request);
}

public sealed class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ITokenRevocationRepository _tokenRevocationRepository;

    public AuthService(
        IAuthRepository repository,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        ITokenRevocationRepository tokenRevocationRepository)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _tokenRevocationRepository = tokenRevocationRepository;
    }

    public async Task<RegisteredUserResponse?> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _repository.EmailExistsAsync(normalizedEmail))
        {
            return null;
        }

        var user = new User
        {
            Email = normalizedEmail,
            Name = request.Name.Trim(),
            YearOfBirth = request.YearOfBirth,
            Role = Role.User
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        try
        {
            await _repository.AddUserAsync(user);
        }
        catch (DbUpdateException exception) when (IsDuplicateKey(exception))
        {
            return null;
        }

        return new RegisteredUserResponse(
            user.Id,
            user.Email,
            user.Name,
            user.YearOfBirth
        );
    }

    public async Task<LoginResult?> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _repository.GetUserByEmailAsync(normalizedEmail);
        if (user?.PasswordHash is null)
        {
            return null;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = _tokenService.CreateToken(user);
        return new LoginResult(
            token.Value,
            token.ExpiresAtUtc,
            new AuthenticatedUserResponse(user.Id, user.Email!, user.Name!)
        );
    }

    public Task RevokeTokenAsync(string tokenId, DateTime expiresAtUtc) =>
        _tokenRevocationRepository.RevokeAsync(tokenId, expiresAtUtc);

    public async Task<ChangePasswordResult> ChangePasswordAsync(
        long userId,
        ChangePasswordRequest request)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user?.PasswordHash is null)
        {
            return ChangePasswordResult.UserNotFound;
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.CurrentPassword);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return ChangePasswordResult.InvalidCurrentPassword;
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        await _repository.SaveChangesAsync();
        return ChangePasswordResult.Success;
    }

    public async Task<UpdateUserProfileResult> UpdateProfileAsync(
        long userId,
        UpdateUserProfileRequest request)
    {
        var user = await _repository.GetUserByIdAsync(userId);
        if (user is null)
        {
            return new UpdateUserProfileResult(UserProfileUpdateStatus.UserNotFound, null);
        }

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _repository.EmailExistsForOtherUserAsync(normalizedEmail, userId))
        {
            return new UpdateUserProfileResult(UserProfileUpdateStatus.EmailAlreadyInUse, null);
        }

        user.Email = normalizedEmail;
        user.Name = request.Name.Trim();
        user.YearOfBirth = request.YearOfBirth;

        try
        {
            await _repository.SaveChangesAsync();
        }
        catch (DbUpdateException exception) when (IsDuplicateKey(exception))
        {
            return new UpdateUserProfileResult(UserProfileUpdateStatus.EmailAlreadyInUse, null);
        }

        return new UpdateUserProfileResult(
            UserProfileUpdateStatus.Success,
            new UserProfileResponse(user.Id, user.Email!, user.Name!, user.YearOfBirth));
    }

    private static bool IsDuplicateKey(DbUpdateException exception) =>
        exception.InnerException is MySqlException { Number: 1062 };
}

public enum ChangePasswordResult
{
    Success,
    UserNotFound,
    InvalidCurrentPassword
}

public enum UserProfileUpdateStatus
{
    Success,
    UserNotFound,
    EmailAlreadyInUse
}

public sealed record UpdateUserProfileResult(
    UserProfileUpdateStatus Status,
    UserProfileResponse? Profile
);

public sealed record LoginResult(
    string Token,
    DateTime ExpiresAtUtc,
    AuthenticatedUserResponse User
);
