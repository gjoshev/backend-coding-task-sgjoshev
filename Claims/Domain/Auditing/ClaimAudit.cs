namespace Claims.Domain.Auditing;

public class ClaimAudit
{
    public int Id { get; set; }

    public string ClaimId { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public string HttpRequestType { get; set; } = string.Empty;
}
