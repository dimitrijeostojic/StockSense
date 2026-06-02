using Domain.Core;

namespace Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error NotFound = new("ApplicationErrors.NotFound", "The requested resource was not found.");
    public static readonly Error InvalidCredentials = new("ApplicationErrors.InvalidCredentials", "Invalid email or password.");
    public static readonly Error EmailAlreadyExists = new("ApplicationErrors.EmailAlreadyExists", "A user with this email already exists.");
    public static readonly Error RegistrationFailed = new("ApplicationErrors.RegistrationFailed", "User registration failed.");
    public static readonly Error InvalidRefreshToken = new("ApplicationErrors.InvalidRefreshToken", "Refresh token is invalid or has expired.");
    public static readonly Error InvalidTwoFactorCode = new("ApplicationErrors.InvalidTwoFactorCode", "The two-factor authentication code is invalid.");
    public static readonly Error TwoFactorNotEnabled = new("ApplicationErrors.TwoFactorNotEnabled", "Two-factor authentication is not enabled for this user.");
    public static readonly Error UnauthorizedRole = new("ApplicationErrors.UnauthorizedRole", "Access is restricted to administrators and managers only.");
    public static readonly Error DeleteFailure = new("ApplicationErrors.DeleteFailure", "Failed to delete user.");
}
