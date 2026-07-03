using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments; 
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public sealed class Booking : Entity
{
    private Booking(Guid id, Guid apartmentId, Guid userId, DateRange duration, Money priceForPeriod, Money cleaingFee, Money amenitiesUpCharge, Money totalPrice, BookingStatus status, DateTime createdDate) : base(id)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        CleaingFee = cleaingFee;
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedDate = createdDate;
    }

    public Guid ApartmentId { get; set; }
    public Guid UserId { get; set; }
    public DateRange Duration { get; set; }
    public Money PriceForPeriod { get; set; }
    public Money CleaingFee { get; set; }
    public Money AmenitiesUpCharge { get; set; }
    public Money TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ConfirmedDate { get; set; }
    public DateTime? RejectedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CancelledDate { get; set; }

    public static Booking Reserve(Apartment apartment, Guid userId, DateRange duration, DateTime createdDate, PricingService pricingService)
    {
        var pricingDetails = pricingService.CalculatePrice(apartment, duration);

        var booking = new Booking(
            Guid.NewGuid(),
            apartment.Id,
            userId,
            duration,
            pricingDetails.PriceForPeriod,
            pricingDetails.CleaingFee,
            pricingDetails.AmenitiesUpChare,
            pricingDetails.TotalPrice,
            BookingStatus.Reserved,
            createdDate
            );

        booking.RaiseDomainEvent(new BookingReservedDomainEvent(booking.Id));

        apartment.LastBooked = createdDate;

        return booking;
    }

    public Result Confirm(DateTime dateTime)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.Confirmed;
        ConfirmedDate = dateTime;

        RaiseDomainEvent(new BookingConfirmedDomainEvent(Id));

        return Result.Success();
    }

    public Result Reject(DateTime dateTime)
    {
        if (Status != BookingStatus.Reserved)
            return Result.Failure(BookingErrors.NotReserved);

        Status = BookingStatus.Rejected;
        RejectedDate = dateTime;

        RaiseDomainEvent(new BookingRejectedDomainEvent(Id));

        return Result.Success();
    }

    public Result Complete(DateTime dateTime)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(BookingErrors.NotConfirmed);

        Status = BookingStatus.Completed;
        ConfirmedDate = dateTime;

        RaiseDomainEvent(new BookingCompletedDomainEvent(Id));

        return Result.Success();
    }

    public Result Cancel(DateTime dateTime)
    {
        if (Status != BookingStatus.Confirmed)
            return Result.Failure(BookingErrors.NotConfirmed);

        var currentDate = DateOnly.FromDateTime(dateTime);

        // لا يمكن الغاء حجز قد بدأ بالفعل
        if (currentDate > Duration.Start)
            return Result.Failure(BookingErrors.AlreadyStarted);

        Status = BookingStatus.Cancelled;
        ConfirmedDate = dateTime;

        RaiseDomainEvent(new BookingCancelledDomainEvent(Id));

        return Result.Success();
    }
}
