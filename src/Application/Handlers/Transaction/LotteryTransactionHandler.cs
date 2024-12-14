using Defender.Common.DB.Model;
using Defender.Common.DB.SharedStorage.Entities;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.RiskGamesService.Application.Common.Interfaces.Handlers;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;

namespace Defender.RiskGamesService.Application.Handlers.Transaction;


public class LotteryTransactionHandler(
    ILotteryUserTicketRepository userTicketRepository)
    : IGameTransactionHandler
{
    public async Task<HandleGameTransactionResult> HandleGameTransactionAsync(
        TransactionStatusUpdatedEvent transactionInfo)
    {
        var findRequest = FindModelRequest<UserTicket>.Init();
        var updateRequest = UpdateModelRequest<UserTicket>.Init();

        switch (transactionInfo.TransactionType, transactionInfo.TransactionStatus)
        {
            case (TransactionType.Payment, TransactionStatus.Proceed):
                findRequest
                    .And(x => x.PaymentTransactionId, transactionInfo.TransactionId)
                    .And(x => x.Status, UserTicketStatus.Requested);

                updateRequest = updateRequest
                    .Set(x => x.Status, UserTicketStatus.Paid);

                await userTicketRepository.UpdateManyUserTicketsAsync(findRequest, updateRequest);

                return HandleGameTransactionResult.KeepTrackingResult;
            case (TransactionType.Payment, TransactionStatus.Failed):
            case (TransactionType.Payment, TransactionStatus.Reverted):
            case (TransactionType.Payment, TransactionStatus.Canceled):
                await userTicketRepository.DeleteTicketByPaymentTransactionIdAsync(transactionInfo.TransactionId);
                break;

            case (TransactionType.Recharge, TransactionStatus.Proceed):
                findRequest
                    .And(x => x.PrizeTransactionId, transactionInfo.TransactionId)
                    .And(x => x.Status, UserTicketStatus.Won);

                updateRequest
                    .Set(x => x.Status, UserTicketStatus.PrizePaid);

                await userTicketRepository.UpdateManyUserTicketsAsync(findRequest, updateRequest);
                break;

            default:
                return HandleGameTransactionResult.KeepTrackingResult;
        }

        return HandleGameTransactionResult.StopTrackingResult;
    }
}

