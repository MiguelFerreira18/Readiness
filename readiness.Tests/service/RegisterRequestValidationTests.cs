using System.ComponentModel.DataAnnotations;

public class RegisterRequestValidationTests
{
    [Theory]
    [InlineData("short")]
    [InlineData("alllowercase1")]
    [InlineData("ALLUPPERCASE1")]
    [InlineData("NoNumbers")]
    public void RegisterRequest_InvalidPassword_FailsValidation(string password)
    {
        var request = new RegisterRequest("user@example.com", "User", 1990, password);
        AssertInvalid(request);
    }

    [Theory]
    [InlineData("not-an-email", "User", 1990)]
    [InlineData("user@example.com", "   ", 1990)]
    [InlineData("user@example.com", "User", 2028)]
    public void RegisterRequest_InvalidProfileData_FailsValidation(
        string email,
        string name,
        int yearOfBirth)
    {
        AssertInvalid(new RegisterRequest(email, name, yearOfBirth, "Password1"));
    }

    [Fact]
    public void RegisterRequest_ValidData_PassesValidation()
    {
        var request = new RegisterRequest("user@example.com", "User", 1990, "Password1");
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.True(isValid);
    }

    private static void AssertInvalid(RegisterRequest request)
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
