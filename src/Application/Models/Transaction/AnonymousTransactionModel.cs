using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;

namespace Defender.RiskGamesService.Application.Models.Transaction;

public class AnonymousTransactionModel
{
    public string? TransactionId { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public TransactionPurpose TransactionPurpose { get; set; }
    public TransactionType TransactionType { get; set; }

    public TransactionStatusUpdatedEvent ConvertToStatusUpdatedEvent()
    {
        return new TransactionStatusUpdatedEvent
        {
            TransactionId = TransactionId,
            TransactionStatus = TransactionStatus,
            TransactionPurpose = TransactionPurpose,
            TransactionType = TransactionType,
        };
    }
}
