using Domain.Dtos;

namespace Application.Emails;

internal static class EmailTemplates
{
    public static EmailMessageDto Welcome(string to, string firstName, string companyName)
    {
        var body = $"""
            Hi {firstName},

            Your StockSense account for {companyName} has been created successfully.

            — StockSense
            """;

        return new EmailMessageDto(to, "Welcome to StockSense", body);
    }

    public static EmailMessageDto UserInvited(string to, string firstName, string companyName)
    {
        var subject = $"You've been added to {companyName} on StockSense";
        var body = $"""
            Hi {firstName},

            You've been added as a team member for {companyName} on StockSense.
            You can log in using the credentials provided to you.

            — StockSense
            """;

        return new EmailMessageDto(to, subject, body);
    }

    public static EmailMessageDto PasswordReset(string to, string firstName, string resetLink)
    {
        var subject = "Reset your StockSense password";
        var body = $"""
            Hi {firstName},

            We received a request to reset your password. Click the link below to choose a new one:

            {resetLink}

            This link expires in 30 minutes. If you didn't request this, you can safely ignore this email.

            — StockSense
            """;

        return new EmailMessageDto(to, subject, body);
    }
}