using System.Threading.Channels;
using Claims.Domain.Auditing;
using Claims.Infrastructure.Auditing;
using Xunit;

namespace Claims.Tests;

/// <summary>
/// Unit tests for <see cref="AuditService"/>.
/// </summary>
public class AuditServiceTests
{
    [Fact]
    public void EnqueueAudit_ClaimAudit_WritesToChannel()
    {
        var channel = Channel.CreateUnbounded<object>();
        var service = new AuditService(channel);

        var audit = new ClaimAudit
        {
            ClaimId = "test-id",
            Created = DateTime.UtcNow,
            HttpRequestType = "POST"
        };

        service.EnqueueAudit(audit);

        Assert.True(channel.Reader.TryRead(out var result));
        Assert.Same(audit, result);
    }

    [Fact]
    public void EnqueueAudit_CoverAudit_WritesToChannel()
    {
        var channel = Channel.CreateUnbounded<object>();
        var service = new AuditService(channel);

        var audit = new CoverAudit
        {
            CoverId = "cover-id",
            Created = DateTime.UtcNow,
            HttpRequestType = "DELETE"
        };

        service.EnqueueAudit(audit);

        Assert.True(channel.Reader.TryRead(out var result));
        Assert.Same(audit, result);
    }

    [Fact]
    public void EnqueueAudit_DoesNotBlock()
    {
        var channel = Channel.CreateUnbounded<object>();
        var service = new AuditService(channel);

        // Enqueue multiple items to verify non-blocking behavior
        for (int i = 0; i < 100; i++)
        {
            service.EnqueueAudit(new ClaimAudit
            {
                ClaimId = $"claim-{i}",
                Created = DateTime.UtcNow,
                HttpRequestType = "POST"
            });
        }

        var count = 0;
        while (channel.Reader.TryRead(out _))
        {
            count++;
        }

        Assert.Equal(100, count);
    }
}
