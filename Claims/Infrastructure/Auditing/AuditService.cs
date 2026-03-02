using Claims.Application.Interfaces;
using System.Threading.Channels;

namespace Claims.Infrastructure.Auditing;

/// <summary>
/// In-memory audit service that enqueues audit events into a <see cref="Channel{T}"/>
/// for asynchronous processing by <see cref="AuditBackgroundService"/>.
/// This ensures that audit persistence does not block HTTP request processing.
/// </summary>
public class AuditService : IAuditService
{
    private readonly Channel<object> _channel;

    public AuditService(Channel<object> channel)
    {
        _channel = channel;
    }

    /// <inheritdoc />
    public void EnqueueAudit(object auditEntity)
    {
        _channel.Writer.TryWrite(auditEntity);
    }
}
