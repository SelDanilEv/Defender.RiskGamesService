using Defender.RiskGamesService.Domain.Entities.Transactions;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;

public interface ITransactionToTrackRepository
{
    Task<List<TransactionToTrack>> GetTransactionsAsync(string drawId, GameType gameType);
    Task<TransactionToTrack> GetTransactionAsync(string transactionId);
    Task<TransactionToTrack> CreateTransactionAsync(TransactionToTrack newModel);
    Task DeleteTransactonAsync(string transactionId);
}
