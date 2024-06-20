using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Defender.Common.Entities;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Domain.Entities.Transactions;

public record TransactionToTrack : IBaseModel
{
    public Guid Id { get; set; }

    public string? TransactionId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public GameType GameType { get; set; }

    public string? DrawId { get; set; }
}


