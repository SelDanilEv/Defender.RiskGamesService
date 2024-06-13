using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Wrapper;

public interface IWalletWrapper
{
    Task<AnonymousTransactionModel> GetTransactionAsync(
        string transactionId);

    Task<TransactionModel> StartPaymentTransactionAsync(
        Guid userId,
        int amount,
        Currency currency,
        GameType gameType,
        string? comment = null);

    Task<TransactionModel> StartRechargeTransactionAsync(
        Guid userId,
        int amount,
        Currency currency,
        GameType gameType,
        string? comment = null);
}
