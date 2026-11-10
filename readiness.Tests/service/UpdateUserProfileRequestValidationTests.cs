using System.ComponentModel.DataAnnotations;

public class UpdateUserProfileRequestValidationTests
{
    [Theory]
    [InlineData("", "Reader", 1990)]
    [InlineData("invalid-email", "Reader", 1990)]
    [InlineData("reader@example.com", "   ", 1990)]
    [InlineData("reader@example.com", "Reader", -1)]
    [InlineData("reader@example.com", "Reader", 2028)]
    public void UpdateUserProfileRequest_InvalidData_FailsValidation(
        string email,
        string name,
        int yearOfBirth)
    {
        var request = new UpdateUserProfileRequest(email, name, yearOfBirth);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.False(isValid);
    }

    [Fact]
    public void UpdateUserProfileRequest_ValidData_PassesValidation()
    {
        var request = new UpdateUserProfileRequest("reader@example.com", "Reader", 1990);
        var results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            results,
            validateAllProperties: true);

        Assert.True(isValid);
    }
}
