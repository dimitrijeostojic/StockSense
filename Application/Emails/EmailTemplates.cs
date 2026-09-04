using Domain.Dtos;

namespace Application.Emails;

internal static class EmailTemplates
{
    public static EmailMessageDto Welcome(string to, string firstName, string companyName)
    {
        var subject = $"Welcome to StockSense, {firstName}!";
        var body = $"""
            Hi {firstName},

            Welcome to StockSense! Your account for {companyName} has been created successfully, and you're all set to start managing your inventory.

            Here's a quick overview to help you get started:

              1. Add your suppliers — the companies you order stock from
              2. Create your product categories
              3. Add your products, linking each one to a category and supplier
              4. Place your first order, and track it all the way to delivery

            Once your first order arrives and its status is marked as "Received," StockSense will automatically update your stock levels for you — no manual counting needed.

            If you ever run low on inventory, we'll help you spot it early so you can reorder in time.

            If you have any questions along the way, just reply to this email — we're happy to help.

            Welcome aboard!

            — The StockSense Team
            """;

        return new EmailMessageDto(to, subject, body);
    }

    public static EmailMessageDto PasswordReset(string to, string firstName, string resetLink)
    {
        var subject = "Reset your StockSense password";
        var body = $"""
            Hi {firstName},

            We received a request to reset the password for your StockSense account. If this was you, click the link below to choose a new password:

            {resetLink}

            This link will expire in 30 minutes for security reasons. If it expires before you get a chance to use it, simply request a new one from the login page.

            If you didn't request a password reset, you can safely ignore this email — your password will remain unchanged, and no further action is needed.

            For your security, we recommend choosing a password you haven't used before.

            — The StockSense Team
            """;

        return new EmailMessageDto(to, subject, body);
    }

    public static EmailMessageDto UserInvited(string to, string firstName, string companyName)
    {
        var subject = $"You've been added to {companyName} on StockSense";
        var body = $"""
            Hi {firstName},

            You've been added as a team member for {companyName} on StockSense — a tool that helps your team keep track of inventory, suppliers, and orders in one place.

            You can log in using the email address this invitation was sent to, along with the password provided to you by your team admin.

            Once you're in, you'll be able to:

              - View and manage products, categories, and suppliers
              - Create and track purchase orders
              - Keep an eye on stock levels across the business

            If you weren't expecting this invitation or believe you received it by mistake, please reach out to your team admin at {companyName}.

            Welcome to the team!

            — The StockSense Team
            """;

        return new EmailMessageDto(to, subject, body);
    }
}