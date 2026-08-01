namespace Bookify.Application.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTime DateTimeNow { get; }
}
