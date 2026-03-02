using System.Threading.Channels;
using Claims.Auditing;

namespace Claims.Services;

/// <summary>
/// Background service that consumes audit events from an in-memory channel
/// and persists them to the audit database asynchronously.
/// </summary>
public class AuditBackgroundService : BackgroundService
{
    private readonly Channel<object> _channel;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditBackgroundService> _logger;

    public AuditBackgroundService(
        Channel<object> channel,
        IServiceScopeFactory scopeFactory,
        ILogger<AuditBackgroundService> logger)
    {
        _channel = channel;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var auditEntity in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
                context.Add(auditEntity);
                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist audit record.");
            }
        }
    }
}
