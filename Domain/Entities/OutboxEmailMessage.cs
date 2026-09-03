using Domain.Primitives;

namespace Domain.Entities;

public sealed class OutboxEmailMessage : Entity
{
    public string? To { get; private set; }
    public string? Subject { get; private set; }
    public string? Body { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? SentAtUtc { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }

    public static OutboxEmailMessage Create(string? to, string? subject, string? body)
    {
        ArgumentNullException.ThrowIfNull(to);
        ArgumentNullException.ThrowIfNull(subject);
        return new OutboxEmailMessage
        {
            To = to,
            Subject = subject,
            Body = body,
            CreatedAtUtc = DateTime.UtcNow,
            Attempts = 0
        };
    }

    public OutboxEmailMessage WithFailedAttempt(string message)
    {
        Attempts++;
        LastError = message;
        return this;
    }

    public OutboxEmailMessage WithSentAtUtc(DateTime time)
    {
        SentAtUtc = time;
        return this;
    }
}
