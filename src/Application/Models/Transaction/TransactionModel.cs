using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.RiskGamesService.Application.Mappings;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Models.Transaction;

public class TransactionModel
{
    public string? TransactionId { get; set; }
    public string? ParentTransactionId { get; set; }
    public TransactionStatus TransactionStatus { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionPurpose TransactionPurpose { get; set; }
    public Currency Currency { get; set; }

    public int Amount { get; set; }
    public DateTime UtcTransactionDate { get; set; }

    public int FromWallet { get; set; }
    public int ToWallet { get; set; }


    public TransactionToTrack ConvertToTransactionToTrack(string drawId)
    {
        return new TransactionToTrack
        {
            TransactionId = TransactionId,
            GameType = TransactionMapper.MapGameType(TransactionPurpose),
            DrawId = drawId,
        };
    }
}
