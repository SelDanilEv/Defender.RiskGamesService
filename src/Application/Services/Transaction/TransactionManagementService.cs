using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Transactions;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Defender.RiskGamesService.Application.Common.Interfaces.Wrapper;
using Defender.RiskGamesService.Application.Factories.Transaction;
using Defender.RiskGamesService.Application.Mappings;
using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Enums;

namespace Defender.RiskGamesService.Application.Services.Transaction;

public class TransactionManagementService(
        IWalletWrapper walletWrapper,
        ITransactionToTrackRepository transactionToTrackRepository,
        TransactionHandlerFactory transactionHandlerFactory)
    : ITransactionManagementService
{
    public async Task HandleTransactionStatusUpdatedEvent(
        TransactionStatusUpdatedEvent @event)
    {
        await HandleTransactionStatusUpdatedAsync(@event);
    }

    public async Task<AnonymousTransactionModel?> TryGetTransactionInfoAsync(
        string? transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            return null;
        }

        try
        {
            return await walletWrapper.GetTransactionAsync(transactionId);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<StartTransactionResult> StartTransactionAsync(
        TransactionRequest request)
    {
        if (request == null
            || string.IsNullOrWhiteSpace(request.DrawId)
            || request.TransactionType == TransactionType.Unknown
            || request.GameType == GameType.Undefined)
        {
            throw new ServiceException(ErrorCode.BR_RGS_InvalidPaymentRequest);
        }

        if (request.Amount <= 0)
        {
            throw new ServiceException(ErrorCode.BR_RGS_InvalidTransactionAmount);
        }

        var startTransactionHandler = transactionHandlerFactory.StartTransactionHandler(
            request.TransactionType);

        var transaction = await startTransactionHandler
            .HandleStartTransactionAsync(request);

        var createTransactionToTrackTask = transactionToTrackRepository
            .CreateTransactionAsync(
                transaction.ConvertToTransactionToTrack(request.DrawId));

        return new StartTransactionResult(transaction, createTransactionToTrackTask);
    }


    public async Task CheckUnhandledTicketsForDrawAsync(string drawId, GameType gameType)
    {
        var unhandledTransactions = await transactionToTrackRepository
            .GetTransactionsAsync(drawId, gameType);

        var getTransactionInfoTasks = unhandledTransactions
            .Select(x => x.TransactionId)
            .Select(TryGetTransactionInfoAsync);

        var deleteTransactionTasks = unhandledTransactions
            .Select(x => x.TransactionId)
            .Select(StopTrackTransactionAsync);

        var getTransactionInfoResult = await Task.WhenAll(getTransactionInfoTasks);

        var ticketsUpdateTasks = getTransactionInfoResult
            .Select(x => x?.ConvertToStatusUpdatedEvent())
            .Select(HandleTransactionStatusUpdatedAsync);

        var allTasks = ticketsUpdateTasks.Concat(deleteTransactionTasks);

        await Task.WhenAll(allTasks);
    }

    public Task StopTrackTransactionAsync(string? transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
        {
            return Task.CompletedTask;
        }

        return transactionToTrackRepository.DeleteTransactonAsync(transactionId);
    }

    private async Task<bool> HandleTransactionStatusUpdatedAsync(
        TransactionStatusUpdatedEvent? transactionInfo)
    {
        if (transactionInfo == null
            || string.IsNullOrWhiteSpace(transactionInfo.TransactionId))
        {
            return false;
        }

        var transactionToTrack = await transactionToTrackRepository
            .GetTransactionAsync(transactionInfo.TransactionId);

        if (transactionToTrack is null)
        {
            return false;
        }

        var gameTransactionHandler = transactionHandlerFactory
            .CreateGameTransactionHandler(
                TransactionMapper.MapGameType(transactionInfo.TransactionPurpose));

        var handleResult = await gameTransactionHandler
            .HandleGameTransactionAsync(transactionInfo);

        if (handleResult.StopTracking)
            await StopTrackTransactionAsync(transactionInfo.TransactionId);

        return true;
    }
}
