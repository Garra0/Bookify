namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOn, string type, string content)
    {
        Id = id;
        OccurredOn = occurredOn;
        Content = content;
        Type = type;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOn { get; init; }

    public string Type { get; init; }

    public string Content { get; init; }

    public DateTime? ProcessedOn { get; init; }

    public string? Error { get; init; }
}
