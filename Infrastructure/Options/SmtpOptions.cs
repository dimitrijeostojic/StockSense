namespace Infrastructure.Options;

public sealed class SmtpOptions
{
    public string Server { get; init; } = string.Empty;
    public int Port { get; init; }
    public string SenderName { get; init; } = string.Empty;
    public string SenderEmail { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
