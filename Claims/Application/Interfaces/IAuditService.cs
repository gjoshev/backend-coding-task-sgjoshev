namespace Claims.Application.Interfaces;

/// <summary>
/// Defines the contract for non-blocking audit event submission.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Enqueues an audit entity for asynchronous persistence.
    /// The caller is not blocked while the audit record is saved.
    /// </summary>
    /// <param name="auditEntity">The audit entity to persist (e.g., <see cref="Domain.Auditing.ClaimAudit"/> or <see cref="Domain.Auditing.CoverAudit"/>).</param>
    void EnqueueAudit(object auditEntity);
}
