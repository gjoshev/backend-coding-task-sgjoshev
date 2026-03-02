namespace Claims.Domain.Auditing;

public class CoverAudit
{
    public int Id { get; set; }

    public string CoverId { get; set; } = string.Empty;

    public DateTime Created { get; set; }

    public string HttpRequestType { get; set; } = string.Empty;
}
