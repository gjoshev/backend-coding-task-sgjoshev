using Claims.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain.Entities;

public class Cover
{
    [BsonId]
    public string? Id { get; set; }

    [BsonElement("startDate")]
    public DateOnly StartDate { get; set; }

    [BsonElement("endDate")]
    public DateOnly EndDate { get; set; }

    [BsonElement("claimType")]
    public CoverType Type { get; set; }

    [BsonElement("premium")]
    public decimal Premium { get; set; }
}
