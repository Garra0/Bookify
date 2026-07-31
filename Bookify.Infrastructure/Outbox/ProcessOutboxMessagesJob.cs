using System.Data;
using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution] // تتأكد من عدم تشغيل اكثر من جوب بنفس الوقت 
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly ISqlConnectionFactory _sqlConnectionFactory; // using dapper
    private readonly IPublisher _publisher; // publish the domain event 
    private readonly IDateTimeProvider _dateTimeProvider;// time now
    private readonly OutboxOptions _outboxOptions;// get batch size from configuration
    private readonly ILogger<ProcessOutboxMessagesJob> _logger;

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _outboxOptions = outboxOptions.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();

        // 1 DB Hit: جلب واستعلام الرسائل المحظورة بشرط واحد وبمرة واحدة فقط
        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        if (!outboxMessages.Any())
        {
            // عند خلو الرسائل، يتم تسجيل اللوج بمستوى LogDebug لتجنب تلوث السجلات الرئبيسية، 
            // وفي نفس الوقت يتيح تتبع عمل الجوب عند الحاجة بتفعيل Debug Level
            _logger.LogDebug("No outbox messages found to process");
            transaction.Commit();
            return;
        }

        _logger.LogInformation("Beginning to process outbox messages ({Count} messages)", outboxMessages.Count);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {
                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                    outboxMessage.Content,
                    JsonSerializerSettings)!;

                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id);

                exception = caughtException;
            }

            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }

        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");
    }

    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(
        IDbConnection connection,
        IDbTransaction transaction)
    {
        // WHERE processed_on IS NULL --> لسا ما تم معالجة الرسالة
        var sql = $"""                
            SELECT id, content
            FROM outbox_messages
            WHERE processed_on IS NULL 
            ORDER BY occurred_on
            LIMIT {_outboxOptions.BatchSize}
            FOR UPDATE
            """;

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, transaction: transaction);

        return outboxMessages.ToList();
    }

    private async Task UpdateOutboxMessageAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        OutboxMessageResponse outboxMessage,
        Exception? exception)
    {
        const string sql = @"
            UPDATE outbox_messages
            SET processed_on = @ProcessedOn,
                error = @Error
            WHERE id = @Id";

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOn = _dateTimeProvider.DateTimeNow,
                Error = exception?.ToString()
            },
            transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
