using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Bookings;

public record DateRange
{
    private DateRange()
    {
    }

    public DateOnly Start { get; init; }
    public DateOnly End { get; init; }

    public int LengthInDays => End.DayNumber - Start.DayNumber;




    public static Result<DateRange> Create(DateOnly start, DateOnly end)
    {
        if (start > end)
        {
            // التعديل: تم تغيير نوع الإرجاع إلى Result<DateRange> بدلاً من DateRange مباشرة.
            // السبب: رمي الاستثناءات (Exceptions) في .NET مكلف جداً من حيث الأداء ويجب تجنبه في منطق التحقق العادي (Domain Validation).
            // النمط الجديد يرجع كائن Result يعبر عن نجاح العملية أو فشلها مع سبب الفشل بشكل منظم.
            return Result.Failure<DateRange>(new(
        "DateRange.Invalid",
        "End date precedes start date"));
        }

        return new DateRange
        {
            Start = start,
            End = end
        };
    }
}

