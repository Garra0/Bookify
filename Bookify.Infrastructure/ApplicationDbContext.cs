using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Bookify.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    //private readonly IPublisher _publisher;
    private readonly IDateTimeProvider _dateTimeProvider;


    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages();

            var result = await base.SaveChangesAsync(cancellationToken);

            // مشكلة الدومين ايفنت ممكن تفشل لذلك راح نستخدم ال Outbox pattern بحيث نخزن الدومين ايفنت في جدول خاص ونستخدم خدمة خارجية لنشرها لاحقاً. هذا يقلل من احتمالية فشل نشر الدومين ايفنت ويضمن استمرارية النظام.
            // فلو خدمة الارسال فشلت فراح يفشل الكود مع اني حفظت الداتا بالداتا بيز لذلك 
            // راح نصير محفظ الرسالة بالدي بي كمان بحيث جوب يعمل لها رن بالخلفية ويعمل ارسال الرسالة
            //await PublishDomainEventsAsync();

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
        }
    }

    // مشكلة الدومين ايفنت ممكن تفشل لذلك راح نستخدم ال Outbox pattern بحيث نخزن الدومين ايفنت في جدول خاص ونستخدم خدمة خارجية لنشرها لاحقاً. هذا يقلل من احتمالية فشل نشر الدومين ايفنت ويضمن استمرارية النظام.
    //private async Task PublishDomainEventsAsync()
    private void AddDomainEventsAsOutboxMessages()

    {
        //var domainEvents = ChangeTracker
        var outboxMessages = ChangeTracker
             .Entries<Entity>()
             .Select(entry => entry.Entity)
             .SelectMany(entity =>
             {
                 var domainEvents = entity.GetDomainEvents();

                 entity.ClearDomainEvents();

                 return domainEvents;
             })
             // start
             // added for the outbox pattern
             .Select(domainEvent => new OutboxMessage(
                 Guid.NewGuid(),
                 _dateTimeProvider.DateTimeNow,
                 domainEvent.GetType().Name,// user, booking, order, etc
                 JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
             // end 
             .ToList();

        // start
        // added for the outbox pattern
        AddRange(outboxMessages);
        // end 


        //foreach (var domainEvent in domainEvents)
        //{
        //    await _publisher.Publish(domainEvent);
        //}
    }
}