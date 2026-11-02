using System.ComponentModel.DataAnnotations;

public sealed record RegisterRequest(
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: Required, StringLength(64), RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty.")]
    string Name,
    [property: Range(0, 2027)] int YearOfBirth,
    [property: Required, StringLength(64, MinimumLength = 8)]
    [property: RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,64}$",
        ErrorMessage = "Password must contain an uppercase letter, a lowercase letter, and a number.")]
    string Password
);

public sealed record RegisteredUserResponse(
    long Id,
    string Email,
    string Name,
    int YearOfBirth
);

public sealed record LoginRequest(
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: Required, StringLength(64, MinimumLength = 1)] string Password
);

public sealed record AuthenticatedUserResponse(
    long Id,
    string Email,
    string Name
);

public sealed record ChangePasswordRequest(
    [property: Required, StringLength(256, MinimumLength = 1)] string CurrentPassword,
    [property: Required, StringLength(64, MinimumLength = 8)]
    [property: RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,64}$",
        ErrorMessage = "Password must contain an uppercase letter, a lowercase letter, and a number.")]
    string NewPassword
);

public sealed record UpdateUserProfileRequest(
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: Required, StringLength(64), RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty.")]
    string Name,
    [property: Range(0, 2027)] int YearOfBirth
);

public sealed record UserProfileResponse(
    long Id,
    string Email,
    string Name,
    int YearOfBirth
);
