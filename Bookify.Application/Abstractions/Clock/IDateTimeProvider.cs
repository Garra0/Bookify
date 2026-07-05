namespace Bookify.Application.Abstractions.Clock;

public interface IDateTimeProvider
{
    DateTime DateTime { get; }
}
