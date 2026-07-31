namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxOptions
{
    // كل كم ثانية عشان يشتغل الجوب
    public int IntervalInSeconds { get; init; }

    // كم رسالة في كل مرة يشتغل فيها الجوب
    public int BatchSize { get; init; }
}
