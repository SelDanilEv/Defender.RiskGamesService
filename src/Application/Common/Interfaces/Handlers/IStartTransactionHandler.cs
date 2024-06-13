using Defender.RiskGamesService.Application.Models.Transaction;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Handlers;


public interface IStartTransactionHandler
{
    Task<TransactionModel> HandleStartTransactionAsync(TransactionRequest request);
}
