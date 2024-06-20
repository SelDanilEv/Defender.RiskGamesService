using Defender.RiskGamesService.Domain.Entities.Transactions;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;

public interface IOutboxTransactionStatusRepository
{
    Task<List<OutboxTransactionStatus>> GetAllAsync();
    Task<OutboxTransactionStatus> CreateTransactionStatusAsync(OutboxTransactionStatus newTransaction);
    Task<bool> TryLockTransactionStatusToHandleAsync(Guid id);
    Task ReleaseTransactionStatusAsync(Guid id);
    Task<OutboxTransactionStatus> IncreaseAttemptCountAsync(OutboxTransactionStatus transaction);
    Task DeleteTransactonStatusAsync(Guid id);
}
