using System.ComponentModel.DataAnnotations;

public class LoginRequestValidationTests
{
    [Theory]
    [InlineData("not-an-email", "Password1")]
    [InlineData("user@example.com", "")]
    public void LoginRequest_InvalidData_FailsValidation(string email, string password)
    {
        var request = new LoginRequest(email, password);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.False(isValid);
    }
}
