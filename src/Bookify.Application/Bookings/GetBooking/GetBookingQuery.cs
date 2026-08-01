using Bookify.Application.Abstractions.Caching; 

namespace Bookify.Application.Bookings.GetBooking;

public sealed record GetBookingQuery(Guid BookingId) : ICachedQuery<BookingResponse> // : IQuery<BookingResponse>;
{
    public string CacheKey => $"booking-{BookingId}";

    public TimeSpan? Expiration => null;
}