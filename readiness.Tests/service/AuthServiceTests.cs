using Microsoft.AspNetCore.Identity;
using Moq;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _repository = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly Mock<ITokenRevocationRepository> _tokenRevocationRepository = new();
    private readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
    private readonly AuthService _service;

    public AuthServiceTests() =>
        _service = new AuthService(
            _repository.Object,
            _passwordHasher,
            _tokenService.Object,
            _tokenRevocationRepository.Object);

    [Fact]
    public async Task RegisterAsync_NewEmail_CreatesUserWithNormalizedEmailAndPasswordHash()
    {
        var role = new Role { Id = 1, Type = RoleType.User };
        _repository.Setup(repository => repository.EmailExistsAsync("alice@example.com"))
            .ReturnsAsync(false);
        _repository.Setup(repository => repository.GetRoleByTypeAsync(RoleType.User))
            .ReturnsAsync(role);
        _repository.Setup(repository => repository.AddUserAsync(It.IsAny<User>()))
            .Callback<User>(user => user.Id = 42)
            .Returns(Task.CompletedTask);

        var result = await _service.RegisterAsync(
            new RegisterRequest("Alice@Example.com", " Alice ", 1990, "Password1"));

        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Equal("Alice", result.Name);
        _repository.Verify(repository => repository.AddUserAsync(It.Is<User>(user =>
            user.RoleId == role.Id &&
            user.PasswordHash != "Password1" &&
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, "Password1")
                == PasswordVerificationResult.Success)), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ReturnsNullWithoutSaving()
    {
        _repository.Setup(repository => repository.EmailExistsAsync("alice@example.com"))
            .ReturnsAsync(true);

        var result = await _service.RegisterAsync(
            new RegisterRequest("Alice@Example.com", "Alice", 1990, "Password1"));

        Assert.Null(result);
        _repository.Verify(repository => repository.AddUserAsync(It.IsAny<User>()), Times.Never);
        _repository.Verify(repository => repository.GetRoleByTypeAsync(It.IsAny<RoleType>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsUserAndCreatesToken()
    {
        var user = AUserWithPassword("Password1");
        var expiry = DateTime.UtcNow.AddMinutes(15);
        _repository.Setup(repository => repository.GetUserByEmailAsync("alice@example.com"))
            .ReturnsAsync(user);
        _tokenService.Setup(service => service.CreateToken(user))
            .Returns(("jwt-token", expiry));

        var result = await _service.LoginAsync(new LoginRequest("Alice@Example.com", "Password1"));

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal(expiry, result.ExpiresAtUtc);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.Email, result.User.Email);
        _tokenService.Verify(service => service.CreateToken(user), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsNullWithoutCreatingToken()
    {
        _repository.Setup(repository => repository.GetUserByEmailAsync("unknown@example.com"))
            .ReturnsAsync((User?)null);

        var result = await _service.LoginAsync(new LoginRequest("unknown@example.com", "Password1"));

        Assert.Null(result);
        _tokenService.Verify(service => service.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsNullWithoutCreatingToken()
    {
        var user = AUserWithPassword("CorrectPassword1");
        _repository.Setup(repository => repository.GetUserByEmailAsync("alice@example.com"))
            .ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequest("alice@example.com", "WrongPassword1"));

        Assert.Null(result);
        _tokenService.Verify(service => service.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task RevokeTokenAsync_DelegatesTokenIdAndExpirationToRepository()
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(15);

        await _service.RevokeTokenAsync("token-id", expiresAtUtc);

        _tokenRevocationRepository.Verify(
            repository => repository.RevokeAsync("token-id", expiresAtUtc),
            Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidCurrentPassword_SavesNewHash()
    {
        var user = AUserWithPassword("CurrentPassword1");
        _repository.Setup(repository => repository.GetUserByIdAsync(user.Id))
            .ReturnsAsync(user);

        var result = await _service.ChangePasswordAsync(
            user.Id,
            new ChangePasswordRequest("CurrentPassword1", "NewPassword2"));

        Assert.Equal(ChangePasswordResult.Success, result);
        Assert.NotEqual("NewPassword2", user.PasswordHash);
        Assert.Equal(
            PasswordVerificationResult.Success,
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, "NewPassword2"));
        Assert.Equal(
            PasswordVerificationResult.Failed,
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, "CurrentPassword1"));
        _repository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidCurrentPassword_DoesNotSave()
    {
        var user = AUserWithPassword("CurrentPassword1");
        var originalHash = user.PasswordHash;
        _repository.Setup(repository => repository.GetUserByIdAsync(user.Id))
            .ReturnsAsync(user);

        var result = await _service.ChangePasswordAsync(
            user.Id,
            new ChangePasswordRequest("WrongPassword1", "NewPassword2"));

        Assert.Equal(ChangePasswordResult.InvalidCurrentPassword, result);
        Assert.Equal(originalHash, user.PasswordHash);
        _repository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task ChangePasswordAsync_UserNotFound_DoesNotSave()
    {
        _repository.Setup(repository => repository.GetUserByIdAsync(42))
            .ReturnsAsync((User?)null);

        var result = await _service.ChangePasswordAsync(
            42,
            new ChangePasswordRequest("CurrentPassword1", "NewPassword2"));

        Assert.Equal(ChangePasswordResult.UserNotFound, result);
        _repository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateProfileAsync_ValidRequest_UpdatesOnlyProfileFields()
    {
        var user = AUserWithPassword("CurrentPassword1");
        var originalId = user.Id;
        var originalRoleId = user.RoleId;
        var originalPasswordHash = user.PasswordHash;
        _repository.Setup(repository => repository.GetUserByIdAsync(user.Id))
            .ReturnsAsync(user);
        _repository.Setup(repository =>
                repository.EmailExistsForOtherUserAsync("new@example.com", user.Id))
            .ReturnsAsync(false);

        var result = await _service.UpdateProfileAsync(
            user.Id,
            new UpdateUserProfileRequest(" New@Example.com ", " New name ", 1995));

        Assert.Equal(UserProfileUpdateStatus.Success, result.Status);
        Assert.Equal("new@example.com", result.Profile!.Email);
        Assert.Equal("New name", result.Profile.Name);
        Assert.Equal(1995, result.Profile.YearOfBirth);
        Assert.Equal(originalId, result.Profile.Id);
        Assert.Equal(originalRoleId, user.RoleId);
        Assert.Equal(originalPasswordHash, user.PasswordHash);
        _repository.Verify(repository => repository.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProfileAsync_EmailAlreadyInUse_DoesNotSave()
    {
        var user = AUserWithPassword("CurrentPassword1");
        var originalEmail = user.Email;
        _repository.Setup(repository => repository.GetUserByIdAsync(user.Id))
            .ReturnsAsync(user);
        _repository.Setup(repository =>
                repository.EmailExistsForOtherUserAsync("taken@example.com", user.Id))
            .ReturnsAsync(true);

        var result = await _service.UpdateProfileAsync(
            user.Id,
            new UpdateUserProfileRequest("taken@example.com", "New name", 1995));

        Assert.Equal(UserProfileUpdateStatus.EmailAlreadyInUse, result.Status);
        Assert.Equal(originalEmail, user.Email);
        _repository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateProfileAsync_UserNotFound_DoesNotSave()
    {
        _repository.Setup(repository => repository.GetUserByIdAsync(404))
            .ReturnsAsync((User?)null);

        var result = await _service.UpdateProfileAsync(
            404,
            new UpdateUserProfileRequest("new@example.com", "New name", 1995));

        Assert.Equal(UserProfileUpdateStatus.UserNotFound, result.Status);
        _repository.Verify(repository => repository.SaveChangesAsync(), Times.Never);
    }

    private User AUserWithPassword(string password)
    {
        var user = new User
        {
            Id = 42,
            Email = "alice@example.com",
            Name = "Alice",
            RoleId = 1,
            Role = new Role { Id = 1, Type = RoleType.User }
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        return user;
    }
}
