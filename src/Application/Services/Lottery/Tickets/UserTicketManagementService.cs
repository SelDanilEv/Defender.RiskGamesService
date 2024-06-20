using Defender.Common.DB.Model;
using Defender.Common.DB.Pagination;
using Defender.Common.DB.SharedStorage.Enums;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.Common.Interfaces;
using Defender.RiskGamesService.Application.Common.Interfaces.Repositories.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Lottery;
using Defender.RiskGamesService.Application.Common.Interfaces.Services.Transaction;
using Defender.RiskGamesService.Application.Models.Lottery.Tickets;
using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;
using Defender.RiskGamesService.Domain.Enums;
using Defender.RiskGamesService.Domain.Helpers;

namespace Defender.RiskGamesService.Application.Services.Lottery.Tickets;

public class UserTicketManagementService(
        ICurrentAccountAccessor currentAccountAccessor,
        ILotteryManagementService lotteryManagementService,
        ILotteryUserTicketRepository userTicketRepository,
        ITransactionManagementService transactionManagementService)
    : IUserTicketManagementService
{
    public Task<PagedResult<UserTicket>> GetUserTicketsAsync(
        PaginationRequest request, Guid userId)
    {
        return userTicketRepository.GetUserTicketsByUserIdAsync(request, userId);
    }

    public Task<List<UserTicket>> GetUserTicketsByDrawNumberAsync(long drawNumber)
    {
        return userTicketRepository
            .GetUserTicketsByDrawNumberAsync(drawNumber);
    }

    public async Task<IEnumerable<int>> PurchaseTicketsAsync(
        PurchaseLotteryTicketsRequest request)
    {
        if (request == null || request?.TicketNumbers?.Count == 0)
        {
            return [];
        }

        var lotteryDraw = await lotteryManagementService
            .GetLotteryDrawByNumberAsync(request.DrawNumber);

        if (lotteryDraw == null || !lotteryDraw.IsActive)
        {
            throw new ServiceException(ErrorCode.BR_RGS_LotteryDrawIsNotActive);
        }

        if (DateTime.UtcNow.AddMinutes(4)  > lotteryDraw.EndDate)
        {
            throw new ServiceException(ErrorCode.BR_RGS_TicketPurchaseNotAllowed);
        }

        var currentUserId = currentAccountAccessor.GetAccountId();

        var getUserTicketsTask = GetUserTicketsByDrawNumberAsync(request.DrawNumber);

        request.ValidateTickets(lotteryDraw)
            .ExcludeTakenTickets((await getUserTicketsTask)
                .Select(x => x.TicketNumber).ToList());

        var paymentRequest = request.CreatePurchaseTransactionRequest();

        var startTransactionResult = await transactionManagementService
            .StartTransactionAsync(paymentRequest);

        var createUserTicketTask = userTicketRepository.CreateUserTicketsAsync(
            request.ToUserTickets(
                currentUserId,
                startTransactionResult.Transaction.TransactionId));

        await Task.WhenAll(
            startTransactionResult.CreateTransactionToTrackTask,
            createUserTicketTask);

        return request.TicketNumbers;
    }

    public async Task CheckWinningsAsync(LotteryDraw draw)
    {
        if (draw == null || draw.Winnings == null || draw.Winnings?.Count == 0)
        {
            return;
        }

        if (draw.IsActive)
        {
            throw new ServiceException(ErrorCode.BR_RGS_LotteryIsStillActive);
        }

        var userTickets = await GetUserTicketsByDrawNumberAsync(draw.DrawNumber);

        List<Task> tasks = [];

        foreach (var userTicket in userTickets)
        {
            if (userTicket.PaymentTransactionId != null)
            {
                tasks.Add(transactionManagementService
                    .StopTrackTransactionAsync(userTicket.PaymentTransactionId));
            }

            var isWinning = false;

            foreach (var winning in draw.Winnings!)
            {
                if (winning.Tickets.Contains(userTicket.TicketNumber))
                {
                    tasks.AddRange(await HandleWinningTicketAsync(userTicket, winning));
                    isWinning = true;
                    break;
                }
            }

            if (isWinning)
            {
                continue;
            }

            var updateRequest = UpdateModelRequest<UserTicket>
                .Init(userTicket.Id)
                .Set(x => x.Status, UserTicketStatus.Lost);

            tasks.Add(userTicketRepository
                .UpdateUserTicketAsync(updateRequest));
        }

        await Task.WhenAll(tasks);
    }

    private async Task<IEnumerable<Task>> HandleWinningTicketAsync(
        UserTicket userTicket,
        Winning winning)
    {
        var tasks = new List<Task>();

        var transactionRequest = new TransactionRequest(
            userTicket.DrawNumber.ToString(),
            LotteryHelpers.
                CalculateLotteryPrizeAmount(userTicket.Amount, winning.Coefficient),
            userTicket.Currency,
            TransactionType.Recharge,
            GameType.Lottery,
            LotteryHelpers
                .LotteryPrizeTransactionComment(
                    userTicket.DrawNumber,
                    userTicket.TicketNumber,
                    userTicket.Amount,
                    winning.Coefficient))
        .SetUserId(userTicket.UserId);

        var startTransactionResult = await transactionManagementService
            .StartTransactionAsync(transactionRequest);

        tasks.Add(startTransactionResult.CreateTransactionToTrackTask);

        var updateTicketRequest = UpdateModelRequest<UserTicket>
            .Init(userTicket.Id)
            .Set(x => x.Status, UserTicketStatus.Won)
            .Set(x => x.PrizePaidAmount, transactionRequest.Amount)
            .Set(x => x.PrizeTransactionId,
                startTransactionResult.Transaction.TransactionId);

        tasks.Add(userTicketRepository.UpdateUserTicketAsync(
            updateTicketRequest));

        return tasks;
    }
}
