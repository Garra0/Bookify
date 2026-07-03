using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments; 
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;

public sealed class Booking : Entity
{
    // الغلط: private Booking(Guid id, Guid apartmentId, Guid userId, DateRange duration, Money priceForPeriod, Money cleaingFee, Money amenitiesUpCharge, Money totalPrice, BookingStatus status, DateTime createdDate) : base(id)
    private Booking(Guid id, Guid apartmentId, Guid userId, DateRange duration, Money priceForPeriod, Money cleaningFee, Money amenitiesUpCharge, Money totalPrice, BookingStatus status, DateTime createdDate) : base(id) // الصح (تعديل الخطأ الإملائي)
    {
        ApartmentId = apartmentId;
        UserId = userId;
        Duration = duration;
        PriceForPeriod = priceForPeriod;
        // الغلط: CleaingFee = cleaingFee;
        CleaningFee = cleaningFee; // الصح (تعديل الخطأ الإملائي)
        AmenitiesUpCharge = amenitiesUpCharge;
        TotalPrice = totalPrice;
        Status = status;
        CreatedDate = createdDate;
    }

    // الغلط: public Guid ApartmentId { get; set; }
    public Guid ApartmentId { get; private set; } // الصح

    // الغلط: public Guid UserId { get; set; }
    public Guid UserId { get; private set; } // الصح

    // الغلط: public DateRange Duration { get; set; }
    public DateRange Duration { get; private set; } // الصح

    // الغلط: public Money PriceForPeriod { get; set; }
    public Money PriceForPeriod { get; private set; } // الصح

    // الغلط: public Money CleaingFee { get; set; }
    public Money CleaningFee { get; private set; } // الصح (تعديل الخطأ الإملائي)

    // الغلط: public Money AmenitiesUpCharge { get; set; }
    public Money AmenitiesUpCharge { get; private set; } // الصح

    // الغلط: public Money TotalPrice { get; set; }
    public Money TotalPrice { get; private set; } // الصح

    // الغلط: public BookingStatus Status { get; set; }
    public BookingStatus Status { get; private set; } // الصح

    // الغلط: public DateTime CreatedDate { get; set; }
    public DateTime CreatedDate { get; private set; } // الصح

    public DateTime? ConfirmedDate { get; private set; } // تعديل الـ setter إلى private
    public DateTime? RejectedDate { get; private set; } // تعديل الـ setter إلى private
    public DateTime? CompletedDate { get; private set; } // تعديل الـ setter إلى private
    public DateTime? CancelledDate { get; private set; } // تعديل الـ setter إلى private

    public static Booking Reserve(Apartment apartment, Guid userId, DateRange duration, DateTime createdDate, PricingService pricingService)
    {
        var pricingDetails = pricingService.CalculatePrice(apartment, duration);

        var booking = new Booking(
            Guid.NewGuid(),
            apartment.Id,
            userId,
            duration,
            pricingDetails.PriceForPeriod,
            // الغلط: pricingDetails.CleaingFee,
            pricingDetails.CleaningFee, // الصح (تعديل الخطأ الإملائي)
            // الغلط: pricingDetails.AmenitiesUpChare,
            pricingDetails.AmenitiesUpCharge, // الصح (تعديل الخطأ الإملائي)
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
        // الغلط: ConfirmedDate = dateTime;
        CompletedDate = dateTime; // الصح

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
        // الغلط: ConfirmedDate = dateTime;
        CancelledDate = dateTime; // الصح

        RaiseDomainEvent(new BookingCancelledDomainEvent(Id));

        return Result.Success();
    }
}
