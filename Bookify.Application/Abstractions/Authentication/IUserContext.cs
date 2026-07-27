namespace Bookify.Application.Abstractions.Authentication;

public interface IUserContext
{
    // ضفناها مشان نعرف هل اليوزر بعد ما عمل اوثرايزيشن راح ينادي خدمه تابعه له ولا لشخص اخر 
    // هو هيك ممكن يرسل  userId لشخص اخر ويشوف الحجز الخاص فيه مثلا فلازم نمنعه
    // وراح نمنعه عن طريق Resource-based Authorization
    // طريقة التحقق اني اعمل if state
    Guid UserId { get; }

    string IdentityId { get; }
}