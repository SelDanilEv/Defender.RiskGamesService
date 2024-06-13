using Defender.Common.DB.SharedStorage.Entities;
using Defender.RiskGamesService.Application.Models.Transaction;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Handlers;


public interface IGameTransactionHandler
{
    Task<HandleGameTransactionResult> HandleGameTransactionAsync(TransactionStatusUpdatedEvent transactionInfo);
}
