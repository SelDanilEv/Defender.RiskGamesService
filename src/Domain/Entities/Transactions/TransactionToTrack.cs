using Defender.Common.Entities;
using Defender.RiskGamesService.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Defender.RiskGamesService.Domain.Entities.Transactions;

public record TransactionToTrack : IBaseModel
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public string? TransactionId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public GameType GameType { get; set; }

    public string? DrawId { get; set; }
}


