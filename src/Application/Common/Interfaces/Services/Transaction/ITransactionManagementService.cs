using Defender.Common.DB.SharedStorage.Entities;
using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Entities.Transactions;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;

public interface ITransactionManagementService
{
    Task HandleTransactionStatusUpdatedEvent(TransactionStatusUpdatedEvent @event);
    Task<AnonymousTransactionModel?> TryGetTransactionInfoAsync(string? transactionId);
    Task<StartTransactionResult> StartTransactionAsync(TransactionRequest request);
    Task CheckUnhandledTicketsForDrawAsync(string drawId, GameType gameType);
    Task StopTrackTransactionAsync(string? transactionId);
}
