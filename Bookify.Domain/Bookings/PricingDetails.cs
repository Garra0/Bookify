using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public record PricingDetails(
    Money PriceForPeriod,
    // الغلط: Money CleaingFee,
    Money CleaningFee, // الصح
    // الغلط: Money AmenitiesUpChare,
    Money AmenitiesUpCharge, // الصح
    Money TotalPrice);