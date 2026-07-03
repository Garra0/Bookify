using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public record PricingDetails(
    Money PriceForPeriod,
    Money CleaingFee,
    Money AmenitiesUpChare,
    Money TotalPrice);