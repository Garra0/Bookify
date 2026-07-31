using Microsoft.Extensions.Diagnostics.HealthChecks;
using Quartz;

namespace Bookify.Infrastructure.Outbox;

internal sealed class ProcessOutboxMessagesJobHealthCheck : IHealthCheck
{
    private readonly ISchedulerFactory _schedulerFactory;

    public ProcessOutboxMessagesJobHealthCheck(ISchedulerFactory schedulerFactory)
    {
        _schedulerFactory = schedulerFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);

            if (scheduler.IsStarted && !scheduler.IsShutdown)
            {
                return HealthCheckResult.Healthy("Outbox background job scheduler is running.");
            }

            return HealthCheckResult.Unhealthy("Outbox background job scheduler is not running.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Failed to verify outbox background job scheduler status.", exception);
        }
    }
}
