using System.ComponentModel.DataAnnotations;

public class ChangePasswordRequestValidationTests
{
    [Theory]
    [InlineData("short")]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoNumbers")]
    public void ChangePasswordRequest_InvalidNewPassword_FailsValidation(string newPassword)
    {
        AssertInvalid(new ChangePasswordRequest("CurrentPassword1", newPassword));
    }

    [Theory]
    [InlineData("", "NewPassword2")]
    [InlineData("CurrentPassword1", "")]
    public void ChangePasswordRequest_EmptyPassword_FailsValidation(
        string currentPassword,
        string newPassword)
    {
        AssertInvalid(new ChangePasswordRequest(currentPassword, newPassword));
    }

    [Fact]
    public void ChangePasswordRequest_ValidData_PassesValidation()
    {
        var request = new ChangePasswordRequest("CurrentPassword1", "NewPassword2");
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.True(isValid);
    }

    private static void AssertInvalid(ChangePasswordRequest request)
    {
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.False(isValid);
    }
}
