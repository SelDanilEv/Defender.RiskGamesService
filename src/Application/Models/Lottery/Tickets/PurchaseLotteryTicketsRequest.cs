using Defender.Common.DB.SharedStorage.Enums;
using Defender.Common.Errors;
using Defender.Common.Exceptions;
using Defender.RiskGamesService.Application.Models.Transaction;
using Defender.RiskGamesService.Domain.Entities.Lottery.Draw;
using Defender.RiskGamesService.Domain.Entities.Lottery.UserTickets;
using Defender.RiskGamesService.Domain.Enums;
using Defender.RiskGamesService.Domain.Helpers;

namespace Defender.RiskGamesService.Application.Models.Lottery.Tickets;

public record PurchaseLotteryTicketsRequest
{
    public long DrawNumber { get; set; }
    public int Amount { get; set; }
    public Currency Currency { get; set; }

    public HashSet<int> TicketNumbers { get; set; } = [];

    public int TotalAmount => TicketNumbers?.Count * Amount ?? 0;

    public List<UserTicket> ToUserTickets(Guid userId, string paymentTransactionId)
    {
        return TicketNumbers == null || TicketNumbers.Count == 0
            ? []
            : TicketNumbers.Select(x => new UserTicket
            {
                DrawNumber = DrawNumber,
                TicketNumber = x,
                Amount = Amount,
                Currency = Currency,
                UserId = userId,
                PaymentTransactionId = paymentTransactionId
            }).ToList();
    }

    public PurchaseLotteryTicketsRequest ValidateTickets(LotteryDraw draw)
    {
        if (!draw.AllowedCurrencies.Contains(Currency))
        {
            throw new ServiceException(ErrorCode.BR_RGS_CurrencyIsNotAllowed);
        }

        if (!draw.IsCustomBetAllowed && !draw.AllowedBets.Contains(Amount))
        {
            throw new ServiceException(ErrorCode.BR_RGS_ThisBetIsNotAllowed);
        }

        if (Amount < draw.MinBetValue || Amount > draw.MaxBetValue)
        {
            throw new ServiceException(ErrorCode.BR_RGS_ThisBetIsNotAllowed);
        }

        TicketNumbers = TicketNumbers
            .Where(x => x >= draw.MinTicketNumber && x <= draw.MaxTicketNumber)
            .ToHashSet();

        if (TicketNumbers.Count == 0)
        {
            throw new ServiceException(ErrorCode.BR_RGS_TryingToPurchaseInvalidTickets);
        }

        return this;
    }

    public PurchaseLotteryTicketsRequest ExcludeTakenTickets(List<int> userTickets)
    {
        TicketNumbers = TicketNumbers
            .Where(x => !userTickets.Contains(x))
            .ToHashSet();

        if (TicketNumbers.Count == 0)
            throw new ServiceException(ErrorCode.BR_RGS_TryingToPurchaseInvalidTickets);

        return this;
    }

    public TransactionRequest CreatePurchaseTransactionRequest() => new(
        DrawNumber.ToString(),
        TotalAmount,
        Currency,
        TransactionType.Payment,
        GameType.Lottery,
        LotteryHelpers.PurchaseLotteryTicketsTransactionComment(
            DrawNumber, TicketNumbers));
}
