namespace Application.AuthManagement.Login;

public sealed record LoginResponse(
    string? AccessToken,
    string? RefreshToken,
    bool RequiresTwoFactor = false,
    string? UserId = null);
