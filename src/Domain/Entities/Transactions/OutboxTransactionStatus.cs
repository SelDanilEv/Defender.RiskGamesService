using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Defender.Common.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.Common.DB.SharedStorage.Entities;

namespace Defender.RiskGamesService.Domain.Entities.Transactions;

public record OutboxTransactionStatus : IBaseModel
{
    public Guid Id { get; set; }
    public string? TransactionId { get; set; }
    [BsonRepresentation(BsonType.String)]
    public TransactionStatus TransactionStatus { get; set; }
    [BsonRepresentation(BsonType.String)]
    public TransactionType TransactionType { get; set; }
    [BsonRepresentation(BsonType.String)]
    public TransactionPurpose TransactionPurpose { get; set; }
    public int Attempt { get; set; } = 0;
    public Guid HandlerId { get; private set; } = Guid.Empty;


    public static OutboxTransactionStatus CreateFromStatusUpdatedEvent(TransactionStatusUpdatedEvent t)
    {
        return new()
        {
            TransactionId = t.TransactionId,
            TransactionStatus = t.TransactionStatus,
            TransactionType = t.TransactionType,
            TransactionPurpose = t.TransactionPurpose,
        };
    }

    public TransactionStatusUpdatedEvent ConvertToStatusUpdatedEvent()
    {
        return new()
        {
            TransactionId = TransactionId,
            TransactionStatus = TransactionStatus,
            TransactionType = TransactionType,
            TransactionPurpose = TransactionPurpose,
        };
    }

}


