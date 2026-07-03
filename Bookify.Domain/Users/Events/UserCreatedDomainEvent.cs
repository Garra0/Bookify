using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Users.Events;

// الغلط: public sealed record UserCreatedDominEvent(Guid UserId) : IDomainEvent;
public sealed record UserCreatedDomainEvent(Guid UserId) : IDomainEvent; // الصح