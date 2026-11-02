using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Moq;
using readiness.Controllers;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_Success_SetsSecureHttpOnlyJwtCookieAndReturnsPublicProfile()
    {
        var service = new Mock<IAuthService>();
        service.Setup(auth => auth.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync(new LoginResult(
                "signed-jwt",
                DateTime.UtcNow.AddMinutes(15),
                new AuthenticatedUserResponse(42, "reader@example.com", "Reader")));
        var controller = CreateController(service.Object);

        var action = await controller.Login(new LoginRequest("reader@example.com", "Password1"));
        var cookie = controller.Response.Headers.SetCookie.ToString();

        Assert.IsType<OkObjectResult>(action.Result);
        Assert.Contains($"{JwtSettings.CookieName}=signed-jwt", cookie);
        Assert.Contains("httponly", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.IsType<AuthenticatedUserResponse>(((OkObjectResult)action.Result!).Value);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorizedWithoutCookie()
    {
        var service = new Mock<IAuthService>();
        service.Setup(auth => auth.LoginAsync(It.IsAny<LoginRequest>()))
            .ReturnsAsync((LoginResult?)null);
        var controller = CreateController(service.Object);

        var action = await controller.Login(new LoginRequest("reader@example.com", "WrongPassword1"));

        Assert.IsType<UnauthorizedObjectResult>(action.Result);
        Assert.False(controller.Response.Headers.ContainsKey("Set-Cookie"));
    }

    [Fact]
    public async Task Logout_ValidToken_RevokesTokenAndDeletesCookie()
    {
        var service = new Mock<IAuthService>();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
        var controller = CreateController(service.Object);
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Jti, "valid-jti"),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToString())
            ],
            "Bearer"));

        var result = await controller.Logout();

        Assert.IsType<NoContentResult>(result);
        service.Verify(
            auth => auth.RevokeTokenAsync(
                "valid-jti",
                DateTimeOffset.FromUnixTimeSeconds(expiresAt).UtcDateTime),
            Times.Once);
        var cookie = controller.Response.Headers.SetCookie.ToString();
        Assert.Contains($"{JwtSettings.CookieName}=", cookie);
        Assert.Contains("expires=", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("secure", cookie, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("samesite=strict", cookie, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Logout_WithoutTokenClaims_ReturnsUnauthorizedWithoutRevoking()
    {
        var service = new Mock<IAuthService>();
        var controller = CreateController(service.Object);

        var result = await controller.Logout();

        Assert.IsType<UnauthorizedObjectResult>(result);
        service.Verify(
            auth => auth.RevokeTokenAsync(It.IsAny<string>(), It.IsAny<DateTime>()),
            Times.Never);
        Assert.False(controller.Response.Headers.ContainsKey("Set-Cookie"));
    }

    [Fact]
    public async Task ChangePassword_AuthenticatedUser_UsesSubjectClaim()
    {
        var service = new Mock<IAuthService>();
        service.Setup(auth => auth.ChangePasswordAsync(
                42,
                It.IsAny<ChangePasswordRequest>()))
            .ReturnsAsync(ChangePasswordResult.Success);
        var controller = CreateController(service.Object);
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "42")
            ],
            "Bearer"));

        var result = await controller.ChangePassword(
            new ChangePasswordRequest("CurrentPassword1", "NewPassword2"));

        Assert.IsType<NoContentResult>(result);
        service.Verify(auth => auth.ChangePasswordAsync(
            42,
            It.Is<ChangePasswordRequest>(request =>
                request.CurrentPassword == "CurrentPassword1" &&
                request.NewPassword == "NewPassword2")), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_MissingSubjectClaim_ReturnsUnauthorized()
    {
        var service = new Mock<IAuthService>();
        var controller = CreateController(service.Object);

        var result = await controller.ChangePassword(
            new ChangePasswordRequest("CurrentPassword1", "NewPassword2"));

        Assert.IsType<UnauthorizedObjectResult>(result);
        service.Verify(auth => auth.ChangePasswordAsync(
            It.IsAny<long>(),
            It.IsAny<ChangePasswordRequest>()), Times.Never);
    }

    [Fact]
    public async Task UpdateMyProfile_AuthenticatedUser_UsesSubjectClaim()
    {
        var service = new Mock<IAuthService>();
        var response = new UserProfileResponse(42, "reader@example.com", "Reader Updated", 1992);
        service.Setup(auth => auth.UpdateProfileAsync(
                42,
                It.IsAny<UpdateUserProfileRequest>()))
            .ReturnsAsync(new UpdateUserProfileResult(UserProfileUpdateStatus.Success, response));
        var controller = CreateUsersController(service.Object);
        controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, "42")
            ],
            "Bearer"));

        var result = await controller.UpdateMyProfile(
            new UpdateUserProfileRequest("reader@example.com", "Reader Updated", 1992));

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
        service.Verify(auth => auth.UpdateProfileAsync(
            42,
            It.Is<UpdateUserProfileRequest>(request =>
                request.Email == "reader@example.com" &&
                request.Name == "Reader Updated" &&
                request.YearOfBirth == 1992)), Times.Once);
    }

    [Fact]
    public async Task UpdateMyProfile_MissingSubjectClaim_ReturnsUnauthorized()
    {
        var service = new Mock<IAuthService>();
        var controller = CreateUsersController(service.Object);

        var result = await controller.UpdateMyProfile(
            new UpdateUserProfileRequest("reader@example.com", "Reader", 1992));

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
        service.Verify(auth => auth.UpdateProfileAsync(
            It.IsAny<long>(),
            It.IsAny<UpdateUserProfileRequest>()), Times.Never);
    }

    private static UsersController CreateUsersController(IAuthService service) =>
        new(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

    private static AuthController CreateController(IAuthService service) =>
        new(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
}
