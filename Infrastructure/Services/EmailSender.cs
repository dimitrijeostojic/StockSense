using Domain.Dtos;
using Infrastructure.InternalServiceInterfaces;
using Infrastructure.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Infrastructure.Services;

internal sealed class EmailSender(IOptions<SmtpOptions> options) : INotificationSender<EmailMessageDto>
{
    private readonly SmtpOptions _options = options.Value;

    public async Task SendAsync(EmailMessageDto messageDto, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageDto.To);
        ArgumentException.ThrowIfNullOrWhiteSpace(messageDto.Subject);
        try
        {
            var message = new MimeMessage();
            var from = new MailboxAddress(_options.SenderName, _options.SenderEmail);
            message.From.Add(from);
            var to = new MailboxAddress(null, messageDto.To);
            message.To.Add(to);
            message.Subject = messageDto.Subject;
            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = messageDto.Body ?? string.Empty
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_options.Server, _options.Port, cancellationToken: cancellationToken);
            await smtp.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
            await smtp.SendAsync(message, cancellationToken);
            await smtp.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.Message, ex);
        }

    }
}
