namespace Application.AuthManagement.Register;

public sealed record RegisterResponse(
    string AccessToken,
    string? RefreshToken);
