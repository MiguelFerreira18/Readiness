using System.ComponentModel.DataAnnotations;

public sealed record RegisterRequest(
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, StringLength(64), RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty.")]
    string Name,
    [Range(0, 2027)] int YearOfBirth,
    [Required, StringLength(64, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,64}$",
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
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, StringLength(64, MinimumLength = 1)] string Password
);

public sealed record LoginUserResponse(
    long Id,
    string Email,
    string Name
);

public sealed record ChangePasswordRequest(
    [Required, StringLength(256, MinimumLength = 1)] string CurrentPassword,
    [Required, StringLength(64, MinimumLength = 8)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,64}$",
        ErrorMessage = "Password must contain an uppercase letter, a lowercase letter, and a number.")]
    string NewPassword
);

public sealed record UpdateUserProfileRequest(
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, StringLength(64), RegularExpression(@".*\S.*", ErrorMessage = "Name cannot be empty.")]
    string Name,
    [Range(0, 2027)] int YearOfBirth
);

public sealed record LoginUserProfileResponse(
    long Id,
    string Email,
    string Name,
    int YearOfBirth
);

public sealed record UpdateUserProfileResult(
    UserProfileUpdateStatus Status,
    LoginUserProfileResponse? Profile
);

public sealed record LoginResult(
    string Token,
    DateTime ExpiresAtUtc,
    LoginUserResponse User
);
