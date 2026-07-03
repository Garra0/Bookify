using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users.Events;

public sealed record UserCreatedDominEvent(Guid UserId) : IDomainEvent;