using Claims.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain.Entities;

public class Claim
{
    [BsonId]
    public string Id { get; set; } = string.Empty;

    [BsonElement("coverId")]
    public string CoverId { get; set; }

    [BsonElement("created")]
    public DateOnly Created { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("claimType")]
    public ClaimType Type { get; set; }

    [BsonElement("damageCost")]
    public decimal DamageCost { get; set; }
}
